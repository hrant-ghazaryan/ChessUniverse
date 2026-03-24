using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Knight(PieceColor color) : Piece(color, PieceType.Knight, 't', new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible( ChessBoard chessBoard, PiecePosition? target)
    {
        if (target is not null)
        {
            if (Math.Abs(Position.Row - target.Row) == 2 && Math.Abs(Position.Col - target.Col) == 1)
                return true;
            else if (Math.Abs(Position.Row - target.Row) == 1 && Math.Abs(Position.Col - target.Col) == 2)
                return true;
            return false;
        }
        else 
            return false;
    }
    public override (List<PiecePosition>,bool) GetPossibleMoves(ChessBoard board)
    {
        List<PiecePosition> possibleMoves = new List<PiecePosition>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                PiecePosition targetposition = new PiecePosition(i, j);
                if (ChessRules.MoveValidation(board, Position, targetposition, board[Position]?.Color))
                    possibleMoves.Add(targetposition);
            }
        }
        if (possibleMoves.Count > 0)
            return (possibleMoves, true);
        else
            return (possibleMoves, false);
    }
    public override Piece Clone()
    {
        return new Knight(this.Color)
        {
            Position = new PiecePosition(this.Position.Row, this.Position.Col)
        };
    }
}
