using ChessUniverse.Mobile.Pages;
using ChessUniverse.Mobile.ViewModels;

namespace ChessUniverse.Mobile;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        BindingContext = new PuzzleCategoriesViewModel();
    }
    private async void OnCategorySelectionChanged(
    object? sender,
    SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not PuzzleCategoryCard categoryCard)
            return;

        if (sender is CollectionView collectionView)
            collectionView.SelectedItem = null;

        await Navigation.PushAsync(
            new PuzzleDifficultyPage(categoryCard.Category));
    }
}
