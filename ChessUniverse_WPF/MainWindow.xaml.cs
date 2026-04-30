using ChessUniverse.Library;
using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

    Game game = new Game();
    private bool firstBoardLoc;
    private int _cellSize = 57;
    ChessBoard pieceBoard = new ChessBoard();
    PieceColor acctiveTurn;
    private System.Windows.Point _ptLast = new System.Windows.Point();

    public MainWindow()
    {
        InitializeComponent();
    }
    #region MOUSE
    private void MouseDown(object sender, MouseEventArgs e)
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
        // Ֆիգուրի առկայության ստուգում մկնիկով նշված վայրում(կորդինատում)

        //մկնիկի բաց թողում
        Mouse.Capture(null);
        StackPanel.SetZIndex(img, 0);

        label3.Content = "M: " + img.Margin.Left.ToString() + " " + img.Margin.Top.ToString();
    }
    #endregion

    #region UI
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
                break;
        }
    }
    #endregion

    #region LOGIC
    private bool IsMovePossible(ChessBoard pieceBoard, MoveInfo moveInfo)
    {
        bool samePosition = moveInfo.Target?.Row == moveInfo.Start?.Row && moveInfo?.Target?.Col == moveInfo?.Start?.Col;
        Piece? currentPiece = pieceBoard[moveInfo.Start];

        //Չթույլատրված քայլի դեպքում ֆիգուրի ետ վերադարձ իր նախնական դիրք
        return currentPiece is null || !samePosition ||
            currentPiece.IsMovePossible(pieceBoard, moveInfo.Target);
    }
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

        PiecePosition? acctiveKing = new PiecePosition();

        //2
        acctiveKing = ChessBoard.GetKingPosition(board, acctiveTurn);
        //3
        checkStartState = ChessRules.IsChecked(board, acctiveKing, acctiveTurn);
        //4
        ChessBoard cloneBoard = (ChessBoard)board.Clone();
        //5
        if (!IsMovePossible(board, moveInfo))
            return new MoveResult(board, MoveType.InvalidMove);

        //6
        if (ChessRules.IsCastlingLeftPossible(cloneBoard, moveInfo))
        { cloneBoard = Game.CastlingLeft(cloneBoard, moveInfo).Item1; currentMoveType = MoveType.LeftCastling; }
        else if (ChessRules.IsCastlingRightPossible(cloneBoard, moveInfo))
        { cloneBoard = Game.CastlingRight(cloneBoard, moveInfo).Item1; currentMoveType = MoveType.RightCastling; }
        else { game.RegularMove(cloneBoard, moveInfo); currentMoveType = MoveType.RegularMove; }
        /*if (IsMovePossible(cloneBoard,moveInfo))
        {
            //6
            if (ChessRules.IsCastlingLeftPossible(cloneBoard, moveInfo))
            { cloneBoard = Game.CastlingLeft(cloneBoard, moveInfo).Item1; currentMoveType = MoveType.LeftCastling; }
            else if (ChessRules.IsCastlingRightPossible(cloneBoard, moveInfo))
            { cloneBoard = Game.CastlingRight(cloneBoard, moveInfo).Item1; currentMoveType = MoveType.RightCastling; }
            else { game.RegularMove(cloneBoard, moveInfo); currentMoveType = MoveType.RegularMove; }
        }
        else
            return (board, MoveType.InvalidMove);*/
        //7
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
        PiecePosition? passiveKing = new PiecePosition();
        //9
        passiveKing = ChessBoard.GetKingPosition(cloneBoard, passiveTurn);
        //10
        if (ChessRules.IsChecked(cloneBoard, passiveKing, passiveTurn))
            MessageBox.Show("Check!");
        acctiveTurn = MoveChanger(acctiveTurn);
        return new MoveResult(cloneBoard, currentMoveType);
    }
    PieceColor MoveChanger(PieceColor acctiveTurn)
    {
        if (acctiveTurn == PieceColor.White)
            return PieceColor.Black;
        return PieceColor.White;
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