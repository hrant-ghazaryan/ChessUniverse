using ChessUniverse.Library;
using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChessUniverse_WPF;

public partial class MainWindow : Window
{
    private bool _t;
    private int _imgDownX;
    private int _imgDownY;
    private int _imgUpX;
    private int _imgUpY;
    ChessBoard boardd = new ChessBoard();

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
        
        BoardLocParsal(boardd);
    }

    private void MouseUp(object sender, MouseEventArgs e)
    {
        _t = false;
        var img = (System.Windows.Controls.Image)sender;

        int x = (int)(img.Margin.Left + 28.5) / 57;
        int y = (int)(img.Margin.Top + 28.5) / 57;

        if (!boardd[_imgDownY / 57, _imgDownX / 57].IsMovePossible(boardd, new PiecePosition(x, y)))
            img.Margin = new Thickness(_imgDownX, _imgDownY, 0, 0);

        Mouse.Capture(null);
        StackPanel.SetZIndex(img, 0);

        /*switch (img.Tag.ToString())
        {
            case "pawn":
                img.Margin = new Thickness(57 * x + 10, 57 * y + 2, 0, 0);
                break;
            case "bpawn":
                img.Margin = new Thickness(57 * x + 10, 57 * y + 2, 0, 0);
                break;
            case "rook":
                img.Margin = new Thickness(57 * x + 6, 57 * y + 4, 0, 0);
                break;
            case "knight":
                img.Margin = new Thickness(57 * x + 3, 57 * y + 3, 0, 0);
                break;
            case "bishop":
                img.Margin = new Thickness(57 * x + 7, 57 * y + 3, 0, 0);
                break;
            case "queen":
                img.Margin = new Thickness(57 * x + 4, 57 * y + 10, 0, 0);
                break;
            case "king":
                img.Margin = new Thickness(57 * x + 4, 57 * y + 7, 0, 0);
                break;
        }*/

        _imgUpX = (int)(img.Margin.Left);
        _imgUpY = (int)(img.Margin.Top);

        label3.Content = "M: " + img.Margin.Left.ToString() + " " + img.Margin.Top.ToString();

        //RookStep(img, _imgDownX, _imgDownY, _imgUpX, _imgUpY);
        Console.ReadLine();
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

    private void RookStep(Image img, int imgDownx, int imgDowny, int imgUpx, int imgUpy)
    {

        if (img.Tag.ToString() == "rook" && !(imgDownx == imgUpx || imgDowny == imgUpy))
            img.Margin = new Thickness(imgDownx, imgDowny, 0, 0);
    }

    public void BoardLocParsal(ChessBoard boardPiece)
    {
        boardPiece.SetStartPosition();
        var images = grid_figure.Children.OfType<Image>().ToList();
        for (int i = 0; i < images.Count; i++)
        {
            
            int cellSize = 57;
            int centerX = (int)Math.Round(images[i].Margin.Left + images[i].Width / 2);
            int centerY = (int)Math.Round(images[i].Margin.Top + images[i].Height / 2);
            int col = centerX / cellSize;
            int row = centerY / cellSize;

            row = Math.Clamp(row, 0, 7);
            col = Math.Clamp(col, 0, 7);

            string? imageName = images[i].Name.ToString();

            if (images[i].Tag.ToString() == "rook" && imageName[0] == 'w')
                boardPiece[row, col] = new Rook(PieceColor.White) { Position = new PiecePosition(7, 7) };
            else if (images[i].Tag.ToString() == "rook" && imageName[0] == 'b')
                boardPiece[row, col] = new Rook(PieceColor.Black);

            if (images[i].Tag.ToString() == "pawn" && imageName[0] == 'w')
                boardPiece[row, col] = new Pawn(PieceColor.White);
            else if (images[i].Tag.ToString() == "pawn" && imageName[0] == 'b')
                boardPiece[row, col] = new Pawn(PieceColor.Black);

            if (images[i].Tag.ToString() == "bishop" && imageName[0] == 'w')
                boardPiece[row, col] = new Bishop(PieceColor.White);
            else if (images[i].Tag.ToString() == "bishop" && imageName[0] == 'b')
                boardPiece[row, col] = new Bishop(PieceColor.Black);

            if (images[i].Tag.ToString() == "knight" && imageName[0] == 'w')
                boardPiece[row, col] = new Knight(PieceColor.White);
            else if (images[i].Tag.ToString() == "knight" && imageName[0] == 'b')
                boardPiece[row, col] = new Knight(PieceColor.Black);

            if (images[i].Tag.ToString() == "queen" && imageName[0] == 'w')
                boardPiece[row, col] = new Queen(PieceColor.White);
            else if (images[i].Tag.ToString() == "queen" && imageName[0] == 'b')
                boardPiece[row, col] = new Queen(PieceColor.Black);

            if (images[i].Tag.ToString() == "king" && imageName[0] == 'w')
                boardPiece[row, col] = new King(PieceColor.White);
            else if (images[i].Tag.ToString() == "king" && imageName[0] == 'b')
                boardPiece[row, col] = new King(PieceColor.Black);

        }
    }
}