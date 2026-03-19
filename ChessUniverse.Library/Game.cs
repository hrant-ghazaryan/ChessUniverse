using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public class Game
{
    public PieceColor T { get; set; }

    public static List<MoveAffirmation>? moves = new List<MoveAffirmation>();
    public static ChessBoard UndoMove(ChessBoard board, ref PieceColor T)
    {
        MoveAffirmation lastMove = moves!.Last();

        PiecePosition? start = moves?[moves.Count - 1].Start;
         T = board[moves[moves.Count - 1].Target].Color;
        PiecePosition? target = moves?[moves.Count - 1].Target;
        lastMove.MoveBack(board, lastMove.Start, ref T);
        moves?.RemoveAt(moves.Count - 1);
        if (T == PieceColor.White)
            T = PieceColor.Black;
        else
            T = PieceColor.White;
        return board;
    }
    public static ChessBoard ResetGame(ChessBoard board)
    {
        board.SetStartPosition();
        return board;
    }

}
