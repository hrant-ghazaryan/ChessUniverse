using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Rook(PieceColor color) : Piece(color, PieceType.Rook, 'r', new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool CanMove(ChessBoard chessBoard, PiecePosition targetposition)
    {
        bool sameRow = Position.Row == targetposition.Row;
        bool sameCol = Position.Col == targetposition.Col;

        if (chessBoard[targetposition]?.Color == chessBoard[Position]?.Color)
            return false;

        if (!sameRow && !sameCol)
            return false;

        if (sameRow && Position.Col > targetposition.Col)
        {
            for (int j = 1; j < Math.Abs(Position.Col - targetposition.Col); j++)
            {
                if (chessBoard[Position.Row, Position.Col - j] != null)
                    return false;
            }
        }
        if (sameRow && Position.Col < targetposition.Col)
        {
            for (int j = 1; j < Math.Abs(Position.Col - targetposition.Col); j++)
            {
                if (chessBoard[Position.Row, Position.Col + j] != null)
                    return false;
            }
        }
        if (Position.Row > targetposition.Row && sameCol)
        {
            for (int j = 1; j < Math.Abs(Position.Row - targetposition.Row); j++)
            {
                if (chessBoard[Position.Row - j, Position.Col] != null)
                    return false;
            }
        }
        if (Position.Row < targetposition.Row && sameCol)
        {
            for (int j = 1; j < Math.Abs(Position.Row - targetposition.Row); j++)
            {
                if (chessBoard[Position.Row + j, Position.Col] != null)
                    return false;
            }
        }
        return true;
    }
    public override Piece Clone()
        => new Rook(this.Color)
        {
            Position = new PiecePosition(this.Position.Row, this.Position.Col),
            HasMoved = this.HasMoved
        };
}