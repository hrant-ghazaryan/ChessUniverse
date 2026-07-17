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
        if (chessBoard[target]?.Color == chessBoard[Position]?.Color)
            return false;

        if (Color == PieceColor.White)
        {
            if (Position.Row - target.Row == 1 && Math.Abs(Position.Col - target.Col) == 1
                && chessBoard[target] != null)
                return true;
            if (Position.Row - target?.Row == 1 && Position.Col == target?.Col && chessBoard[target] == null)
                return true;
            else if (Position.Row == 6 && Position.Row - target?.Row == 2 &&  Position.Col == target?.Col 
                && chessBoard[5,Position.Col] is null && chessBoard[4, Position.Col] is null)
                return true;
        }
        if (Color == PieceColor.Black)
        {
            if (target!.Row - Position.Row == 1 && Math.Abs(target!.Col - Position.Col) == 1
                && chessBoard[target] != null)
                return true;
            if (target?.Row - Position.Row == 1 && Position.Col == target?.Col && chessBoard[target] == null)
                return true;
            else if (Position.Row == 1 && target?.Row - Position.Row == 2 && Position.Col == target?.Col
                && chessBoard[2,Position.Col] is null && chessBoard[3, Position.Col] is null)
                return true;
        }
        return false;
    }
    public override (List<PiecePosition>,bool) GetPossibleMoves(ChessBoard board)
    {
        var pawn = board[Position];
        pawn?.Position = Position;

        List<PiecePosition> possibleMoves = new List<PiecePosition>();
        ChessBoard cloneBoard = (ChessBoard)board.Clone();

        switch (Color)
        {
            case PieceColor.White:
                if (Position.Row == 6)
                {
                    if (pawn!.CanMove(cloneBoard, new PiecePosition(Position.Row - 2, Position.Col)))
                        possibleMoves.Add(new PiecePosition(Position.Row - 2, Position.Col));
                }
                if (ChessRules.IsInside(Position.Row - 1) && 
                    pawn!.CanMove(cloneBoard, new PiecePosition(Position.Row - 1, Position.Col)))
                    possibleMoves.Add(new PiecePosition(Position.Row - 1, Position.Col));

                if (ChessRules.IsInside(Position.Row - 1) && 
                    ChessRules.IsInside(Position.Col - 1) && 
                    pawn!.CanMove(cloneBoard, new PiecePosition(Position.Row - 1, Position.Col - 1)))
                    possibleMoves.Add(new PiecePosition(Position.Row - 1, Position.Col - 1));

                if (ChessRules.IsInside(Position.Row - 1) && 
                    ChessRules.IsInside(Position.Col + 1) && 
                    pawn!.CanMove(cloneBoard, new PiecePosition(Position.Row - 1, Position.Col + 1)))
                    possibleMoves.Add(new PiecePosition(Position.Row - 1, Position.Col + 1));
                break;

            case PieceColor.Black:
                if (Position.Row == 1)
                {
                    if (pawn!.CanMove(cloneBoard, new PiecePosition(Position.Row + 2, Position.Col)))
                        possibleMoves.Add(new PiecePosition(Position.Row + 2, Position.Col));
                }
                if (ChessRules.IsInside(Position.Row + 1) &&
                    pawn!.CanMove(cloneBoard, new PiecePosition(Position.Row + 1, Position.Col)))
                    possibleMoves.Add(new PiecePosition(Position.Row + 1, Position.Col));

                if (ChessRules.IsInside(Position.Row + 1) &&
                    ChessRules.IsInside(Position.Col - 1) &&
                    pawn!.CanMove(cloneBoard, new PiecePosition(Position.Row + 1, Position.Col - 1)))
                    possibleMoves.Add(new PiecePosition(Position.Row + 1, Position.Col - 1));

                if (ChessRules.IsInside(Position.Row + 1) &&
                    ChessRules.IsInside(Position.Col + 1) &&
                    pawn!.CanMove(cloneBoard, new PiecePosition(Position.Row + 1, Position.Col + 1)))
                    possibleMoves.Add(new PiecePosition(Position.Row + 1, Position.Col + 1));
                break;
        }
        return (possibleMoves, possibleMoves.Count > 0);
    }
    public override Piece Clone()
        => new Pawn(this.Color)
        {
            Position = new PiecePosition(this.Position.Row, this.Position.Col),
            HasMoved = this.HasMoved
        };
}
