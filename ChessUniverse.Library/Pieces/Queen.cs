using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Queen(PieceColor color) : Piece(color, PieceType.Queen, 'q', new PiecePosition())
{
    Piece rook = new Rook(PieceColor.Black);
    Piece bishop = new Rook(PieceColor.Black );
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(ChessBoard chessBoard,   PiecePosition startposition, PiecePosition targetposition)
    {
        if (rook.IsMovePossible( chessBoard,  startposition,  targetposition)
            || bishop.IsMovePossible( chessBoard,  startposition,  targetposition))
            return true;
        return false;
    }
}
