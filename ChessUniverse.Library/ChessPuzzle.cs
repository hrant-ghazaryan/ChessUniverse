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
    public PuzzleCategory Category { get; }
    public PuzzleDifficulty Difficulty { get; }

    public ChessPuzzle(
        int id,
        int worldId,
        int levelNumber,
        string title,
        ChessBoard initialBoard,
        PieceColor activeTurn,
        IEnumerable<MoveInfo> solutionMoves)
        : this(
            id,
            worldId,
            levelNumber,
            PuzzleCategory.MateInOne,
            PuzzleDifficulty.Easy,
            title,
            initialBoard,
            activeTurn,
            solutionMoves)
    {
    }

    public ChessPuzzle(
        int id,
        int worldId,
        int levelNumber,
        PuzzleCategory category,
        string title,
        ChessBoard initialBoard,
        PieceColor activeTurn,
        IEnumerable<MoveInfo> solutionMoves)
        : this(
            id,
            worldId,
            levelNumber,
            category,
            PuzzleDifficulty.Easy,
            title,
            initialBoard,
            activeTurn,
            solutionMoves)
    {
    }

    public ChessPuzzle(
        int id,
        int worldId,
        int levelNumber,
        PuzzleCategory category,
        PuzzleDifficulty difficulty,
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
        Category = category;
        Difficulty = difficulty;
        Title = title;

        InitialBoard = (ChessBoard)initialBoard.Clone();
        ActiveTurn = activeTurn;

        SolutionMoves = solutionMoves
            .Select(move => new MoveInfo(move))
            .ToList()
            .AsReadOnly();
    }
}
