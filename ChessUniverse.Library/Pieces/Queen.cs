using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Queen(PieceColor color) : Piece(color, PieceType.Queen, 'q', new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool CanMove(ChessBoard chessBoard, PiecePosition targetposition)
    {
        if (chessBoard[targetposition]?.Color == chessBoard[Position]?.Color)
            return false;

        Rook rook = new Rook(Color) { Position = this.Position };
        if (rook.CanMove(chessBoard, targetposition))
            return true;

        Bishop bishop = new Bishop(Color) { Position = this.Position };
        if (bishop.CanMove(chessBoard, targetposition))
            return true;

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
        => new Queen(this.Color)
        {
            Position = new PiecePosition(this.Position.Row, this.Position.Col),
            HasMoved = this.HasMoved
        };
}
