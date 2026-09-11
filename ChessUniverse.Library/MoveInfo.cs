using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public class MoveInfo
{
    public PiecePosition? Start { get; set; }
    public PiecePosition? Target { get; set; }
    public (bool, PieceColor)? Castling { get; set; }
    public Piece? MovedPiece { get; set; }
    public Piece? CapturedPiece { get; set; }
    public PieceColor Turn { get; set; }

    public MoveInfo() { }
    public MoveInfo(MoveInfo original)
    {
        Start = original.Start is null
            ? null
            : new PiecePosition(original.Start.Row, original.Start.Col);
        Target = original.Target is null
            ? null
            : new PiecePosition(original.Target.Row, original.Target.Col);
        Castling = original.Castling;
        MovedPiece = (Piece?)original.MovedPiece?.Clone();
        CapturedPiece = (Piece?)original.CapturedPiece?.Clone();
        Turn = original.Turn;
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
        Turn = turn;
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
