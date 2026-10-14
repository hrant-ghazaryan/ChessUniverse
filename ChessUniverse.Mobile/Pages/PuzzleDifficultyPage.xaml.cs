using ChessUniverse.Library.Enums;
using ChessUniverse.Mobile.ViewModels;

namespace ChessUniverse.Mobile.Pages;

public partial class PuzzleDifficultyPage : ContentPage
{
    private readonly PuzzleCategory _category;

    public PuzzleDifficultyPage(PuzzleCategory category)
    {
        InitializeComponent();

        _category = category;
        BindingContext = new PuzzleDifficultiesViewModel(category);
    }

    private async void OnDifficultySelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not PuzzleDifficultyCard card)
            return;

        if (sender is CollectionView collectionView)
            collectionView.SelectedItem = null;

        await Navigation.PushAsync(
            new PuzzleListPage(_category, card.Difficulty));
    }
}
