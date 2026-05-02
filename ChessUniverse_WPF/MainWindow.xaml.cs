using ChessUniverse.Library;
using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace ChessUniverse_WPF;

public partial class MainWindow : Window
{
    // Ցույց է տալիս՝ մկնիկը սեղմված է արդյոք (drag-ի վիճակ)
    private bool _t;

    //Մկնիկի սեղմման ժամանակ կորդինատների ֆիքսում
    private int _imgDownX;
    private int _imgDownY;

    //Մկնիկի թողման պահին կորդինատների ֆիքսում
    private int _imgUpX;
    private int _imgUpY;

    Image boardEnteredImage;
    Game game = new Game();
    private bool firstBoardLoc;
    private int _cellSize = 57;
    ChessBoard pieceBoard = new ChessBoard();
    PieceColor acctiveTurn;
    MoveInfo _moveInfo;
    private System.Windows.Point _ptLast = new System.Windows.Point();

    public MainWindow()
    {
        InitializeComponent();
    }

    #region EVENTS
    private void MouseDown(object sender, MouseEventArgs e)
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

        if (!firstBoardLoc)
        {
            BoardLocParsal(pieceBoard);
            firstBoardLoc = true;
        }

    }
    private void MouseMove(object sender, MouseEventArgs e)
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
    private void MouseUp(object sender, MouseEventArgs e)
    {
        _t = false;
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
        MoveUIUpdate(img, moveInfo, currentMoveType);

        Mouse.Capture(null);
        StackPanel.SetZIndex(img, 0);

        label3.Content = "M: " + img.Margin.Left.ToString() + " " + img.Margin.Top.ToString();
    }
    private void PromotionClick(object sender, EventArgs e)
    {
        var selectedImg = (Image)sender;

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
        MoveUIUpdate(boardEnteredImage, _moveInfo, MoveType.RegularMove);

        if (ChessRules.IsChecked(pieceBoard).Item1)
            MessageBox.Show("Check!");

        WhitePromotionOverlay.Visibility = Visibility.Collapsed;
        BlackPromotionOverlay.Visibility = Visibility.Collapsed;
    }
    #endregion

    #region MOVEUI
    private void AddingCaptureToWrap(Image? imgCaptured)
    {
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

    private void CaptureToWrap(Image? img, MoveInfo moveInfo)
    {
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
                AddingCaptureToWrap(imgCaptured);

            // Արդեն առկա ֆիգուրի նույն գույնը ունենալու դեպքում
            // ընտրված ֆիգուրի վերադարձը իր նախնական դիրք
            else
                img?.Margin = new Thickness(_imgDownX, _imgDownY, 0, 0);
        }
    }
    private void LeftCastlingUI(Image img, MoveInfo moveInfo)
    {

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
    private void RightCastlingUI(Image img, MoveInfo moveInfo)
    {

        img?.Margin = new Thickness(
                 moveInfo.Target.Col * _cellSize + (_cellSize - img.Width) / 2,
                moveInfo.Target.Row * _cellSize + (_cellSize - img.Height) / 2,
                0, 0);

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
    public void MoveUIUpdate(Image img, MoveInfo moveInfo, MoveType moveType)
    {
        if (img == null) return;
        if (moveInfo is null) return;
        if (moveInfo.Target is null) return;

        switch (moveType)
        {
            case MoveType.InvalidMove:
                img.Margin = new Thickness(_imgDownX, _imgDownY, 0, 0);
                break;
            case MoveType.RegularMove:
                CaptureToWrap(img, moveInfo);
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
                img?.Margin = new Thickness(
                moveInfo.Target.Col * _cellSize + (_cellSize - img.Width) / 2,
                moveInfo.Target.Row * _cellSize + (_cellSize - img.Height) / 2,
                0, 0);
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
        bool samePosition = moveInfo.Target?.Row == moveInfo.Start?.Row && moveInfo?.Target?.Col == moveInfo?.Start?.Col;
        Piece? currentPiece = pieceBoard[moveInfo.Start];

        return currentPiece is not null && !samePosition &&
            currentPiece!.IsMovePossible(pieceBoard, moveInfo.Target);
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
        MoveType currentMoveType;
        if (acctiveTurn != board[moveInfo.Start]?.Color)
            return new MoveResult(board, MoveType.InvalidMove);
        //1
        bool checkStartState = false;
        bool checkTargetState = false;
        PieceColor passiveTurn;
        if (acctiveTurn == PieceColor.White)
            passiveTurn = PieceColor.Black;
        else
            passiveTurn = PieceColor.White;

        //3
        PiecePosition? acctiveKing = ChessBoard.GetKingPosition(board, acctiveTurn);
        checkStartState = ChessRules.IsChecked(board, acctiveKing, acctiveTurn);
        //4
        ChessBoard cloneBoard = (ChessBoard)board.Clone();
        //5
        if (!IsMovePossible(board, moveInfo))
            return new MoveResult(board, MoveType.InvalidMove);

        //6
        if (IsPawnPromotion(cloneBoard, moveInfo))
        { ShowPromotionOverlay(boardEnteredImage, moveInfo); currentMoveType = MoveType.PawnPromotion; }
        else if (ChessRules.IsCastlingLeftPossible(cloneBoard, moveInfo))
        { cloneBoard = Game.CastlingLeft(cloneBoard, moveInfo).Item1; currentMoveType = MoveType.LeftCastling; }
        else if (ChessRules.IsCastlingRightPossible(cloneBoard, moveInfo))
        { cloneBoard = Game.CastlingRight(cloneBoard, moveInfo).Item1; currentMoveType = MoveType.RightCastling; }
        else { game.RegularMove(cloneBoard, moveInfo); currentMoveType = MoveType.RegularMove; }

        acctiveKing = ChessBoard.GetKingPosition(cloneBoard, acctiveTurn);
        checkTargetState = ChessRules.IsChecked(cloneBoard, acctiveKing, acctiveTurn);

        //8
        if (checkStartState && checkTargetState)
        {
            MessageBox.Show("Invalid Move: You are in check!");
            return new MoveResult(board, MoveType.InvalidMove);
        }
        else if (!checkStartState && checkTargetState)
        {
            MessageBox.Show("Invalid Move: Check way was opened!");
            return new MoveResult(board, MoveType.InvalidMove);
        }
        PiecePosition? passiveKing = ChessBoard.GetKingPosition(cloneBoard, passiveTurn);
        //10
        if (ChessRules.IsChecked(cloneBoard, passiveKing, passiveTurn))
            MessageBox.Show("Check!");
        acctiveTurn = MoveChanger(acctiveTurn);
        return new MoveResult(cloneBoard, currentMoveType);
    }
    /// <summary>
    /// Փոխում է հերթը՝ վերադարձնելով հակառակ գույնի խաղացողին
    /// </summary>
    /// <param name="acctiveTurn">Ներկայիս խաղացողի գույնը</param>
    /// <returns>
    /// Հակառակ գույնը (եթե White է՝ կվերադարձնի Black, և հակառակը)
    /// </returns>
    PieceColor MoveChanger(PieceColor acctiveTurn)
    {
        if (acctiveTurn == PieceColor.White)
            return PieceColor.Black;
        return PieceColor.White;
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
    public void ShowPromotionOverlay(Image img, MoveInfo moveInfo)
    {
        if (moveInfo.Target is null) return;
        if (moveInfo.Start is null) return;

        string name = img.Name.ToString();
        if (name[0] == 'w')
            WhitePromotionOverlay.Visibility = Visibility.Visible;
        else
            BlackPromotionOverlay.Visibility = Visibility.Visible;
        img = boardEnteredImage;
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
    public Image PawnPromotionMove(string? tagSelectedImage)
    {
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
        return boardEnteredImage;
    }
    /// <summary>
    /// Կատարում է pawn promotion-ի լոգիկան՝ փոխարինելով pawn-ը ընտրված ֆիգուրով
    /// և թարմացնելով խաղատախտակի վիճակը
    /// </summary>
    /// <param name="board">Խաղատախտակը, որի վրա կատարվում է փոփոխությունը</param>
    /// <param name="moveInfo">Քայլի սկզբնական և վերջնական դիրքերը</param>
    /// <param name="selectedPiece">Նոր ֆիգուրը, որով փոխարինվում է pawn-ը</param>
    public void PawnPromotionMove(ChessBoard board, MoveInfo moveInfo, Piece? selectedPiece)
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
    }
    #endregion

    public void BoardLocParsal(ChessBoard boardPiece)
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
    }
}