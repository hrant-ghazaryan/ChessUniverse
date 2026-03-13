using ChessUniverse.Library.Enums;
using System.IO.Pipelines;
namespace ChessUniverse.Library.Pieces;

public class King(PieceColor color) : Piece(color, PieceType.King, 'k', new PiecePosition())
{
 
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(ChessBoard chessBoard, PiecePosition start, PiecePosition? target)
    {
        if (start.Col - target.Col == 2 && chessBoard[target.Row, target.Col - 2]?.Type == PieceType.Rook)
        {
            if (chessBoard[start.Row, start.Col]?.HasMoved == false
                && chessBoard[target.Row, target.Col - 2]?.HasMoved == false)
            {
                if (chessBoard[target.Row, target.Col] == null
                    && chessBoard[target.Row, target.Col + 1] == null)
                {
                    PiecePosition l = start;
                    PiecePosition l1 = new PiecePosition { Row = target.Row, Col = target.Col + 1 };
                    PiecePosition l2 = target;

                    chessBoard[l] = chessBoard[start];
                    chessBoard[l1] = chessBoard[start];
                    chessBoard[l2] = chessBoard[start];

                    if (ChessBoard.IsChecked(chessBoard, l) == false)
                    {
                        if (ChessBoard.IsChecked(chessBoard, l1) == false)
                        {
                            if (ChessBoard.IsChecked(chessBoard, l2) == false)
                                return true;
                            else
                            {
                                chessBoard[l1] = null;
                                chessBoard[l2] = null;
                                Console.WriteLine("This position in CHECK");
                            }
                        }
                    }
                }
            }
        }
        else if (target.Col - start.Col == 2 && chessBoard[target.Row, target.Col + 1]?.Type == PieceType.Rook)
        {
            if (chessBoard[Position.Row, Position.Col]?.HasMoved == false
                && chessBoard[target.Row, target.Col + 1]?.HasMoved == false)
            {
                if (chessBoard[target.Row, target.Col] == null
                    && chessBoard[target.Row, target.Col - 1] == null)
                {

                    PiecePosition r = start;
                    PiecePosition r1 = new PiecePosition { Row = target.Row, Col = target.Col - 1 };
                    PiecePosition r2 = target;

                    chessBoard[r] = chessBoard[start];
                    chessBoard[r1] = chessBoard[start];
                    chessBoard[r2] = chessBoard[start];

                    if (ChessBoard.IsChecked(chessBoard, r) == false)
                    {
                        if (ChessBoard.IsChecked(chessBoard, r1) == false)
                        {
                            if (ChessBoard.IsChecked(chessBoard, r2) == false)
                                return true;
                            else
                            {
                                chessBoard[r1] = null;
                                chessBoard[r2] = null;
                                Console.WriteLine("This position in CHECK");
                            }
                            return false;
                        }
                    }
                }
            }
        }
        if (Math.Abs(start.Row - target!.Row) == 1 && Math.Abs(start.Col - target.Col) == 0)
            return true;
        else if (Math.Abs(start.Row - target.Row) == 0 && Math.Abs(start.Col - target.Col) == 1)
            return true;
        else if (Math.Abs(start.Row - target.Row) + Math.Abs(start.Col - target.Col) == 2)
            return true;
        if (chessBoard[start.Row, start.Col]?.Type == PieceType.King && Math.Abs(target.Col - start.Col) == 2 
            && chessBoard[target.Row,target.Col + 1]?.Type == PieceType.Rook 
            && chessBoard[target.Row, target.Col + 1]?.HasMoved == false)
        {
            if (chessBoard[start.Row, start.Col]?.HasMoved == false
                && chessBoard[target.Row, target.Col + 1]?.HasMoved == false)
                return true;
        }
        
        return false;
    }
}
