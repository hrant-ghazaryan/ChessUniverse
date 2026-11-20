using ChessUniverse.Library.Enums;
namespace ChessUniverse.Library.Pieces;

public class Pawn : Piece
{
    public Pawn(PieceColor color)
        : base(color, PieceType.Pawn, 'p', new PiecePosition())
    {
    }

    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool CanMove(ChessBoard chessBoard, PiecePosition target)
    {
        if (target is null)
            return false;

        if (chessBoard[target]?.Color == chessBoard[Position]?.Color)
            return false;

        if (Color == PieceColor.White)
        {
            if (Position.Row - target.Row == 1 && Math.Abs(Position.Col - target.Col) == 1
                && chessBoard[target] != null)
                return true;
            if (Position.Row - target.Row == 1 && Position.Col == target.Col && chessBoard[target] == null)
                return true;
            else if (Position.Row == 6 &&
                    !HasMoved &&
                    Position.Row - target.Row == 2 &&
                    Position.Col == target.Col &&
                    chessBoard[5, Position.Col] is null &&
                    chessBoard[4, Position.Col] is null)
                return true;
        }
        if (Color == PieceColor.Black)
        {
            if (target.Row - Position.Row == 1 && Math.Abs(target.Col - Position.Col) == 1
                && chessBoard[target] != null)
                return true;
            if (target.Row - Position.Row == 1 && Position.Col == target.Col && chessBoard[target] == null)
                return true;
            else if (Position.Row == 1 &&
                    !HasMoved &&
                    target.Row - Position.Row == 2 &&
                    Position.Col == target.Col &&
                    chessBoard[2, Position.Col] is null &&
                    chessBoard[3, Position.Col] is null)
                return true;
        }
        return false;
    }
    public override Piece Clone()
        => new Pawn(this.Color)
        {
            Position = new PiecePosition(this.Position.Row, this.Position.Col),
            HasMoved = this.HasMoved
        };
}
