using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library.Pieces;

public class Queen(PieceColor color) : Piece(color, PieceType.Queen, 'q', new PiecePosition())
{
    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool CanMove(ChessBoard chessBoard, PiecePosition targetposition)
    {
        if (chessBoard[targetposition]?.Color == chessBoard[Position]?.Color)
            return false;

        Rook rook = new Rook(Color) { Position = this.Position };
        if (rook.CanMove(chessBoard, targetposition))
            return true;

        Bishop bishop = new Bishop(Color) { Position = this.Position };
        if (bishop.CanMove(chessBoard, targetposition))
            return true;

        return false;
    }
    public override Piece Clone()
        => new Queen(this.Color)
        {
            Position = new PiecePosition(this.Position.Row, this.Position.Col),
            HasMoved = this.HasMoved
        };
}
