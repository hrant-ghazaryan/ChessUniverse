using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChessUniverse.Library;
using ChessUniverse.Library.Enums;

namespace ChessUniverse.Mobile.ViewModels;

public sealed class PuzzleBoardViewModel : INotifyPropertyChanged
{
    private readonly ChessPuzzleSession _session;
    private BoardSquareViewModel? _selectedSquare;
    private string _statusMessage;

    public string Title { get; }

    public ObservableCollection<BoardSquareViewModel> Squares { get; }

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public PuzzleBoardViewModel(ChessPuzzle puzzle)
    {
        Title = puzzle.Title;
        _session = new ChessPuzzleSession(puzzle);
        _statusMessage = "Ընտրիր սպիտակ խաղաքարը";
        Squares = [];

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Squares.Add(new BoardSquareViewModel(row, col));
            }
        }

        RefreshBoard();
    }

    public void SelectSquare(BoardSquareViewModel square)
    {
        if (_session.IsCompleted)
            return;

        if (_selectedSquare is null)
        {
            Piece? piece = _session.Board[square.Row, square.Col];

            if (piece?.Color != _session.PlayerColor)
            {
                StatusMessage = "Ընտրիր քո խաղաքարերից մեկը";
                return;
            }

            _selectedSquare = square;
            square.IsSelected = true;
            StatusMessage = "Ընտրիր այն վանդակը, ուր ուզում ես քայլել";
            return;
        }

        MoveInfo move = new(
            new PiecePosition(_selectedSquare.Row, _selectedSquare.Col),
            new PiecePosition(square.Row, square.Col));

        _selectedSquare.IsSelected = false;
        _selectedSquare = null;

        PuzzleMoveResult result = _session.TryMove(move);

        StatusMessage = result.Status switch
        {
            PuzzleMoveStatus.CorrectMove => "Ճիշտ քայլ է։ Շարունակի՛ր",
            PuzzleMoveStatus.Completed => "Շնորհավոր, puzzle-ը լուծված է։",
            PuzzleMoveStatus.IncorrectMove => "Սա ճիշտ քայլը չէ։ Կրկին փորձիր։",
            PuzzleMoveStatus.InvalidMove => "Այս քայլը հնարավոր չէ։",
            _ => "Խնդիր առաջացավ։"
        };

        RefreshBoard();
    }

    private void RefreshBoard()
    {
        foreach (BoardSquareViewModel square in Squares)
        {
            square.UpdatePiece(_session.Board[square.Row, square.Col]);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public sealed class BoardSquareViewModel : INotifyPropertyChanged
{
    private string _pieceSymbol = string.Empty;
    private bool _isSelected;

    public int Row { get; }

    public int Col { get; }

    public string BackgroundColor =>
        _isSelected
            ? "#E9D74C"
            : (Row + Col) % 2 == 0
                ? "#F0D9B5"
                : "#B58863";

    public string PieceSymbol
    {
        get => _pieceSymbol;
        private set
        {
            _pieceSymbol = value;
            OnPropertyChanged();
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(BackgroundColor));
        }
    }

    public BoardSquareViewModel(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public void UpdatePiece(Piece? piece)
    {
        PieceSymbol = GetPieceSymbol(piece);
    }

    private static string GetPieceSymbol(Piece? piece)
    {
        if (piece is null)
            return string.Empty;

        return (piece.Type, piece.Color) switch
        {
            (PieceType.King, PieceColor.White) => "♔",
            (PieceType.Queen, PieceColor.White) => "♕",
            (PieceType.Rook, PieceColor.White) => "♖",
            (PieceType.Bishop, PieceColor.White) => "♗",
            (PieceType.Knight, PieceColor.White) => "♘",
            (PieceType.Pawn, PieceColor.White) => "♙",
            (PieceType.King, PieceColor.Black) => "♚",
            (PieceType.Queen, PieceColor.Black) => "♛",
            (PieceType.Rook, PieceColor.Black) => "♜",
            (PieceType.Bishop, PieceColor.Black) => "♝",
            (PieceType.Knight, PieceColor.Black) => "♞",
            (PieceType.Pawn, PieceColor.Black) => "♟",
            _ => string.Empty
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
