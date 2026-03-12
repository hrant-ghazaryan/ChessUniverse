using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Rook(PieceColor color ) : Piece(color, PieceType.Rook, 'r' ,new PiecePosition())
{
    private bool _hasMoved;
    public bool HasMoved
    {
        get => _hasMoved;
        set
        {
            if (Position.Row != 0 && Position.Col != 0)
                _hasMoved = false;
            else if (Position.Row != 0 && Position.Col != 7)
                _hasMoved = false;
            else if (Position.Row != 7 && Position.Col != 7)
                _hasMoved = false;
            else if (Position.Row != 7 && Position.Col != 0)
                _hasMoved = false;
            else
                _hasMoved = true;
        }
    }
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(ChessBoard chessBoard, PiecePosition startposition, PiecePosition targetposition)
    {
        if (startposition.Row == targetposition.Row || startposition.Col == targetposition.Col)
        {
            if (startposition.Row == targetposition.Row && startposition.Col > targetposition.Col)
            {
                for (int j = 1; j < Math.Abs(startposition.Col - targetposition.Col); j++)
                {
                    if (chessBoard[startposition.Row, startposition.Col - j] == null) { }
                    if (chessBoard[startposition.Row, startposition.Col - j] != null)
                        return false;
                }
            }
            if (startposition.Row == targetposition.Row && startposition.Col < targetposition.Col)
            {
                for (int j = 1; j < Math.Abs(startposition.Col - targetposition.Col); j++)
                {
                    if (chessBoard[startposition.Row, startposition.Col + j] == null) { }
                    if (chessBoard[startposition.Row, startposition.Col + j] != null)
                        return false;
                }
            }
            if (startposition.Row > targetposition.Row && startposition.Col == targetposition.Col)
            {
                for (int j = 1; j < Math.Abs(startposition.Row - targetposition.Row); j++)
                {
                    if (chessBoard[startposition.Row - j, startposition.Col] == null) { }
                    if (chessBoard[startposition.Row - j, startposition.Col] != null)
                        return false;
                }
            }
            if (startposition.Row < targetposition.Row && startposition.Col == targetposition.Col)
            {
                for (int j = 1; j < Math.Abs(startposition.Row - targetposition.Row); j++)
                {
                    if (chessBoard[startposition.Row + j, startposition.Col] == null) { }
                    if (chessBoard[startposition.Row + j, startposition.Col] != null)
                        return false;
                }
            }
            return true;
        }
        return false;
    }
}
