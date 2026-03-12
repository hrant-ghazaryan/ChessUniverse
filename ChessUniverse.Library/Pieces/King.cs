using ChessUniverse.Library.Enums;
namespace ChessUniverse.Library.Pieces;

public class King(PieceColor color) : Piece(color, PieceType.King, 'k', new PiecePosition())
{
    private bool _hasMoved;
    public bool HasMoved
    {
        get => _hasMoved;
        set
        {
            if (Position.Row != 0 && Position.Col != 4)
                _hasMoved = false;
            else if (Position.Row != 7 && Position.Col != 4)
                _hasMoved = false;
            else
                _hasMoved = true;
        }
    }
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(ChessBoard chessBoard, PiecePosition start, PiecePosition target)
    {
        if (Math.Abs(start.Row - target.Row) == 1 && Math.Abs(start.Col - target.Col) == 0)
            return true;
        else if (Math.Abs(start.Row - target.Row) == 0 && Math.Abs(start.Col - target.Col) == 1)
            return true;
        else if (Math.Abs(start.Row - target.Row) + Math.Abs(start.Col - target.Col) == 2)
            return true;
        if (chessBoard[start.Row, start.Col]?.Type == PieceType.King && Math.Abs(target.Col - start.Col) == 2 
            && chessBoard[target.Row,target.Col + 1]?.Type == PieceType.Rook 
            && chessBoard[target.Row, target.Col + 1]?.HasMoved == false)
        {
            if (chessBoard[start.Row, start.Col]?.HasMoved == false
                && chessBoard[target.Row, target.Col + 1]?.HasMoved == false)
            {
                return true;
            }
        }
        if (chessBoard[start.Row, start.Col]?.Type == PieceType.King && Math.Abs(target.Col - start.Col) == 2
            && chessBoard[target.Row, target.Col - 2]?.Type == PieceType.Rook
            && chessBoard[target.Row, target.Col - 2]?.HasMoved == false)
        {
            if (chessBoard[start.Row, start.Col]?.HasMoved == false
                && chessBoard[target.Row, target.Col - 2]?.HasMoved == false)
            {
                return true;
            }
        }
        return false;
    }
}