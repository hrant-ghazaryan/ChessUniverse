using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public static class CastlingRules
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
}
