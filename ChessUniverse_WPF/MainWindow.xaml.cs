using ChessUniverse.Library;
using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;
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

    ChessBoard pieceBoard = new ChessBoard();
    private Image? boardEnteredImage;
    private PieceColor acctiveTurn;
    private MoveInfo? _moveInfo;
    private MoveInfo? _previousMove;

    Stack<MoveResult> boardPrevious = new Stack<MoveResult>();
    Stack<MoveResult> boardNext = new Stack<MoveResult>();

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

        pieceBoard.SetStartPosition();
        RenderBoard(pieceBoard);

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

        MoveResult moveDetails = MakeMove(pieceBoard, moveInfo);
        pieceBoard = moveDetails.Board;
        currentMoveType = moveDetails.MoveType;
        BoardStateUpdate(moveDetails);
        MoveUIUpdate(img, moveInfo, currentMoveType);

        if (currentMoveType != MoveType.InvalidMove &&
            currentMoveType != MoveType.PawnPromotion)
            RenderBoard(pieceBoard);

        if (currentMoveType != MoveType.InvalidMove)
        {
            _previousMove = new MoveInfo(moveInfo);

            moveDetails.Turn = acctiveTurn;
            boardPrevious.Push(moveDetails);
        }
        Mouse.Capture(null);
        StackPanel.SetZIndex(img, 0);

        label3.Content = "M: " + img.Margin.Left.ToString() + " " + img.Margin.Top.ToString();
    }
    private void PromotionClick(object sender, EventArgs e)
    {
        if (sender is not Image selectedImg || _moveInfo is null
            || boardEnteredImage is null)
            return;

        Image promotionImage = boardEnteredImage;
        MoveInfo promotionMove = _moveInfo;

        string? tag = selectedImg.Tag.ToString();
        string? name = selectedImg.Name.ToString();
        string color = name[0] == 'w' ? "white" : "black";

        var (file, width, height) = tag switch
        {
            "Knight" => ("horse", 50, 50),
            "Bishop" => ("elephant", 43, 50),
            "Rook" => ("ship", 45, 50),
            "Queen" => ("queen", 50, 42),
            _ => (null, 0, 0)
        };

        if (file is not null)
        {
            boardEnteredImage.Source = new BitmapImage(
                new Uri($"/images/figures/{color}-{file}.png", UriKind.Relative));

            boardEnteredImage.Width = width;
            boardEnteredImage.Height = height;
        }
        /*switch (tag, name[0])
        {
            case ("Knight", 'w'):
                boardEnteredImage.Source = new BitmapImage(
                    new Uri($"/images/figures/white-horse.png", UriKind.Relative));
                boardEnteredImage.Width = 50;
                boardEnteredImage.Height = 50;
                break;
            case ("Bishop", 'w'):
                boardEnteredImage.Source = new BitmapImage(
                    new Uri($"/images/figures/white-elephant.png", UriKind.Relative));
                boardEnteredImage.Width = 43;
                boardEnteredImage.Height = 50;
                break;
            case ("Rook", 'w'):
                boardEnteredImage.Source = new BitmapImage(
                    new Uri($"/images/figures/white-ship.png", UriKind.Relative));
                boardEnteredImage.Width = 45;
                boardEnteredImage.Height = 50;
                break;
            case ("Queen", 'w'):
                boardEnteredImage.Source = new BitmapImage(
                    new Uri($"/images/figures/white-queen.png", UriKind.Relative));
                boardEnteredImage.Width = 50;
                boardEnteredImage.Height = 42;
                break;
            case ("Knight", 'b'):
                boardEnteredImage.Source = new BitmapImage(
                    new Uri($"/images/figures/black-horse.png", UriKind.Relative));
                boardEnteredImage.Width = 50;
                boardEnteredImage.Height = 50;
                break;
            case ("Bishop", 'b'):
                boardEnteredImage.Source = new BitmapImage(
                    new Uri($"/images/figures/black-elephant.png", UriKind.Relative));
                boardEnteredImage.Width = 43;
                boardEnteredImage.Height = 50;
                break;
            case ("Rook", 'b'):
                boardEnteredImage.Source = new BitmapImage(
                    new Uri($"/images/figures/black-ship.png", UriKind.Relative));
                boardEnteredImage.Width = 45;
                boardEnteredImage.Height = 50;
                break;
            case ("Queen", 'b'):
                boardEnteredImage.Source = new BitmapImage(
                    new Uri($"/images/figures/black-queen.png", UriKind.Relative));
                boardEnteredImage.Width = 50;
                boardEnteredImage.Height = 42;
                break;
        }*/
        PawnPromotionMove(tag);
        MoveUIUpdate(promotionImage, promotionMove, MoveType.RegularMove);
        RenderBoard(pieceBoard);

        if (ChessRules.IsChecked(pieceBoard))
            MessageBox.Show("Check!");

        WhitePromotionOverlay.Visibility = Visibility.Collapsed;
        BlackPromotionOverlay.Visibility = Visibility.Collapsed;
    }
    private void PreviousClick(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("IN PROCESS");
        /*if (boardPrevious.Count == 0) return;
        currentMove = boardPrevious.Pop();
        MoveResult temp = boardPrevious.Pop();
        boardNext.Push(temp);
        pieceBoard = temp.Board;
        acctiveTurn = temp.Turn;*/
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

    #endregion

    #region MOVE_LOGIC
    /// <summary>
    ///  Ստուգում է արդյոք քայլը վավեր է տվյալ ֆիգուրի համար
    /// </summary>
    /// <param name="pieceBoard">Խաղատախտակի ընթացիկ վիճակը</param>
    /// <param name="moveInfo">Քայլի սկզբնական և վերջնական դիրքերը</param>
    /// <returns> true, եթե քայլը թույլատրելի է, հակառակ դեպքում false</returns>
    private bool IsMovePossible(ChessBoard pieceBoard, MoveInfo moveInfo)
    {
        if (moveInfo is null) return false;
        if (moveInfo.Start is null) return false;
        if (moveInfo.Target is null) return false;

        bool samePosition = moveInfo.Target.Row == moveInfo.Start.Row && moveInfo?.Target.Col == moveInfo!.Start.Col;
        Piece? currentPiece = pieceBoard[moveInfo.Start];

        return currentPiece is not null && !samePosition &&
            currentPiece!.CanMove(pieceBoard, moveInfo.Target);
    }
    /// <summary>
    /// Փորձում է կատարել տրված քայլը՝ վավերացնելով այն և վերադարձնելով արդյունքը
    /// (առանց UI ազդեցության)
    /// </summary>
    /// <param name="board">Խաղատախտակի ընթացիկ վիճակը</param>
    /// <param name="moveInfo">Քայլի սկզբնական և վերջնական դիրքերը</param>
    /// <returns>
    /// MoveResult, որը պարունակում է նոր խաղատախտակը և քայլի տեսակը
    /// (RegularMove, Castling, PawnPromotion կամ InvalidMove)
    /// </returns>
    public MoveResult MakeMove(ChessBoard board, MoveInfo moveInfo)
    {
        if (moveInfo is null) return new MoveResult(board, MoveType.InvalidMove);
        if (moveInfo.Start is null) return new MoveResult(board, MoveType.InvalidMove);
        if (moveInfo.Target is null) return new MoveResult(board, MoveType.InvalidMove);

        MoveType currentMoveType;
        if (acctiveTurn != board[moveInfo.Start]?.Color)
            return new MoveResult(board, MoveType.InvalidMove);

        bool checkStartState = false;
        bool checkTargetState = false;

        PieceColor passiveTurn;
        if (acctiveTurn == PieceColor.White)
            passiveTurn = PieceColor.Black;
        else
            passiveTurn = PieceColor.White;

        PiecePosition? acctiveKing = ChessBoard.GetKingPosition(board, acctiveTurn);
        checkStartState = ChessRules.IsChecked(board, acctiveKing, acctiveTurn);

        ChessBoard cloneBoard = (ChessBoard)board.Clone();

        if (_previousMove is not null &&
            ChessRules.TryEnPassant(cloneBoard, moveInfo, _previousMove))
        {
            currentMoveType = MoveType.EnPassant;
        }
        else
        {
            if (!IsMovePossible(board, moveInfo))
                return new MoveResult(board, MoveType.InvalidMove);

            if (IsPawnPromotion(cloneBoard, moveInfo))
            {
                ShowPromotionOverlay(boardEnteredImage, moveInfo);
                currentMoveType = MoveType.PawnPromotion;
            }
            else if (CastlingRules.IsCastlingLeftPossible(cloneBoard, moveInfo))
            {
                cloneBoard = Game.Castling(cloneBoard, moveInfo);
                currentMoveType = MoveType.LeftCastling;
            }
            else if (CastlingRules.IsCastlingRightPossible(cloneBoard, moveInfo))
            {
                cloneBoard = Game.Castling(cloneBoard, moveInfo);
                currentMoveType = MoveType.RightCastling;
            }
            else
            {
                Game.RegularMove(cloneBoard, moveInfo);
                currentMoveType = MoveType.RegularMove;
            }
        }

        acctiveKing = ChessBoard.GetKingPosition(cloneBoard, acctiveTurn);
        checkTargetState = ChessRules.IsChecked(cloneBoard, acctiveKing, acctiveTurn);

        if (checkStartState && checkTargetState)
            return new MoveResult(board, MoveType.InvalidMove, BoardState.InvalidMove);
        else if (!checkStartState && checkTargetState)
            return new MoveResult(board, MoveType.InvalidMove, BoardState.InvalidMove);

        PiecePosition? passiveKing = ChessBoard.GetKingPosition(cloneBoard, passiveTurn);

        if (ChessRules.IsChecked(cloneBoard, passiveKing, passiveTurn))
        {
            if (ChessRules.IsCheckmate(cloneBoard, passiveTurn))
                return new MoveResult(cloneBoard, currentMoveType, BoardState.CheckMate);
            acctiveTurn = MoveChanger(acctiveTurn);
            return new MoveResult(cloneBoard, currentMoveType, BoardState.Check);
        }

        if (ChessRules.IsStaleMate(cloneBoard, passiveTurn))
            return new MoveResult(cloneBoard, currentMoveType, BoardState.StaleMate);

        acctiveTurn = MoveChanger(acctiveTurn);
        return new MoveResult(cloneBoard, currentMoveType, BoardState.Ongoing);
    }
    /// <summary>
    /// Փոխում է հերթը՝ վերադարձնելով հակառակ գույնի խաղացողին
    /// </summary>
    /// <param name="acctiveTurn">Ներկայիս խաղացողի գույնը</param>
    /// <returns>
    /// Հակառակ գույնը (եթե White է՝ կվերադարձնի Black, և հակառակը)
    /// </returns>
    /// <summary>
    /// Ստուգում է արդյոք տրված գույնի խաղացողը գտնվում է մատի (checkmate) մեջ՝
    /// փորձելով նրա բոլոր հնարավոր քայլերը և ստուգելով,
    /// արդյոք կա գոնե մեկ քայլ, որի արդյունքում թագավորը դուրս է գալիս շախից
    /// </summary>
    /// <param name="board">Խաղատախտակի ընթացիկ վիճակը</param>
    /// <param name="color">Խաղացողի գույնը, որի համար կատարվում է ստուգումը</param>
    /// <returns>
    /// true՝ եթե մատ է (ոչ մի թույլատրելի քայլ չի փրկում շախից),
    /// false՝ եթե կա գոնե մեկ անվտանգ քայլ
    /// </returns>
    PieceColor MoveChanger(PieceColor acctiveTurn)
        => acctiveTurn is PieceColor.White
        ? PieceColor.Black
        : PieceColor.White;
    public static bool IsCheckMate(ChessBoard board, PieceColor color)
    {
        var kingBoard = ChessBoard.GetKingPosition(board, color);
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var piece = board[i, j];
                if (piece is null || piece.Color != color)
                    continue;

                List<PiecePosition> moves = piece.GetPossibleMoves(board).Item1;

                foreach (var move in moves)
                {
                    var cloneBoard = (ChessBoard)board.Clone();

                    cloneBoard[move] = piece;
                    cloneBoard[move]!.Position = move;
                    cloneBoard[i, j] = null;

                    var kingAfter = ChessBoard.GetKingPosition(cloneBoard, color);

                    if (!ChessRules.IsChecked(cloneBoard, kingAfter, color))
                    {
                        cloneBoard[i, j] = piece;
                        cloneBoard[i, j]!.Position = new PiecePosition(i, j);
                        cloneBoard[move] = null;
                        cloneBoard = (ChessBoard)board.Clone();
                        return false;
                    }
                    cloneBoard[i, j] = piece;
                    cloneBoard[i, j]!.Position = new PiecePosition(i, j);
                    cloneBoard[move] = null;
                }
            }
        }

        return true;
    }
    #endregion

    #region PawnPromotion
    /// <summary>
    /// Ստուգում է արդյոք տվյալ քայլով pawn-ը հասնում է վերջին հորիզոնականին և պետք է փոխակերպվի
    /// </summary>
    /// <param name="board">Խաղատախտակի ընթացիկ վիճակը</param>
    /// <param name="moveInfo">Քայլի սկզբնական և վերջնական դիրքերը</param>
    /// <returns>
    /// true, եթե զինվորը հասնում է վերջին տողին (0 կամ 7), հակառակ դեպքում false
    /// </returns>
    public static bool IsPawnPromotion(ChessBoard board, MoveInfo moveInfo)
    {
        if (moveInfo.Start is null) return false;
        if (moveInfo.Target is null) return false;
        if (board[moveInfo.Start] is null) return false;

        Piece? piece = board[moveInfo.Start];

        return piece?.Type == PieceType.Pawn &&
            (moveInfo?.Target.Row == 7 || moveInfo?.Target.Row == 0);
    }
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
    /// <summary>
    /// Ստեղծում է նոր ֆիգուր՝ ըստ օգտատիրոջ ընտրության (Queen, Rook, Bishop, Knight)
    /// և կիրառում է pawn promotion-ը խաղատախտակի վրա
    /// </summary>
    /// <param name="tagSelectedImage">Ընտրված ֆիգուրի տեսակը (Tag-ից)</param>
    /// <returns>
    /// Թարմացված պատկերը, որը պետք է արտացոլվի UI-ում
    /// </returns>
    // My
    /*public void PawnPromotionMove(string? tagSelectedImage)
    {
        if (boardEnteredImage is null || _moveInfo is null)
            return;

        string name = boardEnteredImage.Name.ToString();
        PieceColor newColor = PieceColor.White;
        if (name[0] == 'b')
            newColor = PieceColor.Black;

        Piece? newPiece = tagSelectedImage switch
        {
            "Queen" => new Queen(newColor),
            "Rook" => new Rook(newColor),
            "Knight" => new Knight(newColor),
            "Bishop" => new Bishop(newColor),
            _ => null
        };

        PawnPromotionMove(pieceBoard, _moveInfo, newPiece);
    }*/
    public void PawnPromotionMove(string? tagSelectedImage)
    {
        if (_moveInfo is null || _moveInfo.Target is null)
            return;

        MoveInfo moveInfo = _moveInfo;
        PiecePosition target = moveInfo.Target;

        PieceType? promotionType = tagSelectedImage switch
        {
            "Queen" => PieceType.Queen,
            "Rook" => PieceType.Rook,
            "Knight" => PieceType.Knight,
            "Bishop" => PieceType.Bishop,
            _ => null
        };

        if (promotionType is null)
            return;

        // Զինվորը նախ տեղափոխվում է վերջին շարք։
        Game.RegularMove(pieceBoard, moveInfo);

        // Իսկ փոխակերպման կանոնը կատարվում է Library-ում։
        Game.PromotePawn(pieceBoard, target, promotionType.Value);
    }
    /// <summary>
    /// Կատարում է pawn promotion-ի լոգիկան՝ փոխարինելով pawn-ը ընտրված ֆիգուրով
    /// և թարմացնելով խաղատախտակի վիճակը
    /// </summary>
    /// <param name="board">Խաղատախտակը, որի վրա կատարվում է փոփոխությունը</param>
    /// <param name="moveInfo">Քայլի սկզբնական և վերջնական դիրքերը</param>
    /// <param name="selectedPiece">Նոր ֆիգուրը, որով փոխարինվում է pawn-ը</param>
    // My
    /*public void PawnPromotionMove(ChessBoard board, MoveInfo moveInfo, Piece? selectedPiece)
    {
        if (selectedPiece is null)
            return;

        if (moveInfo.Start is null || moveInfo.Target is null)
            return;

        Piece? piece = board[moveInfo.Start];
        board[moveInfo.Target] = null;
        board[moveInfo.Target] = selectedPiece;
        piece?.HasMoved = true;
        board[moveInfo.Start] = null;
        selectedPiece?.Position = moveInfo.Target;
    }*/
    #endregion

    // My
    /*public void BoardLocParsal(ChessBoard boardPiece)
    {
        var images = grid_figure.Children.OfType<Image>().ToList();
        for (int i = 0; i < images.Count; i++)
        {

            int cellSize = 57;
            int centerCol = (int)Math.Round(images[i].Margin.Left + images[i].Width / 2);
            int centerRow = (int)Math.Round(images[i].Margin.Top + images[i].Height / 2);

            int col = centerCol / cellSize;
            int row = centerRow / cellSize;

            row = Math.Clamp(row, 0, 7);
            col = Math.Clamp(col, 0, 7);

            string? imageName = images[i].Name.ToString();

            if (images[i].Tag.ToString() == "rook" && imageName[0] == 'w')
                boardPiece[row, col] = new Rook(PieceColor.White) { Position = new PiecePosition(row, col) };
            else if (images[i].Tag.ToString() == "rook" && imageName[0] == 'b')
                boardPiece[row, col] = new Rook(PieceColor.Black) { Position = new PiecePosition(row, col) };

            if (images[i].Tag.ToString() == "pawn" && imageName[0] == 'w')
                boardPiece[row, col] = new Pawn(PieceColor.White) { Position = new PiecePosition(row, col) };
            else if (images[i].Tag.ToString() == "pawn" && imageName[0] == 'b')
                boardPiece[row, col] = new Pawn(PieceColor.Black) { Position = new PiecePosition(row, col) };

            if (images[i].Tag.ToString() == "bishop" && imageName[0] == 'w')
                boardPiece[row, col] = new Bishop(PieceColor.White) { Position = new PiecePosition(row, col) };
            else if (images[i].Tag.ToString() == "bishop" && imageName[0] == 'b')
                boardPiece[row, col] = new Bishop(PieceColor.Black) { Position = new PiecePosition(row, col) };

            if (images[i].Tag.ToString() == "knight" && imageName[0] == 'w')
                boardPiece[row, col] = new Knight(PieceColor.White) { Position = new PiecePosition(row, col) };
            else if (images[i].Tag.ToString() == "knight" && imageName[0] == 'b')
                boardPiece[row, col] = new Knight(PieceColor.Black) { Position = new PiecePosition(row, col) };

            if (images[i].Tag.ToString() == "queen" && imageName[0] == 'w')
                boardPiece[row, col] = new Queen(PieceColor.White) { Position = new PiecePosition(row, col) };
            else if (images[i].Tag.ToString() == "queen" && imageName[0] == 'b')
                boardPiece[row, col] = new Queen(PieceColor.Black) { Position = new PiecePosition(row, col) };

            if (images[i].Tag.ToString() == "king" && imageName[0] == 'w')
                boardPiece[row, col] = new King(PieceColor.White) { Position = new PiecePosition(row, col) };
            else if (images[i].Tag.ToString() == "king" && imageName[0] == 'b')
                boardPiece[row, col] = new King(PieceColor.Black) { Position = new PiecePosition(row, col) };

        }
    }*/
}
