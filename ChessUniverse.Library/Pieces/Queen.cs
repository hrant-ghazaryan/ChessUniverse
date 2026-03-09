using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Queen(PieceColor color) : Piece(color, PieceType.Queen, 'q')
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(ref ChessBoard chessBoard, ref PiecePosition startposition, ref PiecePosition targetposition)
    {
        //if (start.Row == target.Row || start.Col == target.Col ||
        //    Math.Abs(start.Row - target.Row) == Math.Abs(start.Col - target.Col))
        //{

        //}
        //return false;
        if (startposition.Row == targetposition.Row || startposition.Col == targetposition.Col)
        {
            List<Piece> movestotarget = new List<Piece>();
            if (startposition.Row == targetposition.Row && startposition.Col > targetposition.Col)
            {
                for (int j = 1; j < Math.Abs(startposition.Col - targetposition.Col); j++)
                    movestotarget.Add(chessBoard[startposition.Row, startposition.Col - j]);
            }
            if (startposition.Row == targetposition.Row && startposition.Col < targetposition.Col)
            {
                for (int j = 1; j < Math.Abs(startposition.Col - targetposition.Col); j++)
                    movestotarget.Add(chessBoard[startposition.Row, startposition.Col + j]);
            }
            if (startposition.Row > targetposition.Row && startposition.Col == targetposition.Col)
            {
                for (int i = 1; i < Math.Abs(startposition.Row - targetposition.Row); i++)
                    movestotarget.Add(chessBoard[startposition.Row - i, startposition.Col]);
            }
            if (startposition.Row < targetposition.Row && startposition.Col == targetposition.Col)
            {
                for (int i = 1; i < Math.Abs(startposition.Row - targetposition.Row); i++)
                    movestotarget.Add(chessBoard[startposition.Row + i, startposition.Col]);
            }
            foreach (var item in movestotarget)
            {
                if (item != null)
                    return false;
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
                        movestotarget.Add(chessBoard[startposition.Row + i, targetposition.Row - j]);
                    }
                }
            }
            if (startposition.Row < targetposition.Row && startposition.Col < targetposition.Col)
            {
                for (int i = 1; i < Math.Abs(startposition.Row - targetposition.Row); i++)
                {
                    for (int j = 1; j < Math.Abs(startposition.Col - targetposition.Col); j++)
                    {
                        movestotarget.Add(chessBoard[startposition.Row + i, targetposition.Row + j]);
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
                            movestotarget.Add(chessBoard[startposition.Row - i, targetposition.Row + j]);
                    }
                }
            }
            if (startposition.Row > targetposition.Row && startposition.Col > targetposition.Col)
            {
                for (int i = 1; i < Math.Abs(startposition.Row - targetposition.Row); i++)
                {
                    for (int j = 1; j < Math.Abs(startposition.Col - targetposition.Col); j++)
                    {
                        movestotarget.Add(chessBoard[startposition.Row - i, targetposition.Row - j]);
                    }
                }
            }
            foreach (var item in movestotarget)
            {
                if (item != null)
                    return false;
            }
            return true;
        }
        return false;
    }
}
