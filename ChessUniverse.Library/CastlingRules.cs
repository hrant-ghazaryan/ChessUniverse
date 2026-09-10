using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

/*public static class CastlingRules
{
    public static bool ValidateCastlingParameters(ChessBoard chessBoard, MoveInfo moveInfo)
    {
        if (chessBoard is null) return false;
        if (moveInfo is null) return false;
        if (moveInfo.Start is null) return false;
        if (moveInfo.Target is null) return false;

        return true;
    }
    public static CastlingRookMove GetCastlingRookMove(MoveInfo moveInfo)
    {
        var kingTargetRowPosition = moveInfo!.Target!.Row;
        var kingTargetColPosition = moveInfo!.Target!.Col;

        var rookTargetRowPosition = kingTargetRowPosition;
        var rookTargetColPosition = moveInfo!.Target!.Col == 6 ? 5 : 3;

        var rookStartRowPosition = rookTargetRowPosition;
        var rookStartColPosition = moveInfo!.Target!.Col == 6 ? 7 : 0;
        return new CastlingRookMove
        (
            new PiecePosition { Col = rookStartColPosition, Row = rookStartRowPosition },
            new PiecePosition { Col = rookTargetColPosition, Row = rookTargetRowPosition }
        );
    }
    public static bool IsCastlingLeftPossible(ChessBoard chessBoard, MoveInfo moveInfo)
    {
        if (moveInfo is null) return false;
        if (moveInfo.Start is null) return false;
        if (moveInfo.Target is null) return false;

        if (chessBoard[moveInfo.Start]?.Type != PieceType.King ||
            (ChessRules.IsInside(moveInfo.Target.Col - 2) && chessBoard[moveInfo.Target.Row, 0]?.Type != PieceType.Rook))
            return false;

        if (moveInfo.Start.Col - moveInfo.Target.Col != 2 ||
            moveInfo.Start.Row != moveInfo.Target.Row ||
            moveInfo.Target.Col - 2 < 0 || moveInfo.Target.Col - 2 > 8)
            return false;

        if (chessBoard[moveInfo.Start.Row, moveInfo.Start.Col]?.HasMoved != false
            || chessBoard[moveInfo.Target.Row, moveInfo.Target.Col - 2]?.HasMoved != false)
            return false;

        if (chessBoard[moveInfo.Target.Row, moveInfo.Target.Col] != null
            || chessBoard[moveInfo.Target.Row, moveInfo.Target.Col + 1] != null)
            return false;

        if (ChessRules.IsChecked(chessBoard))
            return false;

        PiecePosition? l1 = new PiecePosition { Row = moveInfo.Target.Row, Col = moveInfo.Target.Col - 1 };
        chessBoard[l1] = chessBoard[moveInfo.Start];
        if (ChessRules.IsChecked(chessBoard, l1))
        {
            chessBoard[l1] = null;
            l1 = null;
            return false;
        }
        else
        {
            chessBoard[l1] = null;
            l1 = null;
        }

        PiecePosition? l2 = moveInfo.Target;
        chessBoard[l2] = chessBoard[moveInfo.Start];
        if (ChessRules.IsChecked(chessBoard, l2))
        {
            chessBoard[l2] = null;
            l2 = null;
            return false;
        }
        chessBoard[l2] = null;
        l2 = null;
        return true;
    }
    public static bool IsCastlingRightPossible(ChessBoard chessBoard, MoveInfo moveInfo)
    {
        if (moveInfo is null) return false;
        if (moveInfo.Start is null) return false;
        if (moveInfo.Target is null) return false;

        if (chessBoard[moveInfo.Start]?.Type != PieceType.King ||
            chessBoard[moveInfo.Target.Row, 7]?.Type != PieceType.Rook ||
            moveInfo.Target.Col != 6)
            return false;

        if (chessBoard[moveInfo.Start.Row, moveInfo.Start.Col]?.HasMoved != false
            || chessBoard[moveInfo.Target.Row, 7]?.HasMoved != false)
            return false;

        if (moveInfo.Target.Col - moveInfo.Start.Col != 2 ||
            moveInfo.Start.Row != moveInfo.Target.Row ||
            !ChessRules.IsInside(moveInfo.Target.Col + 1))
            return false;

        if (chessBoard[moveInfo.Target.Row, moveInfo.Target.Col] != null
            || chessBoard[moveInfo.Target.Row, moveInfo.Target.Col - 1] != null)
            return false;

        if (ChessRules.IsChecked(chessBoard))
            return false;

        PiecePosition? r1 = new PiecePosition { Row = moveInfo.Target.Row, Col = moveInfo.Target.Col - 1 };
        chessBoard[r1] = chessBoard[moveInfo.Start];
        if (ChessRules.IsChecked(chessBoard, r1))
        {
            // THIS POSITION IS UNDER CHECK 
            chessBoard[r1] = null; r1 = null; return false;
        }
        else { chessBoard[r1] = null; r1 = null; }

        PiecePosition? r2 = moveInfo.Target;
        chessBoard[r2] = chessBoard[moveInfo.Start];
        if (ChessRules.IsChecked(chessBoard, r2))
        {
            // THIS POSITION IS UNDER CHECK  
            chessBoard[r2] = null; r2 = null; return false;
        }
        else { chessBoard[r2] = null; r2 = null; }
        return true;
    }
    public static bool IsCastlingPossible(ChessBoard chessBoard, MoveInfo moveInfo)
    {
        if (!ValidateCastlingParameters(chessBoard, moveInfo))
            return false;

        return true;
    }
    public static bool IsCastlingPiecesValid(ChessBoard chessBoard, MoveInfo moveInfo)
    {
        if (!ValidateCastlingParameters(chessBoard, moveInfo))
            return false;

        var startPosition = moveInfo.Start!;
        var targetPosition = moveInfo.Target!;

        var rookMove = GetCastlingRookMove(moveInfo);
        var rookStartPosition = rookMove.StartPosition;
        var rookTargetPosition = rookMove.TargetPosition;

        if (chessBoard[startPosition]?.Type != PieceType.King ||
            chessBoard[targetPosition!.Row, rookStartPosition.Col]?.Type != PieceType.Rook ||
            targetPosition.Col != 6)
            return false;

        if (chessBoard[startPosition]?.HasMoved != false
            || chessBoard[targetPosition.Row, 7]?.HasMoved != false)
            return false;

        return true;
    }
}*/
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