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
    public override bool IsMovePossible(ChessBoard chessBoard, PiecePosition start, PiecePosition target)
    {
        if (color == PieceColor.White)
        {
            if (start.Row - target.Row == 1 && start.Col == target.Col)
                return true;
            else if (start.Row - target.Row == 2 && !IsMoved && start.Col == target.Col)
                return true;
        }
        if (color == PieceColor.Black)
        {
            if (target.Row - start.Row == 1 && start.Col == target.Col)
                return true;
            else if (target.Row - start.Row == 2 && !IsMoved && start.Col == target.Col)
                return true;
        }
        return false;
    }
}
