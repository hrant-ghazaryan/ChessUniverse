namespace ChessUniverse.Library;

public readonly struct CastlingRookMove(PiecePosition startPosition, PiecePosition targetPosition)
{
    public PiecePosition StartPosition { get; } = startPosition;
    public PiecePosition TargetPosition { get; } = targetPosition;
}
