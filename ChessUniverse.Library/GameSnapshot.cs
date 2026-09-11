using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public sealed class GameSnapshot
{
    public ChessBoard Board { get; }

    public PieceColor ActiveTurn { get; }

    public MoveInfo? PreviousMove { get; }

    public IReadOnlyList<Piece> CapturedPieces { get; }

    public GameSnapshot(
        ChessBoard board,
        PieceColor activeTurn,
        MoveInfo? previousMove,
        IEnumerable<Piece> capturedPieces)
    {
        ArgumentNullException.ThrowIfNull(board);
        ArgumentNullException.ThrowIfNull(capturedPieces);

        Board = (ChessBoard)board.Clone();
        ActiveTurn = activeTurn;

        PreviousMove = previousMove is null
            ? null
            : new MoveInfo(previousMove);

        CapturedPieces = capturedPieces
            .Select(piece => (Piece)piece.Clone())
            .ToList()
            .AsReadOnly();
    }
}