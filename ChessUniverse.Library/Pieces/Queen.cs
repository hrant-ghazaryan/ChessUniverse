using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Queen(PieceColor color) : Piece(color, PieceType.Queen, 'q')
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
}
