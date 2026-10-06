using ChessUniverse.Library;
using ChessUniverse.Library.Enums;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessUniverse_WPF;

public partial class MainWindow : Window
{
    // Ցույց է տալիս՝ մկնիկը սեղմված է արդյոք (drag-ի վիճակ)
    private bool _t;

    //Մկնիկի սեղմման ժամանակ կորդինատների ֆիքսում
    private System.Windows.Point _ptLast = new System.Windows.Point();
    private int _imgDownX;
    private int _imgDownY;
    private int _cellSize = 57;

    private bool audioPlayed;

    private Image? boardEnteredImage;
    private MoveInfo? _moveInfo;

    private readonly ChessGame _game = new();

    private ChessBoard pieceBoard => _game.Board;
    private PieceColor acctiveTurn => _game.ActiveTurn;
    private MoveInfo? _previousMove => _game.PreviousMove;

    private static string GetSoundPath(string fileName)
        => Path.Combine(AppContext.BaseDirectory, "Sounds", fileName);
    private void RenderBoard(ChessBoard board)
    {
        grid_figure.Children.Clear();

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Piece? piece = board[row, col];

                if (piece is not null)
                    grid_figure.Children.Add(CreatePieceImage(piece));
            }
        }
    }
    private void RenderCapturedPieces()
    {
        WhiteCaptures.Children.Clear();
        BlackCaptures.Children.Clear();

        foreach (Piece piece in _game.CapturedPieces)
        {
            string color = piece.Color == PieceColor.White ? "white" : "black";

            string fileName = piece.Type switch
            {
                PieceType.Pawn => "soldier",
                PieceType.Rook => "ship",
                PieceType.Knight => "horse",
                PieceType.Bishop => "elephant",
                PieceType.Queen => "queen",
                PieceType.King => "king",
                _ => throw new InvalidOperationException(
                    $"Unknown piece type: {piece.Type}")
            };

            Image capturedImage = new Image
            {
                Source = new BitmapImage(new Uri(
                    $"/images/figures/{color}-{fileName}.png",
                    UriKind.Relative)),
                Width = 20,
                Height = 20,
                Stretch = Stretch.Fill,
                IsHitTestVisible = false,
                Margin = new Thickness(0)
            };

            if (piece.Color == PieceColor.Black)
                BlackCaptures.Children.Add(capturedImage);
            else
                WhiteCaptures.Children.Add(capturedImage);
        }
    }
    private void UpdateHistoryButtons()
    {
        Previous.IsEnabled = _game.CanUndo;
        Next.IsEnabled = _game.CanRedo;
    }
    private Image CreatePieceImage(Piece piece)
    {
        string color = piece.Color == PieceColor.White ? "white" : "black";
        string namePrefix = piece.Color == PieceColor.White ? "w" : "b";

        var (fileName, tag, width, height) = piece.Type switch
        {
            PieceType.Pawn => ("soldier", "pawn", 35d, 50d),
            PieceType.Rook => ("ship", "rook", 45d, 50d),
            PieceType.Knight => ("horse", "knight", 50d, 50d),
            PieceType.Bishop => ("elephant", "bishop", 43d, 50d),
            PieceType.Queen => ("queen", "queen", 50d, 42d),
            PieceType.King => ("king", "king", 50d, 45d),
            _ => throw new InvalidOperationException(
                $"Unknown piece type: {piece.Type}")
        };

        Image image = new Image
        {
            Name = $"{namePrefix}_piece_{piece.Position.Row}_{piece.Position.Col}",
            Tag = tag,
            Source = new BitmapImage(new Uri(
                $"/images/figures/{color}-{fileName}.png",
                UriKind.Relative)),
            Width = width,
            Height = height,
            Stretch = Stretch.Fill,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(
                piece.Position.Col * _cellSize + (_cellSize - width) / 2,
                piece.Position.Row * _cellSize + (_cellSize - height) / 2,
                0,
                0)
        };

        image.MouseDown += OnPieceMouseDown;
        image.MouseMove += OnPieceMouseMove;
        image.MouseUp += OnPieceMouseUp;

        return image;
    }
    public MainWindow()
    {
        InitializeComponent();

        StartNewGame();

        SoundManager.Load("start", GetSoundPath("game-start.mp3"));
        SoundManager.Load("move", GetSoundPath("move-self.mp3"));
        SoundManager.Load("promotion", GetSoundPath("promote.mp3"));
        SoundManager.Load("castle", GetSoundPath("castle.mp3"));
        SoundManager.Load("capture", GetSoundPath("capture.mp3"));
        SoundManager.Load("invalidMove", GetSoundPath("illegal.mp3"));
        SoundManager.Load("check", GetSoundPath("move-check.mp3"));
        SoundManager.Load("checkMate", GetSoundPath("game-end.mp3"));
        this.ResizeMode = ResizeMode.CanMinimize;
    }

    #region EVENTS
    private void OnPieceMouseDown(object sender, MouseEventArgs e)
    {
        _t = true;
        var img = (System.Windows.Controls.Image)sender;
        boardEnteredImage = img;
        _ptLast = e.GetPosition(img);

        Mouse.Capture(img);
        StackPanel.SetZIndex(img, 1);

        label1.Content = "X: " + img.Margin.Left.ToString();
        label2.Content = "Y: " + img.Margin.Top.ToString();

        _imgDownX = (int)img.Margin.Left;
        _imgDownY = (int)img.Margin.Top;
    }
    private void OnPieceMouseMove(object sender, MouseEventArgs e)
    {
        if (_t)
        {
            var img = (System.Windows.Controls.Image)sender;
            var ptNew = new System.Windows.Point();

            ptNew.X = img.Margin.Left;
            ptNew.Y = img.Margin.Top;

            img.Margin = new Thickness(ptNew.X + (e.GetPosition(img).X - _ptLast.X),
                ptNew.Y + (e.GetPosition(img).Y - _ptLast.Y), 0, 0);
        }
    }
    private void OnPieceMouseUp(object sender, MouseEventArgs e)
    {
        _t = false;
        audioPlayed = false;
        MoveType currentMoveType;

        //Ընտրված ֆիգուրի նախնական կորդինատի փոխակերպումը երկչափ զանգվածի տողի և սյան
        int enteredPieceRow = (int)(_imgDownY + 28.5) / 57;
        int enteredPieceCol = (int)(_imgDownX + 28.5) / 57;
        PiecePosition enteredPiece = new PiecePosition(enteredPieceRow, enteredPieceCol);

        //Մկնիկով նշված վայրի – կորդինատի փոխակերպումը երկչափ զանգվածի տողի և սյան
        var img = (System.Windows.Controls.Image)sender;
        int row = (int)(img.Margin.Top + 28.5) / 57;
        int col = (int)(img.Margin.Left + 28.5) / 57;
        row = Math.Clamp(row, 0, 7);
        col = Math.Clamp(col, 0, 7);
        PiecePosition imgPosition = new PiecePosition(row, col);

        MoveInfo moveInfo = new MoveInfo
            (enteredPiece, imgPosition);

        MoveResult moveDetails = _game.TryMove(moveInfo);
        currentMoveType = moveDetails.MoveType;

        if (currentMoveType == MoveType.PawnPromotion)
        {

            ShowPromotionOverlay(img, moveInfo);
            MoveUIUpdate(img, moveInfo, currentMoveType);
        }
        else
        {
            BoardStateUpdate(moveDetails);
            MoveUIUpdate(img, moveInfo, currentMoveType);

            if (currentMoveType != MoveType.InvalidMove)
            {
                UpdateHistoryButtons();

                RenderBoard(pieceBoard);
                RenderCapturedPieces();
            }
        }

        Mouse.Capture(null);
        StackPanel.SetZIndex(img, 0);

        label3.Content = "M: " + img.Margin.Left.ToString() + " " + img.Margin.Top.ToString();
    }
    private void PromotionClick(object sender, EventArgs e)
    {
        if (sender is not Image selectedImg)
            return;

        string? tag = selectedImg.Tag?.ToString();

        PieceType? promotionType = tag switch
        {
            "Queen" => PieceType.Queen,
            "Rook" => PieceType.Rook,
            "Bishop" => PieceType.Bishop,
            "Knight" => PieceType.Knight,
            _ => null
        };

        if (promotionType is null)
            return;

        MoveResult result = _game.PromotePawn(promotionType.Value);

        if (result.MoveType == MoveType.InvalidMove)
            return;

        _moveInfo = null;
        boardEnteredImage = null;

        WhitePromotionOverlay.Visibility = Visibility.Collapsed;
        BlackPromotionOverlay.Visibility = Visibility.Collapsed;

        RenderBoard(pieceBoard);
        RenderCapturedPieces();

        MoveShower.Content = acctiveTurn.ToString();

        BoardStateUpdate(result);
        UpdateHistoryButtons();
    }
    private void PreviousClick(object sender, RoutedEventArgs e)
    {
        if (!_game.Undo())
            return;

        RefreshGameUI();
        SoundManager.Play("move");
    }
    private void NextClick(object sender, RoutedEventArgs e)
    {
        if (!_game.Redo())
            return;

        RefreshGameUI();
        SoundManager.Play("move");
    }
    private void NewGameClick(object sender, RoutedEventArgs e)
    {
        StartNewGame();
        SoundManager.Play("start");
    }
    #endregion

    #region MOVE_UI
    /// <summary>
    /// Տեղափոխում է վերցված ֆիգուրը խաղատախտակից դեպի captures panel
    /// և հարմարեցնում է դրա տեսքն ու behavior-ը
    /// </summary>
    /// <param name="imgCaptured">Վերցված ֆիգուրի պատկերը</param>
    private void AddingCaptureToWrap(Image? imgCaptured)
    {
        if (imgCaptured is null)
            return;

        if (!audioPlayed)
            SoundManager.Play("capture"); audioPlayed = true;

        grid_figure.Children.Remove(imgCaptured);
        imgCaptured?.Margin = new Thickness(0);
        imgCaptured?.Width = 20;
        imgCaptured?.Height = 20;
        imgCaptured?.IsHitTestVisible = false;
        string? name = imgCaptured!.Name.ToString();
        if (name[0] == 'b')
            BlackCaptures.Children.Add(imgCaptured);
        else
            WhiteCaptures.Children.Add(imgCaptured);
    }
    /// <summary>
    /// Իրականացնում է capture-ի UI թարմացումը՝
    /// հակառակ ֆիգուրը տեղափոխելով WrapPanel,
    /// իսկ սխալ քայլի դեպքում վերադարձնելով ֆիգուրը սկզբնական դիրք
    /// </summary>
    /// <param name="img">Տեղափոխվող ֆիգուրի պատկերը</param>
    /// <param name="moveInfo">Քայլի տվյալները (նպատակային դիրքը ներառյալ)</param>
    private void CaptureToWrap(Image? img, MoveInfo moveInfo)
    {
        if (moveInfo.Target is null)
            return;

        if (pieceBoard[moveInfo.Target] is not null)
        {
            bool isCaptured = false;
            var imgCaptured = img;

            // Ստուգում ենք ֆիգուրի վերջնական դիրքում ուրիշ ֆիգուրայի առկայությունը
            foreach (var item in grid_figure.Children)
            {
                var imgTarget = (Image)item;
                int rowTarget = (int)(imgTarget.Margin.Top + 28.5) / 57;
                int colTarget = (int)(imgTarget.Margin.Left + 28.5) / 57;

                if (rowTarget == moveInfo.Target.Row && colTarget == moveInfo.Target.Col &&
                imgTarget?.Name?.ToString()?[0] != img?.Name?.ToString()?[0])
                {
                    isCaptured = true;
                    imgCaptured = imgTarget;
                }
            }
            // Ուրիշ ֆիգուրայի առկայության դեպքում ջնջում ենք ֆիգուրան խաղատախտակից
            // և ավելացնում սպանված ֆիգուրների WrapPanel ում
            if (isCaptured)
            {
                if (!audioPlayed) { SoundManager.Play("capture"); audioPlayed = true; }
                AddingCaptureToWrap(imgCaptured);
            }

            // Արդեն առկա ֆիգուրի նույն գույնը ունենալու դեպքում
            // ընտրված ֆիգուրի վերադարձը իր նախնական դիրք
            else
                img?.Margin = new Thickness(_imgDownX, _imgDownY, 0, 0);
        }
    }
    private void EnPassantCaptureToWrap(Image movedImage, MoveInfo moveInfo)
    {
        if (moveInfo.Start is null || moveInfo.Target is null)
            return;

        int capturedPawnRow = moveInfo.Start.Row;
        int capturedPawnCol = moveInfo.Target.Col;

        Image? capturedImage = grid_figure.Children
            .OfType<Image>()
            .FirstOrDefault(image =>
            {
                int row = (int)(image.Margin.Top + 28.5) / _cellSize;
                int col = (int)(image.Margin.Left + 28.5) / _cellSize;

                return image != movedImage &&
                       row == capturedPawnRow &&
                       col == capturedPawnCol &&
                       image.Name[0] != movedImage.Name[0];
            });

        if (capturedImage is not null)
            AddingCaptureToWrap(capturedImage);

        movedImage.Margin = new Thickness(
            moveInfo.Target.Col * _cellSize + (_cellSize - movedImage.Width) / 2,
            moveInfo.Target.Row * _cellSize + (_cellSize - movedImage.Height) / 2,
            0,
            0);
    }
    /// <summary>
    /// Թարմացնում է UI-ը ձախ ռոկիրովկայի ժամանակ՝
    /// տեղափոխելով թագավորին դեպի նպատակային դիրք և նավին համապատասխան դիրք
    /// </summary>
    /// <param name="img">Տեղափոխվող թագավորի պատկերը</param>
    /// <param name="moveInfo">Քայլի տվյալները (նպատակային դիրքը ներառյալ)</param>
    private void LeftCastlingUI(Image img, MoveInfo moveInfo)
    {
        if (moveInfo is null) return;
        if (moveInfo.Target is null) return;

        if (!audioPlayed) { SoundManager.Play("castle"); audioPlayed = true; }
        img?.Margin = new Thickness(
                moveInfo.Target.Col * _cellSize + (_cellSize - img.Width) / 2,
                moveInfo.Target.Row * _cellSize + (_cellSize - img.Height) / 2,
                0, 0);

        foreach (var item in grid_figure.Children)
        {
            var imgTarget = (Image)item;
            int rowTarget = (int)(imgTarget.Margin.Top + 28.5) / 57;
            int colTarget = (int)(imgTarget.Margin.Left + 28.5) / 57;
            if (rowTarget == moveInfo.Target.Row && colTarget == 0)
            {
                imgTarget.Margin = new Thickness(imgTarget.Margin.Left + (_cellSize * 3),
                    imgTarget.Margin.Top,
                    0, 0);
            }
        }
        Mouse.Capture(null);
        StackPanel.SetZIndex(img, 0);
    }
    /// <summary>
    /// Թարմացնում է UI-ը աջ ռոկիրովկայի ժամանակ՝
    /// տեղափոխելով թագավորի պատկերը դեպի նպատակային դիրք
    /// և համապատասխան նավի (rook) պատկերը նոր դիրք
    /// </summary>
    /// <param name="img">Տեղափոխվող թագավորի պատկերը</param>
    /// <param name="moveInfo">Քայլի տվյալները, ներառյալ նպատակային դիրքը</param>
    private void RightCastlingUI(Image img, MoveInfo moveInfo)
    {
        if (moveInfo is null) return;
        if (moveInfo.Target is null) return;

        img?.Margin = new Thickness(
                 moveInfo.Target.Col * _cellSize + (_cellSize - img.Width) / 2,
                moveInfo.Target.Row * _cellSize + (_cellSize - img.Height) / 2,
                0, 0);
        if (!audioPlayed) { SoundManager.Play("castle"); audioPlayed = true; }

        foreach (var item in grid_figure.Children)
        {
            var imgTarget = (Image)item;
            int rowTarget = (int)(imgTarget.Margin.Top + 28.5) / 57;
            int colTarget = (int)(imgTarget.Margin.Left + 28.5) / 57;
            if (rowTarget == moveInfo.Target.Row && colTarget == 7)
            {
                imgTarget.Margin = new Thickness(imgTarget.Margin.Left - (_cellSize * 2),
                    imgTarget.Margin.Top,
                    0, 0);
            }
        }
        Mouse.Capture(null);
        StackPanel.SetZIndex(img, 0);
    }
    /// <summary>
    /// Կառավարում է խաղատախտակի UI-ի թարմացումը՝
    /// ըստ քայլի տեսակի՝ կատարելով ֆիգուրի տեղաշարժ,
    /// capture-ի մշակումը և հատուկ քայլերի (ռոկիրովկա, promotion) արտացոլումը
    /// </summary>
    /// /// <param name="img">Տեղափոխվող ֆիգուրի պատկերը</param>
    /// <param name="moveInfo">Քայլի տվյալները (սկիզբ և նպատակային դիրք)</param>
    /// <param name="moveType">Քայլի տեսակը (MoveType)</param>
    private void MoveUIUpdate(Image img, MoveInfo moveInfo, MoveType moveType)
    {
        if (img == null) return;
        if (moveInfo is null) return;
        if (moveInfo.Target is null) return;
        MoveShower.Content = acctiveTurn.ToString();
        switch (moveType)
        {
            case MoveType.InvalidMove:
                if (!audioPlayed) { SoundManager.Play("invalidMove"); audioPlayed = true; }
                img.Margin = new Thickness(_imgDownX, _imgDownY, 0, 0);
                break;
            case MoveType.RegularMove:
                CaptureToWrap(img, moveInfo);
                if (!audioPlayed) { SoundManager.Play("move"); audioPlayed = true; }
                img?.Margin = new Thickness(
            moveInfo.Target.Col * _cellSize + (_cellSize - img.Width) / 2,
            moveInfo.Target.Row * _cellSize + (_cellSize - img.Height) / 2,
            0, 0);
                break;
            case MoveType.LeftCastling:
                LeftCastlingUI(img, moveInfo);
                break;
            case MoveType.RightCastling:
                RightCastlingUI(img, moveInfo);
                break;
            case MoveType.PawnPromotion:
                CaptureToWrap(img, moveInfo);
                if (!audioPlayed) { SoundManager.Play("promotion"); audioPlayed = true; }
                img?.Margin = new Thickness(
                moveInfo.Target.Col * _cellSize + (_cellSize - img.Width) / 2,
                moveInfo.Target.Row * _cellSize + (_cellSize - img.Height) / 2,
                0, 0);
                break;
            case MoveType.EnPassant:
                EnPassantCaptureToWrap(img, moveInfo);
                break;
        }
    }
    /// <summary>
    /// Թարմացնում է խաղի վիճակի UI արտացոլումը՝ ըստ MoveResult-ի,
    /// ցուցադրում է համապատասխան հաղորդագրություններ (շախ, մատ)
    /// և մատի դեպքում փակում է պատուհանը
    /// </summary>
    /// <param name="moveResult">Քայլի արդյունքը, որը պարունակում է BoardState-ը</param>
    private void BoardStateUpdate(MoveResult moveResult)
    {
        switch (moveResult.BoardState)
        {
            case BoardState.CheckMate:
                if (!audioPlayed) { SoundManager.Play("checkMate"); audioPlayed = true; }
                MessageBox.Show("CHECKMATE");
                MessageBox.Show($"{acctiveTurn.ToString().ToUpper()} WIN");
                Close();
                break;
            case BoardState.Check:
                if (!audioPlayed) { SoundManager.Play("check"); audioPlayed = true; }
                MessageBox.Show("CHECK");
                break;
            case BoardState.StaleMate:
                MessageBox.Show("STALEMATE — DRAW");
                Close();
                break;
        }
    }
    private void RefreshGameUI()
    {
        WhitePromotionOverlay.Visibility = Visibility.Collapsed;
        BlackPromotionOverlay.Visibility = Visibility.Collapsed;

        boardEnteredImage = null;
        _moveInfo = null;

        MoveShower.Content = acctiveTurn.ToString();

        RenderBoard(pieceBoard);
        RenderCapturedPieces();

        UpdateHistoryButtons();
    }
    private void StartNewGame()
    {
        _game.StartNewGame();

        _moveInfo = null;
        boardEnteredImage = null;

        WhiteCaptures.Children.Clear();
        BlackCaptures.Children.Clear();

        WhitePromotionOverlay.Visibility = Visibility.Collapsed;
        BlackPromotionOverlay.Visibility = Visibility.Collapsed;

        MoveShower.Content = acctiveTurn.ToString();

        RenderBoard(pieceBoard);

        UpdateHistoryButtons();
    }
    #endregion

    #region PawnPromotion
    /// <summary>
    /// Ստուգում է pawn promotion-ի պայմանը և ցուցադրում համապատասխան ընտրության overlay-ը
    /// (սպիտակ կամ սև), պահպանելով քայլի տվյալները հետագա օգտագործման համար
    /// </summary>
    /// <param name="board">Խաղատախտակի ընթացիկ վիճակը</param>
    /// <param name="img">Ընտրված ֆիգուրի պատկերը</param>
    /// <param name="moveInfo">Քայլի սկզբնական և վերջնական դիրքերը</param>
    public void ShowPromotionOverlay(Image? img, MoveInfo moveInfo)
    {
        if (img is null || moveInfo.Start is null ||
         moveInfo.Target is null)
            return;

        string name = img.Name;
        if (name[0] == 'w')
            WhitePromotionOverlay.Visibility = Visibility.Visible;
        else
            BlackPromotionOverlay.Visibility = Visibility.Visible;
        boardEnteredImage = img;
        _moveInfo = moveInfo;
    }
    #endregion
}
