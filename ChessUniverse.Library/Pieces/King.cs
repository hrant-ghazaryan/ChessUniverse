using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class King(PieceColor color) : Piece(color, PieceType.King, 'k')
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
}