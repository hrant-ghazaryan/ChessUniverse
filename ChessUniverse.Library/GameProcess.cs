using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public class GameProcess
{
    public static List<MoveInfo> moves = new List<MoveInfo>();
    public static ChessBoard UndoMove(ChessBoard board, ref PieceColor T)
    {
        if (moves == null || moves.Count == 0)
            throw new Exception("No moves to undo");

        MoveInfo? lastMove = moves.Last();

        PiecePosition start = moves[moves.Count - 1].Start;
        PiecePosition target = moves[moves.Count - 1].Target;

        if (board[target] is not Piece piece)
            throw new Exception("No piece at target position");

        T = piece.Color;
        //lastMove?.MoveBack(board, lastMove.Start, ref T);
        moves.RemoveAt(moves.Count - 1);

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
