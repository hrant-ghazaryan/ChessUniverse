using ChessUniverse.Library.Enums;

namespace ChessUniverse.Mobile.ViewModels;

public sealed class PuzzleCategoriesViewModel
{
    public IReadOnlyList<PuzzleCategoryCard> Categories { get; } =
    [
        new(
            PuzzleCategory.MateInOne,
            "mate_in_one.png"),

        new(
            PuzzleCategory.MateInTwo,
            "mate_in_two.png")
    ];
}
public sealed class PuzzleCategoryCard
{
    public PuzzleCategory Category { get; }

    public string PreviewImage { get; }

    public PuzzleCategoryCard(
        PuzzleCategory category,
        string previewImage)
    {
        Category = category;
        PreviewImage = previewImage;
    }
}
