using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Queen(PieceColor color) : Piece(color, PieceType.Queen, 'q')
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(PiecePosition start, PiecePosition target)
    {
        if (start.Row == target.Row || start.Col == target.Col ||
            Math.Abs(start.Row - target.Row) == Math.Abs(start.Col - target.Col))
            return true;
        return false;
    }
}
