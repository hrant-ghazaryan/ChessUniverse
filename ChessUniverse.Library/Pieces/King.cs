using ChessUniverse.Library.Enums;
namespace ChessUniverse.Library.Pieces;

public class King(PieceColor color) : Piece(color, PieceType.King, 'k', new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool CanMove(ChessBoard chessBoard, PiecePosition target)
    {
        MoveInfo moveInfo = new MoveInfo(Position, target);

        if (chessBoard[target]?.Color == chessBoard[Position]?.Color)
            return false;

        int dRow = Math.Abs(Position.Row - target.Row);
        int dCol = Math.Abs(Position.Col - target.Col);

        return (dRow <= 1 && dCol <= 1 && !(dRow == 0 && dCol == 0) &&
              ChessRules.IsInside(target.Row) && ChessRules.IsInside(target.Col))
              || CastlingRules.IsCastlingLeftPossible(chessBoard, moveInfo)
              || CastlingRules.IsCastlingRightPossible(chessBoard, moveInfo);
    }
    public override Piece Clone()
        => new King(this.Color)
        {
            Position = new PiecePosition(this.Position.Row, this.Position.Col),
            HasMoved = this.HasMoved
        };
}
