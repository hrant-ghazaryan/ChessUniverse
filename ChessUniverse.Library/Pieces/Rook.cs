using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Rook(PieceColor color) : Piece(color, PieceType.Rook, 'r')
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
}
