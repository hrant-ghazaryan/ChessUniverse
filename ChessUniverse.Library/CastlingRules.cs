using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public static class CastlingRules
{
    public static bool ValidateCastlingParameters(
        ChessBoard chessBoard,
        MoveInfo moveInfo)
        => chessBoard is not null && moveInfo is not null &&
               moveInfo.Start is not null && moveInfo.Target is not null;

    public static CastlingRookMove GetCastlingRookMove(MoveInfo moveInfo)
    {
        ArgumentNullException.ThrowIfNull(moveInfo);

        PiecePosition target = moveInfo.Target
            ?? throw new ArgumentException("Castling target is required.", nameof(moveInfo));

        bool isKingSide = target.Col == 6;

        int rookStartCol = isKingSide ? 7 : 0;
        int rookTargetCol = isKingSide ? 5 : 3;

        return new CastlingRookMove(
            new PiecePosition(target.Row, rookStartCol),
            new PiecePosition(target.Row, rookTargetCol));
    }

    public static bool IsCastlingLeftPossible(ChessBoard chessBoard, MoveInfo moveInfo)
        => IsCastlingPossible(chessBoard, moveInfo, isKingSide: false);
    public static bool IsCastlingRightPossible(ChessBoard chessBoard, MoveInfo moveInfo)
        => IsCastlingPossible(chessBoard, moveInfo, isKingSide: true);

    private static bool IsCastlingPossible( ChessBoard board,
        MoveInfo moveInfo, bool isKingSide)
    {
        if (!ValidateCastlingParameters(board, moveInfo))
            return false;

        PiecePosition start = moveInfo.Start!;
        PiecePosition target = moveInfo.Target!;
        Piece? king = board[start];

        if (king is null || king.Type is not PieceType.King)
            return false;

        int homeRow = king.Color == PieceColor.White ? 7 : 0;
        int expectedTargetCol = isKingSide ? 6 : 2;
        int rookCol = isKingSide ? 7 : 0;

        if (start.Row != homeRow || start.Col != 4 ||
            target.Row != homeRow || target.Col != expectedTargetCol)
            return false;

        Piece? rook = board[homeRow, rookCol];

        if (rook is null || rook.Type != PieceType.Rook ||
            rook.Color != king.Color || king.HasMoved || rook.HasMoved)
            return false;

        int firstEmptyCol = isKingSide ? 5 : 1;
        int lastEmptyCol = isKingSide ? 6 : 3;

        for (int col = firstEmptyCol; col <= lastEmptyCol; col++)
        {
            if (board[homeRow, col] is not null)
                return false;
        }

        if (ChessRules.IsChecked(board, start, king.Color))
            return false;

        int intermediateCol = isKingSide ? 5 : 3;
        PiecePosition intermediate = new PiecePosition(homeRow, intermediateCol);

        if (IsKingInCheckAfterMove(board, start, intermediate, king.Color))
            return false;

        return !IsKingInCheckAfterCastling( board,
            start, target,
            homeRow, rookCol,
            isKingSide ? 5 : 3, king.Color);
    }

    private static bool IsKingInCheckAfterMove( ChessBoard board,
        PiecePosition start, PiecePosition target, PieceColor color)
    {
        ChessBoard boardAfterMove = (ChessBoard)board.Clone();

        Piece king = boardAfterMove[start]!;
        boardAfterMove[target] = king;
        boardAfterMove[start] = null;
        king.Position = target;

        return ChessRules.IsChecked(boardAfterMove, target, color);
    }

    private static bool IsKingInCheckAfterCastling( ChessBoard board, 
        PiecePosition kingStart, PiecePosition kingTarget,
        int row, int rookStartCol, int rookTargetCol,
        PieceColor color)
    {
        ChessBoard boardAfterCastling = (ChessBoard)board.Clone();

        Piece king = boardAfterCastling[kingStart]!;
        Piece rook = boardAfterCastling[row, rookStartCol]!;

        boardAfterCastling[kingTarget] = king;
        boardAfterCastling[kingStart] = null;
        king.Position = kingTarget;

        boardAfterCastling[row, rookTargetCol] = rook;
        boardAfterCastling[row, rookStartCol] = null;
        rook.Position = new PiecePosition(row, rookTargetCol);

        return ChessRules.IsChecked(boardAfterCastling, kingTarget, color);
    }
}