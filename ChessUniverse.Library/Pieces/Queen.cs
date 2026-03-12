using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Queen(PieceColor color) : Piece(color, PieceType.Queen, 'q', new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(ChessBoard chessBoard,   PiecePosition startposition, PiecePosition targetposition)
    {
        if (startposition.Row == targetposition.Row || startposition.Col == targetposition.Col)
        {
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
        if (Math.Abs(startposition.Row - targetposition.Row) == Math.Abs(startposition.Col - targetposition.Col))
        {
            List<Piece> movestotarget = new List<Piece>();
            if (startposition.Row < targetposition.Row && startposition.Col > targetposition.Col)
            {
                for (int i = 1; i < Math.Abs(startposition.Row - targetposition.Row); i++)
                {
                    for (int j = 1; j < Math.Abs(startposition.Col - targetposition.Col); j++)
                    {
                        if (i == j)
                        {
                            if (chessBoard[startposition.Row + i, startposition.Col - j] == null) { }
                            if (chessBoard[startposition.Row + i, startposition.Col - j] != null)
                                return false;
                        }
                    }
                }
            }
            if (startposition.Row < targetposition.Row && startposition.Col < targetposition.Col)
            {
                for (int i = 1; i < Math.Abs(startposition.Row - targetposition.Row); i++)
                {
                    for (int j = 1; j < Math.Abs(startposition.Col - targetposition.Col); j++)
                    {
                        if (i == j)
                        {
                            if (chessBoard[startposition.Row + i, startposition.Col + j] == null) { }
                            if (chessBoard[startposition.Row + i, startposition.Col + j] != null)
                                return false;
                        }
                    }
                }
            }
            if (startposition.Row > targetposition.Row && startposition.Col < targetposition.Col)
            {
                for (int i = 1; i < Math.Abs(startposition.Row - targetposition.Row); i++)
                {
                    for (int j = 1; j < Math.Abs(startposition.Col - targetposition.Col); j++)
                    {
                        if (i == j)
                        {
                            if (chessBoard[startposition.Row - i, startposition.Col + j] == null) { }
                            if (chessBoard[startposition.Row - i, startposition.Col + j] != null)
                                return false;
                        }
                    }
                }
            }
            if (startposition.Row > targetposition.Row && startposition.Col > targetposition.Col)
            {
                for (int i = 1; i < Math.Abs(startposition.Row - targetposition.Row); i++)
                {
                    for (int j = 1; j < Math.Abs(startposition.Col - targetposition.Col); j++)
                    {
                        if (i == j)
                        {
                            if (chessBoard[startposition.Row - i, startposition.Col - j] == null) { }
                            if (chessBoard[startposition.Row - i, startposition.Col - j] != null)
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
}
