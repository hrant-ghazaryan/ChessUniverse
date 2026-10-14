using ChessUniverse.Library;
using ChessUniverse.Library.Enums;

namespace ChessUniverse.Mobile.ViewModels;

public sealed class PuzzleListViewModel
{
    public string CategoryTitle { get; }

    public IReadOnlyList<PuzzleListItemViewModel> Puzzles { get; }

    public PuzzleListViewModel(
        PuzzleCategory category,
        PuzzleDifficulty difficulty)
    {
        CategoryTitle = $"{GetCategoryTitle(category)} · {difficulty}";

        Puzzles = PuzzleCatalog
            .GetPuzzles(category, difficulty)
            .Select(puzzle => new PuzzleListItemViewModel(
                puzzle,
                GetPreviewImage(category)))
            .ToList()
            .AsReadOnly();
    }

    private static string GetPreviewImage(PuzzleCategory category)
        => category switch
        {
            PuzzleCategory.MateInOne => "mate_in_one.png",
            PuzzleCategory.MateInTwo => "mate_in_two.png",
            _ => throw new ArgumentOutOfRangeException(nameof(category))
        };

    private static string GetCategoryTitle(PuzzleCategory category)
        => category switch
        {
            PuzzleCategory.MateInOne => "Mate in 1",
            PuzzleCategory.MateInTwo => "Mate in 2",
            _ => "Puzzles"
        };
}

public sealed class PuzzleListItemViewModel
{
    public ChessPuzzle Puzzle { get; }

    public string PreviewImage { get; }

    public PuzzleListItemViewModel(
        ChessPuzzle puzzle,
        string previewImage)
    {
        Puzzle = puzzle;
        PreviewImage = previewImage;
    }
}
