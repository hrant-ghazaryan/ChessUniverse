using ChessUniverse.Library.Enums;
namespace ChessUniverse.Library.Pieces;

public class King(PieceColor color) : Piece(color, PieceType.King, 'k', new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(ChessBoard chessBoard, PiecePosition target)
    {
        MoveInfo moveInfo = new MoveInfo(Position, target);

        if (chessBoard[target]?.Color == chessBoard[Position]?.Color)
            return false;

        int dRow = Math.Abs(Position.Row - target.Row);
        int dCol = Math.Abs(Position.Col - target.Col);

        return (dRow <= 1 && dCol <= 1 && !(dRow == 0 && dCol == 0) &&
              ChessRules.IsInside(target.Row) && ChessRules.IsInside(target.Col))
              || ChessRules.IsCastlingLeftPossible(chessBoard, moveInfo)
              || ChessRules.IsCastlingRightPossible(chessBoard, moveInfo); 
    }
    public override (List<PiecePosition>, bool) GetPossibleMoves(ChessBoard board)
    {

        bool IsCheckedForStaleMate(ChessBoard chessBoard, PiecePosition position)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = chessBoard[i, j];
                    var pieceparam = chessBoard[position];
                    if (piece is not null)
                    {
                        if (piece.IsMovePossible(chessBoard, position))
                            return true;
                    }
                }
            }
            return false;
        }
        ChessBoard copyBoard = new ChessBoard();
        copyBoard = (ChessBoard)board.Clone();

        List<PiecePosition> possibleMoves = new List<PiecePosition>();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (i == Position.Row && j == Position.Col)
                    continue;
                PiecePosition targetposition = new PiecePosition(i, j);
                if (ChessRules.MoveValidation(board, Position, targetposition, board[Position]!.Color))
                {
                    if (!IsCheckedForStaleMate(copyBoard, targetposition))
                        possibleMoves.Add(targetposition);
                }
            }
        }
        return (possibleMoves, possibleMoves.Count > 0);
    }
    public override Piece Clone()
    {
        return new King(this.Color)
        {
            Position = new PiecePosition(this.Position.Row, this.Position.Col)
        };
    }
}
