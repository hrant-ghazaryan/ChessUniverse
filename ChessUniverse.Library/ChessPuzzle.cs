using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public sealed class ChessPuzzle
{
    public int Id { get; }

    public int WorldId { get; }

    public int LevelNumber { get; }

    public string Title { get; }

    public ChessBoard InitialBoard { get; }

    public PieceColor ActiveTurn { get; }

    public IReadOnlyList<MoveInfo> SolutionMoves { get; }

    public ChessPuzzle(
        int id,
        int worldId,
        int levelNumber,
        string title,
        ChessBoard initialBoard,
        PieceColor activeTurn,
        IEnumerable<MoveInfo> solutionMoves)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(worldId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(levelNumber);

        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(initialBoard);
        ArgumentNullException.ThrowIfNull(solutionMoves);

        Id = id;
        WorldId = worldId;
        LevelNumber = levelNumber;
        Title = title;

        InitialBoard = (ChessBoard)initialBoard.Clone();
        ActiveTurn = activeTurn;

        SolutionMoves = solutionMoves
            .Select(move => new MoveInfo(move))
            .ToList()
            .AsReadOnly();
    }
}
