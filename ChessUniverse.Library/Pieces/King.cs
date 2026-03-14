using ChessUniverse.Library.Enums;
using System.IO.Pipelines;
namespace ChessUniverse.Library.Pieces;

public class King(PieceColor color) : Piece(color, PieceType.King, 'k', new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(ChessBoard chessBoard, PiecePosition target)
    {
        if (Position.Col - target.Col == 2 && chessBoard[target.Row, target.Col - 2]?.Type == PieceType.Rook)
        {
            if (chessBoard[Position.Row, Position.Col]?.HasMoved == false
                && chessBoard[target.Row, target.Col - 2]?.HasMoved == false)
            {
                if (chessBoard[target.Row, target.Col] == null
                    && chessBoard[target.Row, target.Col + 1] == null)
                {
                    PiecePosition l = Position;
                    chessBoard[l] = chessBoard[Position];
                    if (ChessRules.IsChecked(chessBoard, l) == false) { }
                    else
                        Console.WriteLine("THIS POSITION IS UNDER CHECK");

                    PiecePosition l1 = new PiecePosition { Row = target.Row, Col = target.Col - 1 };
                    chessBoard[l1] = chessBoard[Position];
                    if (ChessRules.IsChecked(chessBoard, l1) == false)
                        chessBoard[l1] = null;
                    else
                        Console.WriteLine("THIS POSITION IS UNDER CHECK");

                    PiecePosition l2 = target;
                    chessBoard[l2] = chessBoard[Position];
                    if (ChessRules.IsChecked(chessBoard, l2) == false)
                        chessBoard[l2] = null;
                    else
                        Console.WriteLine("THIS POSITION IS UNDER CHECK");

                    return true;
                }
            }
        }
        else if (target.Col - Position.Col == 2 && chessBoard[target.Row, target.Col + 1]?.Type == PieceType.Rook)
        {
            if (chessBoard[Position.Row, Position.Col]?.HasMoved == false
                && chessBoard[target.Row, target.Col + 1]?.HasMoved == false)
            {
                if (chessBoard[target.Row, target.Col] == null
                    && chessBoard[target.Row, target.Col - 1] == null)
                {

                    PiecePosition r = Position;
                    chessBoard[r] = chessBoard[Position];
                    if (ChessRules.IsChecked(chessBoard, r) == false) { }
                    else
                        Console.WriteLine("THIS POSITION IS UNDER CHECK");

                    PiecePosition r1 = new PiecePosition { Row = target.Row, Col = target.Col - 1 };
                    chessBoard[r1] = chessBoard[Position];
                    if (ChessRules.IsChecked(chessBoard, r1) == false)
                        chessBoard[r1] = null;
                    else
                        Console.WriteLine("THIS POSITION IS UNDER CHECK");

                    PiecePosition r2 = target;
                    chessBoard[r2] = chessBoard[Position];
                    if (ChessRules.IsChecked(chessBoard, r2) == false)
                        chessBoard[r2] = null;
                    else
                        Console.WriteLine("THIS POSITION IS UNDER CHECK");

                    return true;
                
                }
            }
        }

        if (Math.Abs(Position.Row - target!.Row) == 1 && Math.Abs(Position.Col - target.Col) == 0)
            return true;
        else if (Math.Abs(Position.Row - target.Row) == 0 && Math.Abs(Position.Col - target.Col) == 1)
            return true;
        else if (Math.Abs(Position.Row - target.Row) + Math.Abs(Position.Col - target.Col) == 2)
            return true;

        return false;
    }
}
