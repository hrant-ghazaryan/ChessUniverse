using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public sealed class FenPosition
{
    public ChessBoard Board { get; }

    public PieceColor ActiveTurn { get; }

    // FEN-ի en passant դաշտից վերականգնված նախորդ քայլը (զինվորի երկու վանդականոց քայլը)։
    // null է, եթե en passant դաշտը "-" է կամ բացակայում է։
    public MoveInfo? PreviousMove { get; }

    public int HalfmoveClock { get; }

    public int FullmoveNumber { get; }

    public FenPosition(
        ChessBoard board,
        PieceColor activeTurn,
        MoveInfo? previousMove = null,
        int halfmoveClock = 0,
        int fullmoveNumber = 1)
    {
        ArgumentNullException.ThrowIfNull(board);

        Board = (ChessBoard)board.Clone();
        ActiveTurn = activeTurn;
        PreviousMove = previousMove is null ? null : new MoveInfo(previousMove);
        HalfmoveClock = halfmoveClock;
        FullmoveNumber = fullmoveNumber;
    }
}
