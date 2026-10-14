using ChessUniverse.Library.Enums;

namespace ChessUniverse.Mobile.ViewModels;

public sealed class PuzzleDifficultiesViewModel
{
    public string CategoryTitle { get; }

    public IReadOnlyList<PuzzleDifficultyCard> Difficulties { get; } =
    [
        new(PuzzleDifficulty.Easy, "Easy", "Սկսնակների համար", "#3D8B40"),
        new(PuzzleDifficulty.Medium, "Medium", "Ավելի բարդ դիրքեր", "#D9822B"),
        new(PuzzleDifficulty.Hard, "Hard", "Իրական մարտահրավեր", "#B93A3A")
    ];

    public PuzzleDifficultiesViewModel(PuzzleCategory category)
    {
        CategoryTitle = category switch
        {
            PuzzleCategory.MateInOne => "Mate in 1",
            PuzzleCategory.MateInTwo => "Mate in 2",
            _ => "Puzzles"
        };
    }
}

public sealed class PuzzleDifficultyCard
{
    public PuzzleDifficulty Difficulty { get; }
    public string Title { get; }
    public string Description { get; }
    public string Color { get; }

    public PuzzleDifficultyCard(
        PuzzleDifficulty difficulty,
        string title,
        string description,
        string color)
    {
        Difficulty = difficulty;
        Title = title;
        Description = description;
        Color = color;
    }
}
