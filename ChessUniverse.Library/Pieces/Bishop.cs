using ChessUniverse.Library.Enums;
namespace ChessUniverse.Library.Pieces;

public class Bishop(PieceColor color) : Piece(color, PieceType.Bishop, 'b', new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(ChessBoard chessBoard, PiecePosition targetposition)
    {
        if (chessBoard[targetposition]?.Color == chessBoard[Position]?.Color)
            return false;
        if (Math.Abs(Position.Row - targetposition.Row) != Math.Abs(Position.Col - targetposition.Col))
            return false;

        if (Position.Row < targetposition.Row && Position.Col > targetposition.Col)
        {
            for (int i = 1; i < Math.Abs(Position.Row - targetposition.Row); i++)
            {
                if (chessBoard[Position.Row + i, Position.Col - i] != null)
                    return false;
            }
        }
        if (Position.Row < targetposition.Row && Position.Col < targetposition.Col)
        {
            for (int i = 1; i < Math.Abs(Position.Row - targetposition.Row); i++)
            {
                if (chessBoard[Position.Row + i, Position.Col + i] != null)
                    return false;
            }
        }
        if (Position.Row > targetposition.Row && Position.Col < targetposition.Col)
        {
            for (int i = 1; i < Math.Abs(Position.Row - targetposition.Row); i++)
            {
                if (chessBoard[Position.Row - i, Position.Col + i] != null)
                    return false;
            }
        }
        if (Position.Row > targetposition.Row && Position.Col > targetposition.Col)
        {
            for (int i = 1; i < Math.Abs(Position.Row - targetposition.Row); i++)
            {
                if (chessBoard[Position.Row - i, Position.Col - i] != null)
                    return false;
            }
        }

        return true;
    }
    public override (List<PiecePosition>, bool) GetPossibleMoves(ChessBoard board)
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

    public override bool CanMove(ChessBoard chessBoard, PiecePosition targetposition)
    {
        if (targetposition is null)
            return false;
        if (Math.Abs(Position.Row - targetposition.Row) != Math.Abs(Position.Col - targetposition.Col))
            return false;

        if (Position.Row < targetposition.Row && Position.Col > targetposition.Col)
        {
            for (int i = 1; i < Math.Abs(Position.Row - targetposition.Row); i++)
            {
                if (chessBoard[Position.Row + i, Position.Col - i] != null)
                    return false;
            }
        }
        if (Position.Row < targetposition.Row && Position.Col < targetposition.Col)
        {
            for (int i = 1; i < Math.Abs(Position.Row - targetposition.Row); i++)
            {
                if (chessBoard[Position.Row + i, Position.Col + i] != null)
                    return false;
            }
        }
        if (Position.Row > targetposition.Row && Position.Col < targetposition.Col)
        {
            for (int i = 1; i < Math.Abs(Position.Row - targetposition.Row); i++)
            {
                if (chessBoard[Position.Row - i, Position.Col + i] != null)
                    return false;
            }
        }
        if (Position.Row > targetposition.Row && Position.Col > targetposition.Col)
        {
            for (int i = 1; i < Math.Abs(Position.Row - targetposition.Row); i++)
            {
                if (chessBoard[Position.Row - i, Position.Col - i] != null)
                    return false;
            }
        }

        return true;
    }
}

