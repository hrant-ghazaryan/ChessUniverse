using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Rook(PieceColor color) : Piece(color, PieceType.Rook, 'r')
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(PiecePosition start, PiecePosition target)
    {
        if (start.Row == target.Row || start.Col == target.Col)
            return true;
        return false;
    }
}
