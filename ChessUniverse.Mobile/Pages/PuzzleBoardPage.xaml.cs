using ChessUniverse.Library;
using ChessUniverse.Mobile.ViewModels;

namespace ChessUniverse.Mobile.Pages;

public partial class PuzzleBoardPage : ContentPage
{
    private readonly PuzzleBoardViewModel _viewModel;
    private readonly Dictionary<BoardSquareViewModel, Button> _squareButtons = [];

    public PuzzleBoardPage(ChessPuzzle puzzle)
    {
        InitializeComponent();

        _viewModel = new PuzzleBoardViewModel(puzzle);
        BindingContext = _viewModel;

        CreateBoard();
    }

    private void CreateBoard()
    {
        foreach (BoardSquareViewModel square in _viewModel.Squares)
        {
            Button button = new()
            {
                Padding = 0,
                CornerRadius = 0,
                BorderWidth = 0,
                FontSize = 30,
                TextColor = Colors.Black,
                BindingContext = square
            };

            button.Clicked += OnSquareClicked;

            Grid.SetRow(button, square.Row);
            Grid.SetColumn(button, square.Col);

            BoardGrid.Children.Add(button);
            _squareButtons.Add(square, button);
        }

        RefreshBoardButtons();
    }

    private void OnSquareClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button ||
            button.BindingContext is not BoardSquareViewModel square)
        {
            return;
        }

        _viewModel.SelectSquare(square);
        RefreshBoardButtons();
    }

    private void RefreshBoardButtons()
    {
        foreach ((BoardSquareViewModel square, Button button) in _squareButtons)
        {
            button.Text = square.PieceSymbol;
            button.BackgroundColor = Color.FromArgb(square.BackgroundColor);
        }
    }
}
