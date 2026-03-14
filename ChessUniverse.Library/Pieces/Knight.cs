using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Knight(PieceColor color) : Piece(color, PieceType.Knight, 't', new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible( ChessBoard chessBoard, PiecePosition target)
    {
        if (Math.Abs(Position.Row - target.Row) == 2 && Math.Abs(Position.Col - target.Col) == 1)
            return true;
        else if (Math.Abs(Position.Row - target.Row) == 1 && Math.Abs(Position.Col - target.Col) == 2)
            return true;
        return false;
    }
}
