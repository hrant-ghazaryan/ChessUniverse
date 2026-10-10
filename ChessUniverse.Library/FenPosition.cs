using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public sealed class FenPosition
{
    public ChessBoard Board { get; }

    public PieceColor ActiveTurn { get; }

    public FenPosition(
        ChessBoard board,
        PieceColor activeTurn)
    {
        ArgumentNullException.ThrowIfNull(board);

        Board = (ChessBoard)board.Clone();
        ActiveTurn = activeTurn;
    }
}
