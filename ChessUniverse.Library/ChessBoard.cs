using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;

namespace ChessUniverse.Library;

public class ChessBoard
{
    const byte n = 8;
    private Piece?[,] _squares = new Piece[n, n];

    public Piece? this[int row, int col]
    {
        get => _squares[row, col];
        set => _squares[row, col] = value;
    }
    public Piece? this[string coordinate]
    {
        get
        {
            if (coordinate.Length != 2)
                return null;
            char file = coordinate[0]; // A - H
            char rank = coordinate[1]; //1 - 8
            int col = file - 'A';
            int row = 7 - (rank - '1');

            if (row < 0 || row > 7 
                || col < 0 || col > 7 ) 
                return null;

            return _squares[row,col];
        }
    }

    public void SetStartPosition()
    {
        Array.Clear(_squares, 0, _squares.Length);
        _squares[0,0] = new Rook(PieceColor.Black);
        _squares[0, 1] = new Knight(PieceColor.Black);
        _squares[0, 2] = new Bishop(PieceColor.Black);
        _squares[0, 3] = new Queen(PieceColor.Black);
        _squares[0, 4] = new King(PieceColor.Black);
        _squares[0, 5] = new Bishop(PieceColor.Black);
        _squares[0, 6] = new Knight(PieceColor.Black);
        _squares[0, 7] = new Rook(PieceColor.Black);
        for (int i = 0; i < n; i++)
        {
            _squares[1,i] = new Pawn(PieceColor.Black);
            _squares[6, i] = new Pawn(PieceColor.White);
        }
        _squares[7, 0] = new Rook(PieceColor.White); //A1
        _squares[7, 1] = new Knight(PieceColor.White);
        _squares[7, 2] = new Bishop(PieceColor.White);
        _squares[7, 3] = new Queen(PieceColor.White);
        _squares[7, 4] = new King(PieceColor.White);
        _squares[7, 5] = new Bishop(PieceColor.White);
        _squares[7, 6] = new Knight(PieceColor.White);
        _squares[7, 7] = new Rook(PieceColor.White);

        for (int i = 0; i < 8; i++)
        {
            Console.Write($"{8 - i}  ");
            for (int j = 0; j < 8; j++)
            {
                //if (_squares[i, j] != null)
                //    Console.Write($"{_squares[i, j].Symbol}");
                //else
                //    Console.Write("." + "\t");
                bool isLightSquare = (i + j) % 2 == 0;
                Console.BackgroundColor = isLightSquare ? ConsoleColor.Gray : ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.Black;
                var piece = _squares[i, j];
                char symbol = piece?.Symbol ?? '.';

                Console.Write($" {symbol} ");
            }

            Console.ResetColor();
            Console.WriteLine();
        }
        Console.WriteLine("    a  b  c  d  e  f  g  h");
    }

}
