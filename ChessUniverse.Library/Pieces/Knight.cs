using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Knight(PieceColor color) : Piece(color, PieceType.Knight, 't')
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(PiecePosition start, PiecePosition target)
    {
        if (Math.Abs(start.Row - target.Row) == 2 && Math.Abs(start.Col - target.Col) == 1)
            return true;
        else if (Math.Abs(start.Row - target.Row) == 1 && Math.Abs(start.Col - target.Col) == 2)
            return true;
        return false;
    }
}
