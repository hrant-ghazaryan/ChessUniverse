using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Knight(PieceColor color) : Piece(color, PieceType.Knight, 't', new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(ChessBoard chessBoard, PiecePosition? target)
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
    public override (List<PiecePosition>, bool) GetPossibleMoves(ChessBoard board)
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
        return (possibleMoves, possibleMoves.Count > 0);
    }
    public override Piece Clone()
    {
        return new Knight(this.Color)
        {
            Position = new PiecePosition(this.Position.Row, this.Position.Col)
        };
    }
}
