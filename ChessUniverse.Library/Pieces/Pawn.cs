using ChessUniverse.Library.Enums;
namespace ChessUniverse.Library.Pieces;

public class Pawn(PieceColor color) : Piece(color, PieceType.Pawn, 'p', new PiecePosition())
{

    private bool _isMoved;
    public bool IsMoved
    {
        get => _isMoved; 
        set
        {
            if (Position.Row == 2 || Position.Row == 6)
                _isMoved = false;
            else
                _isMoved = true;
        }
    }

    public override char GetSymbol(PieceColor color)
     => base.GetSymbol(color);
    public override bool IsMovePossible(ChessBoard chessBoard, PiecePosition target)
    {
        if (color == PieceColor.White)
        {
            if (Position.Row - target.Row == 1 && Math.Abs(Position.Col - target.Col) == 1
                && chessBoard[target] != null)
                return true;
            if (Position.Row - target?.Row == 1 && Position.Col == target?.Col)
                return true;
            else if (Position.Row - target?.Row == 2 && !HasMoved && Position.Col == target?.Col)
                return true;
        }
        if (color == PieceColor.Black)
        {
            if (target!.Row - Position.Row == 1 && Math.Abs(target!.Col - Position.Col) == 1
                && chessBoard[target] != null)
                return true;
            if (target?.Row - Position.Row == 1 && Position.Col == target?.Col)
                return true;
            else if (target?.Row - Position.Row == 2 && !HasMoved && Position.Col == target?.Col)
                return true;
        }
        return false;
    }
}
