using ChessUniverse.Library;
using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;
using System.Runtime.CompilerServices;
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

        BoardLocParsal(pieceBoard);
    }
    private void MouseUp(object sender, MouseEventArgs e)
    {
        _t = false;
        var img = (System.Windows.Controls.Image)sender;

        //Մկնիկով նշված վայրի – կորդինատի փոխակերպումը երկչափ զանգվածի տողի և սյան
        int row = (int)(img.Margin.Top + 28.5) / 57;
        int col = (int)(img.Margin.Left + 28.5) / 57;
        row = Math.Clamp(row, 0, 7);
        col = Math.Clamp(col, 0, 7);

        //Ընտրված ֆիգուրի նախնական կորդինատի փոխակերպումը երկչափ զանգվածի տողի և սյան
        int enteredPieceRow = (int)(_imgDownY + 28.5) / 57;
        int enteredPieceCol = (int)(_imgDownX + 28.5) / 57;

        //Ֆիգուրի առկայության ստուգում մկնիկով նշված վայրում(կորդինատում)
        if (pieceBoard[enteredPieceRow, enteredPieceCol] is null)
        {
            Mouse.Capture(null);
            StackPanel.SetZIndex(img, 0);
            return;
        }

        //Չթույլատրված քայլի դեպքում ֆիգուրի ետ վերադարձ իր նախնական դիրք
        if ((row != enteredPieceRow || col != enteredPieceCol) && !pieceBoard[enteredPieceRow, enteredPieceCol]!.IsMovePossible(pieceBoard, new PiecePosition(row, col)))
            img.Margin = new Thickness(_imgDownX, _imgDownY, 0, 0);

        else
        {
            foreach (var item in grid_figure.Children)
            {
                if (item is Image)
                {

                }
            }
            //Ֆիգուրի քայլ
            pieceBoard[row, col] = pieceBoard[enteredPieceRow, enteredPieceCol];
            pieceBoard?[row, col]?.Position = new PiecePosition(row, col);
            pieceBoard?[enteredPieceRow, enteredPieceCol] = null;

            //Ֆիգուրի քայլ UI
            img.Margin = new Thickness(
                col * _cellSize + (_cellSize - img.Width) / 2,
                row * _cellSize + (_cellSize - img.Height) / 2,
                0, 0);

        }

        //մկնիկի բաց թողում
        Mouse.Capture(null);
        StackPanel.SetZIndex(img, 0);

        label3.Content = "M: " + img.Margin.Left.ToString() + " " + img.Margin.Top.ToString();
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