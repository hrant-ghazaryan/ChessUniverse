using ChessUniverse.Library.Enums;
using System.Collections;

namespace ChessUniverse.Library.Pieces;

public class Bishop(PieceColor color) : Piece(color, PieceType.Bishop, 'b')
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public  override bool IsMovePossible(ref ChessBoard chessBoard, ref PiecePosition startposition,ref PiecePosition targetposition)
    {
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

