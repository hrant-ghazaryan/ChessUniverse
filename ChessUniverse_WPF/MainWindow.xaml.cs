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

    private readonly ChessGame _game = new();

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

        int capturedPiecesBeforeMove = _game.CapturedPieces.Count;
        MoveResult moveDetails = _game.TryMove(moveInfo);
        currentMoveType = moveDetails.MoveType;
        bool wasCapture = _game.CapturedPieces.Count > capturedPiecesBeforeMove;

        if (currentMoveType == MoveType.PawnPromotion)
        {

            Piece? promotionPawn = _game.Board[moveInfo.Target!];

            if (promotionPawn is not null)
                ShowPromotionOverlay(promotionPawn.Color);

            MoveUIUpdate(img, currentMoveType, wasCapture);
        }
        else
        {
            BoardStateUpdate(moveDetails);
            MoveUIUpdate(img, currentMoveType, wasCapture);
            if (currentMoveType != MoveType.InvalidMove)
            {
                UpdateHistoryButtons();

                RenderBoard(_game.Board);
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

        WhitePromotionOverlay.Visibility = Visibility.Collapsed;
        BlackPromotionOverlay.Visibility = Visibility.Collapsed;

        RenderBoard(_game.Board);
        RenderCapturedPieces();

        MoveShower.Content = _game.ActiveTurn.ToString();

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
    private void MoveUIUpdate(
    Image image,
    MoveType moveType,
    bool wasCapture)
    {
        MoveShower.Content = _game.ActiveTurn.ToString();

        if (moveType == MoveType.InvalidMove)
        {
            if (!audioPlayed)
            {
                SoundManager.Play("invalidMove");
                audioPlayed = true;
            }

            image.Margin = new Thickness(_imgDownX, _imgDownY, 0, 0);
            return;
        }

        string soundName = moveType switch
        {
            MoveType.LeftCastling or MoveType.RightCastling => "castle",
            MoveType.PawnPromotion => "promotion",
            MoveType.EnPassant => "capture",
            _ when wasCapture => "capture",
            _ => "move"
        };

        if (!audioPlayed)
        {
            SoundManager.Play(soundName);
            audioPlayed = true;
        }
    }
    private void BoardStateUpdate(MoveResult moveResult)
    {
        switch (moveResult.BoardState)
        {
            case BoardState.CheckMate:
                if (!audioPlayed) { SoundManager.Play("checkMate"); audioPlayed = true; }
                MessageBox.Show("CHECKMATE");
                PieceColor winner = _game.ActiveTurn == PieceColor.White
                    ? PieceColor.Black
                    : PieceColor.White;

                MessageBox.Show($"{winner.ToString().ToUpper()} WIN");
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

        MoveShower.Content = _game.ActiveTurn.ToString();

        RenderBoard(_game.Board);
        RenderCapturedPieces();

        UpdateHistoryButtons();
    }
    private void ShowPromotionOverlay(PieceColor color)
    {
        if (color == PieceColor.White)
            WhitePromotionOverlay.Visibility = Visibility.Visible;
        else
            BlackPromotionOverlay.Visibility = Visibility.Visible;
    }
    private void StartNewGame()
    {
        _game.StartNewGame();

        WhiteCaptures.Children.Clear();
        BlackCaptures.Children.Clear();

        WhitePromotionOverlay.Visibility = Visibility.Collapsed;
        BlackPromotionOverlay.Visibility = Visibility.Collapsed;

        MoveShower.Content = _game.ActiveTurn.ToString();

        RenderBoard(_game.Board);

        UpdateHistoryButtons();
    }
    #endregion
}
