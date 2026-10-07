using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public sealed class PuzzleMoveResult
{
    public PuzzleMoveStatus Status { get; }

    public MoveResult? GameMoveResult { get; }

    public IReadOnlyList<MoveResult> AutomaticMoveResults { get; }

    public PuzzleMoveResult(
        PuzzleMoveStatus status,
        MoveResult? gameMoveResult = null,
        IEnumerable<MoveResult>? automaticMoveResults = null)
    {
        Status = status;
        GameMoveResult = gameMoveResult;

        AutomaticMoveResults = automaticMoveResults is null
            ? Array.Empty<MoveResult>()
            : automaticMoveResults
             .ToList()
             .AsReadOnly();
    }
}
