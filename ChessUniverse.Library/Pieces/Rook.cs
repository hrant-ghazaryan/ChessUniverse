using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Rook(PieceColor color ) : Piece(color, PieceType.Rook, 'r' ,new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(ChessBoard chessBoard, PiecePosition startposition, PiecePosition targetposition)
    {
        if (startposition.Row == targetposition.Row || startposition.Col == targetposition.Col)
        {
            List<Piece> movestotarget = new List<Piece>();
            if (startposition.Row == targetposition.Row && startposition.Col > targetposition.Col)
            {
                for (int j = 1; j < Math.Abs(startposition.Col - targetposition.Col); j++)
                {
                    if (chessBoard[startposition.Row, startposition.Col - j] == null) { }
                    if (chessBoard[startposition.Row, startposition.Col - j] != null)
                        return false;
                }
            }
            if (startposition.Row == targetposition.Row && startposition.Col < targetposition.Col)
            {
                for (int j = 1; j < Math.Abs(startposition.Col - targetposition.Col); j++)
                {
                    if (chessBoard[startposition.Row, startposition.Col + j] == null) { }
                    if (chessBoard[startposition.Row, startposition.Col + j] != null)
                        return false;
                }
            }
            if (startposition.Row > targetposition.Row && startposition.Col == targetposition.Col)
            {
                for (int j = 1; j < Math.Abs(startposition.Row - targetposition.Row); j++)
                {
                    if (chessBoard[startposition.Row - j, startposition.Col] == null) { }
                    if (chessBoard[startposition.Row - j, startposition.Col] != null)
                        return false;
                }
            }
            if (startposition.Row < targetposition.Row && startposition.Col == targetposition.Col)
            {
                for (int j = 1; j < Math.Abs(startposition.Row - targetposition.Row); j++)
                {
                    if (chessBoard[startposition.Row + j, startposition.Col] == null) { }
                    if (chessBoard[startposition.Row + j, startposition.Col] != null)
                        return false;
                }
            }
            return true;
        }
        return false;
    }
}
