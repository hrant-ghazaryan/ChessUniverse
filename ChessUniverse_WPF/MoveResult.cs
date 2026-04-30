using ChessUniverse.Library;
using ChessUniverse.Library.Enums;

namespace ChessUniverse_WPF;

public class MoveResult
{
    public ChessBoard Board { get; set; }
    public MoveType MoveType { get; set; }
    public MoveResult(ChessBoard board, MoveType moveType)
    {
        Board = board;
        MoveType = moveType;
    }
}
