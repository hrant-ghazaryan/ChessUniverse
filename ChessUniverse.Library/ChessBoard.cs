using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;

namespace ChessUniverse.Library;

public class ChessBoard : ICloneable
{
    const byte n = 8;
    private Piece?[,] _squares = new Piece[n, n];

    public Piece? this[PiecePosition position]
    {
        get
        {
            //1
            if (position.Row >= 0 && position.Row <= 7)
                return _squares[position.Row, position.Col];
            else if(position.Col >= 0 && position.Col <= 7)
                return _squares[position.Row, position.Col];
            else
                return null;
        }
        set
        {
            int row = position.Row; // 1 - 8
            int col = position.Col; // A - H
            if (position.Row < 0 || position.Row > 7
             || position.Col < 0 || position.Col > 7)
                return;
            _squares[position.Row, position.Col] = value;
        }
    }
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
            char rank = coordinate[1]; // 1 - 8
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
        _squares[0, 0] = new Rook(PieceColor.Black) { Position = new PiecePosition { Row = 0, Col = 0 } };
        _squares[0, 1] = new Knight(PieceColor.Black) { Position = new PiecePosition { Row = 0, Col = 1 } };
        _squares[0, 2] = new Bishop(PieceColor.Black) { Position = new PiecePosition { Row = 0, Col = 2 } };
        _squares[0, 3] = new Queen(PieceColor.Black) { Position = new PiecePosition { Row = 0, Col = 3} };
        _squares[0, 4] = new King(PieceColor.Black) { Position = new PiecePosition { Row = 0, Col = 4} };
        _squares[0, 5] = new Bishop(PieceColor.Black) { Position = new PiecePosition { Row = 0, Col = 5 } };
        _squares[0, 6] = new Knight(PieceColor.Black) { Position = new PiecePosition { Row = 0, Col = 6 } };
        _squares[0, 7] = new Rook(PieceColor.Black) { Position = new PiecePosition { Row = 0, Col = 7 } };
        for (int i = 0; i < n; i++)
        {
            _squares[1, i] = new Pawn(PieceColor.Black) { Position = new PiecePosition { Row = 1, Col = i } };
            _squares[6, i] = new Pawn(PieceColor.White) { Position = new PiecePosition { Row = 6, Col = i } };
        }
        _squares[7, 0] = new Rook(PieceColor.White) { Position = new PiecePosition { Row = 7, Col = 0 } }; //A1
        _squares[7, 1] = new Knight(PieceColor.White) { Position = new PiecePosition { Row = 7, Col = 1 } };
        _squares[7, 2] = new Bishop(PieceColor.White) { Position = new PiecePosition { Row = 7, Col = 2 } };
        _squares[7, 3] = new Queen(PieceColor.White) { Position = new PiecePosition { Row = 7, Col = 3} };
        _squares[7, 4] = new King(PieceColor.White) { Position = new PiecePosition { Row = 7, Col = 4} };
        _squares[7, 5] = new Bishop(PieceColor.White) { Position = new PiecePosition { Row = 7, Col = 5 } };
        _squares[7, 6] = new Knight(PieceColor.White) { Position = new PiecePosition { Row = 7, Col = 6 } };
        _squares[7, 7] = new Rook(PieceColor.White) { Position = new PiecePosition { Row = 7, Col = 7} };

    }
    public static PiecePosition? GetKingPosition(ChessBoard chessBoard, PieceColor color)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var piece = chessBoard[i, j];
                if (piece?.Type == PieceType.King
                    && piece?.Color == color)
                    return piece.Position;
            }
        }
        return null;
    }
    public static List<PiecePosition> GetPiecePositions(ChessBoard board, PieceColor color)
    {
        List<PiecePosition> positions = new List<PiecePosition>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var piece = board[i, j];
                if (piece != null && piece.Color == color)
                    positions.Add(piece.Position);
            }
        }
        return positions;
    }
    public object Clone()
    {
        ChessBoard newBoard = new ChessBoard();

        for (int i = 0;i < 8; i++)
        {
            for(int j = 0;j < 8; j++)
            {
                var piece = _squares[i, j];
                if (piece != null)
                    newBoard[i, j] = (Piece)piece.Clone();
            }
        }
        return newBoard;
    }
}
