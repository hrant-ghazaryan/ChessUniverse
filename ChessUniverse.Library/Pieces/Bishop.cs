using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Bishop(PieceColor color) : Piece(color, PieceType.Bishop, 'b')
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
}

