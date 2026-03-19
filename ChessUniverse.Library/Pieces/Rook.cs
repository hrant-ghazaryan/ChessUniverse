using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Rook(PieceColor color ) : Piece(color, PieceType.Rook, 'r' ,new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(ChessBoard chessBoard, PiecePosition targetposition)
    {
        //rowStep = sign(targetRow - startRow)
        //colStep = sign(targetCol - startCol)
        if (Position.Row == targetposition.Row || Position.Col == targetposition.Col)
        {
            if (Position.Row == targetposition.Row && Position.Col > targetposition.Col)
            {
                for (int j = 1; j < Math.Abs(Position.Col - targetposition.Col); j++)
                {
                    if (chessBoard[Position.Row, Position.Col - j] == null) { }
                    if (chessBoard[Position.Row, Position.Col - j] != null)
                        return false;
                }
            }
            if (Position.Row == targetposition.Row && Position.Col < targetposition.Col)
            {
                for (int j = 1; j < Math.Abs(Position.Col - targetposition.Col); j++)
                {
                    if (chessBoard[Position.Row, Position.Col + j] == null) { }
                    if (chessBoard[Position.Row, Position.Col + j] != null)
                        return false;
                }
            }
            if (Position.Row > targetposition.Row && Position.Col == targetposition.Col)
            {
                for (int j = 1; j < Math.Abs(Position.Row - targetposition.Row); j++)
                {
                    if (chessBoard[Position.Row - j, Position.Col] == null) { }
                    if (chessBoard[Position.Row - j, Position.Col] != null)
                        return false;
                }
            }
            if (Position.Row < targetposition.Row && Position.Col == targetposition.Col)
            {
                for (int j = 1; j < Math.Abs(Position.Row - targetposition.Row); j++)
                {
                    if (chessBoard[Position.Row + j, Position.Col] == null) { }
                    if (chessBoard[Position.Row + j, Position.Col] != null)
                        return false;
                }
            }
            return true;
        }
        return false;
    }
    public override List<PiecePosition> GetPossibleMoves(ChessBoard board)
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
        return possibleMoves;
    }
}
