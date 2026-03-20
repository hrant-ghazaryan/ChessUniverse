using ChessUniverse.Library.Enums;
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
                    if (ChessRules.IsChecked(chessBoard).Item1)
                    {
                        Console.WriteLine("YOU ARE IN CHECK");
                        return false;
                    }

                    PiecePosition? l1 = new PiecePosition { Row = target.Row, Col = target.Col - 1 };
                    chessBoard[l1] = chessBoard[Position];
                    if (ChessRules.IsChecked(chessBoard, l1))
                    {
                        chessBoard[l1] = null;
                        l1 = null;
                        Console.WriteLine("THIS POSITION IS UNDER CHECK");
                        return false;
                    }
                    else
                    {
                        chessBoard[l1] = null;
                        l1 = null;
                    }

                    PiecePosition? l2 = target;
                    chessBoard[l2] = chessBoard[Position];
                    if (ChessRules.IsChecked(chessBoard, l2))
                    {
                        chessBoard[l2] = null;
                        l2 = null;
                        Console.WriteLine("THIS POSITION IS UNDER CHECK");
                        return false;
                    }
                    else
                    {
                        chessBoard[l2] = null;
                        l2 = null;
                    }

                    return true;
                }
            }
        }
        else if (target.Col - Position.Col == 2 && target.Col + 1 < 8 && chessBoard[target.Row, target.Col + 1]?.Type == PieceType.Rook)
        {
            if (chessBoard[Position.Row, Position.Col]?.HasMoved == false
                && chessBoard[target.Row, target.Col + 1]?.HasMoved == false)
            {
                if (chessBoard[target.Row, target.Col] == null
                    && chessBoard[target.Row, target.Col - 1] == null)
                {
                    if (ChessRules.IsChecked(chessBoard).Item1)
                    {
                        Console.WriteLine("YOU ARE IN CHECK");
                        return false;
                        //PiecePosition r = Position;
                        //chessBoard[r] = chessBoard[Position];
                        //if (ChessRules.IsChecked(chessBoard, r) == false) { }
                    }

                    PiecePosition? r1 = new PiecePosition { Row = target.Row, Col = target.Col - 1 };
                    chessBoard[r1] = chessBoard[Position];
                    if (ChessRules.IsChecked(chessBoard, r1))
                    {
                        chessBoard[r1] = null;
                        r1 = null;
                        Console.WriteLine("THIS POSITION IS UNDER CHECK");
                        return false;
                    }
                    else
                    {
                        chessBoard[r1] = null;
                        r1 = null;
                    }

                    PiecePosition? r2 = target;
                    chessBoard[r2] = chessBoard[Position];
                    if (ChessRules.IsChecked(chessBoard, r2))
                    {
                        chessBoard[r2] = null;
                        r2 = null;
                        Console.WriteLine("THIS POSITION IS UNDER CHECK");
                        return false;
                    }
                    else
                    {
                        chessBoard[r2] = null;
                        r2 = null;
                    }

                    return true;
                }
            }
        }

        if (Math.Abs(Position.Row - target.Row) == 1 && Math.Abs(Position.Col - target.Col) == 0)
            return true;
        else if (Math.Abs(Position.Row - target.Row) == 0 && Math.Abs(Position.Col - target.Col) == 1)
            return true;
        else if (Math.Abs(Position.Row - target.Row) + Math.Abs(Position.Col - target.Col) == 2)
            return true;

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
