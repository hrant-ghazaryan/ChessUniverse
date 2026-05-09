using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public class MoveResult
{
    public ChessBoard Board { get; set; }
    public MoveType MoveType { get; set; }
    public BoardState BoardState { get; set; }
    public PieceColor Turn { get; set; } 
    public MoveResult(ChessBoard board, MoveType moveType)
    {
        Board = board;
        MoveType = moveType;
    }
    public MoveResult(ChessBoard board, MoveType moveType, BoardState boardState)
    {
        Board = board;
        MoveType = moveType;
        BoardState = boardState;
    }
    public MoveResult(ChessBoard board, MoveType moveType, BoardState boardState, PieceColor turn)
    {
        Board = board;
        MoveType = moveType;
        BoardState = boardState;
        Turn = turn;
    }
}
