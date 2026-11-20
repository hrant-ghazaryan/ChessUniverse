using ChessUniverse.Library;
using ChessUniverse.Mobile.ViewModels;

namespace ChessUniverse.Mobile.Pages;

public partial class PuzzleBoardPage : ContentPage
{
    private readonly PuzzleBoardViewModel _viewModel;
    private readonly ChessPuzzle? _nextPuzzle;
    private readonly Dictionary<BoardSquareViewModel, Border> _squareControls = [];

    public PuzzleBoardPage(ChessPuzzle puzzle)
    {
        InitializeComponent();

        _nextPuzzle = PuzzleCatalog
            .GetPuzzles(puzzle.Category, puzzle.Difficulty)
            .FirstOrDefault(next =>
                next.LevelNumber > puzzle.LevelNumber);

        _viewModel = new PuzzleBoardViewModel(
            puzzle,
            _nextPuzzle is not null);
        BindingContext = _viewModel;

        CreateBoard();
    }

    private void CreateBoard()
    {
        foreach (BoardSquareViewModel square in _viewModel.Squares)
        {
            Label pieceLabel = new()
            {
                FontSize = 30,
                TextColor = Colors.Black,
                InputTransparent = true,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            Border squareControl = new()
            {
                Padding = 0,
                StrokeThickness = 0,
                Content = pieceLabel,
                BindingContext = square
            };

            TapGestureRecognizer tap = new();
            tap.Tapped += (_, _) => OnSquareSelected(square);
            squareControl.GestureRecognizers.Add(tap);

            Grid.SetRow(squareControl, square.Row);
            Grid.SetColumn(squareControl, square.Col);

            BoardGrid.Children.Add(squareControl);
            _squareControls.Add(square, squareControl);
        }

        RefreshBoardButtons();
    }

    private void OnSquareSelected(BoardSquareViewModel square)
    {
        _viewModel.SelectSquare(square);
        RefreshBoardButtons();
    }

    private async void OnNextClicked(object? sender, EventArgs e)
    {
        if (_nextPuzzle is null)
            return;

        await Navigation.PushAsync(new PuzzleBoardPage(_nextPuzzle));
        Navigation.RemovePage(this);
    }

    private void RefreshBoardButtons()
    {
        foreach ((BoardSquareViewModel square, Border control) in _squareControls)
        {
            if (control.Content is Label pieceLabel)
                pieceLabel.Text = square.PieceSymbol;

            control.BackgroundColor = Color.FromArgb(square.BackgroundColor);
        }
    }
}
