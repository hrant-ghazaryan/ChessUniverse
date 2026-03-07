using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Knight(PieceColor color) : Piece(color, PieceType.Knight, 't')
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
}
