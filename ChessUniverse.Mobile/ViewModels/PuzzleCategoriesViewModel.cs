using ChessUniverse.Library.Enums;

namespace ChessUniverse.Mobile.ViewModels;

public sealed class PuzzleCategoriesViewModel
{
    public IReadOnlyList<PuzzleCategoryCard> Categories { get; } =
    [
        new(
            PuzzleCategory.MateInOne,
            "Mate in 1"),

        new(
            PuzzleCategory.MateInTwo,
            "Mate in 2")
    ];
}
public sealed class PuzzleCategoryCard
{
    public PuzzleCategory Category { get; }

    public string Title { get; }

    public PuzzleCategoryCard(
        PuzzleCategory category,
        string title)
    {
        Category = category;
        Title = title;
    }
}
