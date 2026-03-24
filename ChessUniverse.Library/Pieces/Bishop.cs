using ChessUniverse.Library.Enums;
namespace ChessUniverse.Library.Pieces;

public class Bishop(PieceColor color) : Piece(color, PieceType.Bishop, 'b', new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public  override bool IsMovePossible(ChessBoard chessBoard, PiecePosition targetposition)
    {
        if (Math.Abs(Position.Row - targetposition.Row) == Math.Abs(Position.Col - targetposition.Col))
        {
            List<Piece> movestotarget = new List<Piece>();
            if (Position.Row < targetposition.Row && Position.Col > targetposition.Col)
            {
                for (int i = 1; i < Math.Abs(Position.Row - targetposition.Row); i++)
                {
                    for (int j = 1; j < Math.Abs(Position.Col - targetposition.Col); j++)
                    {
                        if (i == j)
                        {
                            if (chessBoard[Position.Row + i, Position.Col - j] == null) { }
                            if (chessBoard[Position.Row + i, Position.Col - j] != null)
                                return false;
                        }
                    }
                }
            }
            if (Position.Row < targetposition.Row && Position.Col < targetposition.Col)
            {
                for (int i = 1; i < Math.Abs(Position.Row - targetposition.Row); i++)
                {
                    for (int j = 1; j < Math.Abs(Position.Col - targetposition.Col); j++)
                    {
                        if (i == j)
                        {
                            if (chessBoard[Position.Row + i, Position.Col + j] == null) { }
                            if (chessBoard[Position.Row + i, Position.Col + j] != null)
                                return false;
                        }
                    }
                }
            }
            if (Position.Row > targetposition.Row && Position.Col < targetposition.Col)
            {
                for (int i = 1; i < Math.Abs(Position.Row - targetposition.Row); i++)
                {
                    for (int j = 1; j < Math.Abs(Position.Col - targetposition.Col); j++)
                    {
                        if (i == j) 
                        {
                            if (chessBoard[Position.Row - i, Position.Col + j] == null) { }
                            if (chessBoard[Position.Row - i, Position.Col + j] != null)
                                return false;
                        }
                    }
                }
            }
            if (Position.Row > targetposition.Row && Position.Col > targetposition.Col)
            {
                for (int i = 1; i < Math.Abs(Position.Row - targetposition.Row); i++)
                {
                    for (int j = 1; j < Math.Abs(Position.Col - targetposition.Col); j++)
                    {
                        if (i == j)
                        {
                            if (chessBoard[Position.Row - i, Position.Col - j] == null) { }
                            if (chessBoard[Position.Row - i, Position.Col - j] != null)
                                return false;
                        }
                    }
                }
            }
            foreach (var item in movestotarget)
            {
                if (item == null) { }
                if (item != null)
                    return false;
            }
            return true;
        }
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
                if (ChessRules.MoveValidation(board, Position, targetposition, board[Position]!.Color))
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
        return new Bishop(this.Color)
        {
            Position = new PiecePosition(this.Position.Row, this.Position.Col)
        };
    }
}

