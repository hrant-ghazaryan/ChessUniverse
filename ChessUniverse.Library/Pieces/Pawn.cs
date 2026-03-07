using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Pawn(PieceColor color) : Piece(color, PieceType.Pawn, 'p')
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
}
