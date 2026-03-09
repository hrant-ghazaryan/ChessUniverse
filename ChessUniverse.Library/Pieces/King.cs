using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class King(PieceColor color) : Piece(color, PieceType.King, 'k')
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(ref ChessBoard chessBoard, ref PiecePosition start, ref PiecePosition target)
    {
        if (Math.Abs(start.Row - target.Row) == 1 || Math.Abs(start.Col - target.Col) == 1 ||
            Math.Abs(start.Row - target.Row) + Math.Abs(start.Col - target.Col) == 2)
            return true;
        return false;
    }
}