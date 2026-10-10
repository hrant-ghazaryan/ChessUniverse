namespace ChessUniverse.Library;

public sealed class ChessWorld
{
    public int Id { get; }

    public string Title { get; }

    public IReadOnlyList<ChessPuzzle> Puzzles { get; }

    public ChessWorld(
        int id,
        string title,
        IEnumerable<ChessPuzzle> puzzles)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(puzzles);

        List<ChessPuzzle> puzzleList = puzzles.ToList();

        if (puzzleList.Any(puzzle => puzzle.WorldId != id))
        {
            throw new ArgumentException(
                "Every puzzle must belong to this world.",
                nameof(puzzles));
        }

        if (puzzleList
            .GroupBy(puzzle => puzzle.LevelNumber)
            .Any(group => group.Count() > 1))
        {
            throw new ArgumentException(
                "Level numbers must be unique within a world.",
                nameof(puzzles));
        }

        Id = id;
        Title = title;

        Puzzles = puzzleList
            .OrderBy(puzzle => puzzle.LevelNumber)
            .ToList()
            .AsReadOnly();
    }
}
