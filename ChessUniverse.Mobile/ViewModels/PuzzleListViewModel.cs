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
                GetPreviewImage(puzzle)))
            .ToList()
            .AsReadOnly();
    }

    private static string GetPreviewImage(ChessPuzzle puzzle)
        => puzzle.Id switch
        {
            1 => "mate_in_one_level_1.jpg",
            2 => "mate_in_two_level_1.jpg",
            3 => "mate_in_one_level_2.jpg",
            _ => GetFallbackPreviewImage(puzzle.Category)
        };

    private static string GetFallbackPreviewImage(PuzzleCategory category)
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
