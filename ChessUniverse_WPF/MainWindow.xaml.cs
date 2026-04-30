using ChessUniverse.Library;
using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

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
    private System.Windows.Point _ptLast = new System.Windows.Point();

    public MainWindow()
    {
        InitializeComponent();
    }

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

        // Ֆիգուրի առկայության ստուգում մկնիկով նշված վայրում(կորդինատում)
        if (!IsMovePossible(pieceBoard, img, moveInfo))
        {
            img.Margin = new Thickness(_imgDownX, _imgDownY, 0, 0);
            Mouse.Capture(null);
            StackPanel.SetZIndex(img, 0);
            return;
        }

        // Սպանված ֆիգուրայի դուրս բերումը խաղատախտակից
        CaptureToWrap(img, moveInfo);

        // Ֆիգուրի քայլ board ում
        if (ChessRules.IsCastlingLeftPossible(pieceBoard, moveInfo))
        {
            // Board Update
            pieceBoard = Game.CastlingLeft(pieceBoard, moveInfo).Item1;

            // UI Update
            LeftCastlingUI(img, moveInfo);
            return;

        }
        if (ChessRules.IsCastlingRightPossible(pieceBoard, moveInfo))
        {
            // Board Update
            pieceBoard = Game.CastlingRight(pieceBoard, moveInfo).Item1;
            RightCastlingUI(img, moveInfo);
            return;

        }

        //Ֆիգուրի քայլ Board
        game.RegularMove(pieceBoard, moveInfo);
        //Ֆիգուրի քայլ UI
        img?.Margin = new Thickness(
            col * _cellSize + (_cellSize - img.Width) / 2,
            row * _cellSize + (_cellSize - img.Height) / 2,
            0, 0);

        //մկնիկի բաց թողում
        Mouse.Capture(null);
        StackPanel.SetZIndex(img, 0);

        label3.Content = "M: " + img.Margin.Left.ToString() + " " + img.Margin.Top.ToString();
    }

    private bool IsMovePossible(ChessBoard pieceBoard, Image img, MoveInfo moveInfo)
    {
        bool samePosition = moveInfo.Target?.Row == moveInfo.Start?.Row && moveInfo?.Target?.Col == moveInfo?.Start?.Col;
        Piece? currentPiece = pieceBoard[moveInfo.Start];

        //Չթույլատրված քայլի դեպքում ֆիգուրի ետ վերադարձ իր նախնական դիրք
        if (currentPiece is null || samePosition ||
            !currentPiece.IsMovePossible(pieceBoard, moveInfo.Target))
        {
            img.Margin = new Thickness(_imgDownX, _imgDownY, 0, 0);
            Mouse.Capture(null);
            StackPanel.SetZIndex(img, 0);
            return false;
        }
        return true;
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
    

    // ՈՒՂՂՄԱՆ ԵՆԹԱԿԱ
    /*public void UIUpdate(ChessBoard pieceBoard)
    {
        grid_figure.Children.Clear();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieceBoard[i, j] is not null)
                {
                    grid_figure.Children.Add(new Image()
                    {
                        Margin = new Thickness(j * _cellSize + (_cellSize - img.Width) / 2,
                row * _cellSize + (_cellSize - img.Height) / 2,
                0, 0);), } )
                }
}
        }
    }*/

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