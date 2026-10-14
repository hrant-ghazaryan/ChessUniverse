using ChessUniverse.Library;

namespace ChessUniverse.Mobile.ViewModels;

public sealed class WorldsViewModel
{
    public IReadOnlyList<ChessWorld> Worlds =>
        PuzzleCatalog.Worlds;
}