using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Bishop(PieceColor color) : Piece(color, PieceType.Bishop, 'b')
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);

    //public override ChessBoard IsMovePossible(ChessBoard board, 
    //    PiecePosition startposition, PiecePosition targetposition)
    //{
    //    if (Math.Abs(startposition.X - targetposition.X) == Math.Abs(startposition.Y - targetposition.Y))
    //    {
    //        board[startposition.X, startposition.Y] = board[targetposition.X, targetposition.Y];
    //        return board;
    //    }
    //    return board;
    //}
    public  override bool IsMovePossible(PiecePosition startposition, PiecePosition targetposition)
    {
        if (Math.Abs(startposition.Row - targetposition.Row) == Math.Abs(startposition.Col - targetposition.Col))
            return true;
        return base.IsMovePossible(startposition, targetposition);
    }
}

