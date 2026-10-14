using ChessUniverse.Library.Enums;
using ChessUniverse.Mobile.ViewModels;

namespace ChessUniverse.Mobile.Pages;

public partial class PuzzleListPage : ContentPage
{
    public PuzzleListPage(
        PuzzleCategory category,
        PuzzleDifficulty difficulty)
    {
        InitializeComponent();

        BindingContext = new PuzzleListViewModel(category, difficulty);
    }
    private async void OnPuzzleSelectionChanged(
    object? sender,
    SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not PuzzleListItemViewModel item)
            return;

        if (sender is CollectionView collectionView)
            collectionView.SelectedItem = null;

        await Navigation.PushAsync(new PuzzleBoardPage(item.Puzzle));
    }
}
