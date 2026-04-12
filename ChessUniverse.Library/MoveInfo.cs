using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public class MoveInfo
{
    public PiecePosition? Start { get; set; }
    public PiecePosition? Target { get; set; }
    public (bool, PieceColor) Castling { get; set; }
    public Piece? MovedPiece { get; set; }
    public Piece? CapturedPiece { get; set; }
    public PieceColor T { get; set; }
    public MoveInfo() { }
    public MoveInfo(MoveInfo original)
    {
        Start = original.Start;
        Target = original.Target;
        Castling = original.Castling;
        MovedPiece = (Piece?)original.MovedPiece?.Clone();
        CapturedPiece = (Piece?)original.CapturedPiece?.Clone();
        T = original.T;
    }
    public MoveInfo(PiecePosition start, PiecePosition target)
    {
        Start = start;
        Target = target;
    }
    public MoveInfo(PiecePosition start, PiecePosition target, PieceColor turn)
    {
        Start = start;
        Target = target;
        T = turn;
    }
    public MoveInfo(PiecePosition start, PiecePosition target, Piece movedPiece, Piece? capturedFigure, (bool, PieceColor) castling)
    {
        Start = start;
        Target = target;
        MovedPiece = movedPiece;
        CapturedPiece = capturedFigure;
        Castling = castling;
    }
}
