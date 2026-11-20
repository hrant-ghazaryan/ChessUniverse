using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Knight(PieceColor color) : Piece(color, PieceType.Knight, 't', new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool CanMove(ChessBoard chessBoard, PiecePosition? target)
    {
        if (target is null)
            return false;

        if (!ChessRules.IsInside(target.Row) || !ChessRules.IsInside(target.Col))
            return false;

        if (chessBoard[target]?.Color == chessBoard[Position]?.Color)
            return false;

        //Սկզբնական և վերջնական դիրքերի տողերի տարբերությունը
        int dRow = Math.Abs(Position.Row - target.Row);

        //Սկզբնական և վերջնական դիրքերի սյուների տարբերությունը
        int dCol = Math.Abs(Position.Col - target.Col);

        return (dRow == 1 && dCol == 2) || (dRow == 2 && dCol == 1);
    }
    public override Piece Clone()
        => new Knight(this.Color)
        {
            Position = new PiecePosition(this.Position.Row, this.Position.Col),
            HasMoved = this.HasMoved
        };
}
