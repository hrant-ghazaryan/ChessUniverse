using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public static class ChessRules
{
    public static bool IsInside(int position)
    {
        if (position >= 0 && position < 8)
            return true;
        return false;
    }
    public static bool IsChecked(ChessBoard chessBoard)
    {
        PiecePosition? BlackKing = ChessBoard.GetKingPosition(chessBoard, PieceColor.Black);
        PiecePosition? WhiteKing = ChessBoard.GetKingPosition(chessBoard, PieceColor.White);

        if (BlackKing is null || WhiteKing is null)
            return false;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var piece = chessBoard[i, j];
                if (piece is null) continue;

                if (piece?.Color == PieceColor.White
                    && piece.IsMovePossible(chessBoard, BlackKing))
                    return true;
                if (piece?.Color == PieceColor.Black
                    && piece.IsMovePossible(chessBoard, WhiteKing))
                    return true;
            }
        }
        return false;
    }
    //checkk
    public static bool IsChecked(ChessBoard chessBoard, PiecePosition activeKingPosition)
    {
        var pieceparam = chessBoard[activeKingPosition];

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var piece = chessBoard[i, j];
                if (piece is not null)
                {
                    if (piece?.Color == PieceColor.White && pieceparam?.Color == PieceColor.Black
                    && piece.IsMovePossible(chessBoard, activeKingPosition))
                        return true;
                    if (piece?.Color == PieceColor.Black && pieceparam?.Color == PieceColor.White
                    && piece.IsMovePossible(chessBoard, activeKingPosition))
                        return true;
                }
            }
        }
        return false;
    }
    // +
    public static bool IsChecked(ChessBoard chessBoard, PiecePosition? activeKingPosition, PieceColor activeTurn)
    {
        if (activeKingPosition is null) return false;
        var pieceparam = chessBoard[activeKingPosition];

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var piece = chessBoard[i, j];
                if (piece is not null)
                {
                    if (piece?.Color != activeTurn
                    && piece!.IsMovePossible(chessBoard, activeKingPosition))
                        return true;
                }
            }
        }
        return false;
    }
    /*public static bool MoveValidation(ChessBoard? board, PiecePosition? start, PiecePosition? end,
         PieceColor turn)
    {
        if (board is null) return false;
        if (start is null || end is null) return false;
        if (board[start] is null) return false;

        Piece? piece = board[start];
        if (turn == piece?.Color) return false;

        Piece? endPiece = board[end];
        if (endPiece == null || piece?.Color != endPiece.Color)
        {
            if (piece?.Type == PieceType.King && piece!.IsMovePossible(board, end) && !IsChecked(board, end))
                return true;
            else if (piece!.IsMovePossible(board, end))
                return true;
        }
        return false;
    }*/
    public static bool MoveValidation(ChessBoard? board, PiecePosition? start, PiecePosition? end,
         PieceColor turn)
    {
        if (board is null) return false;
        if (start is null || end is null) return false;
        if (board[start] is null) return false;

        Piece? piece = board[start];
        if (turn != piece?.Color) return false;

        /*Piece? endPiece = board[end];
        if (endPiece == null || piece?.Color != endPiece.Color)
        {
            if (piece!.IsMovePossible(board, end))
                return true;
        }*/
        if (piece!.IsMovePossible(board, end))
            return true;
        return false;
    }
    public static bool IsStaleMate(ChessBoard board, PieceColor T)
    {
        List<PiecePosition> allTPieces = ChessBoard.GetAllPiecePositions(board, T);
        int pieceCount = allTPieces.Count;
        foreach (var item in allTPieces)
        {
            if (board[item]?.GetPossibleMoves(board).Item2 == true)
                return false;
        }
        return true;
    }
    public static bool IsCastlingLeftPossible(ChessBoard chessBoard, MoveInfo moveInfo)
    {
        if (moveInfo is null) return false;
        if (moveInfo.Start is null) return false;
        if (moveInfo.Target is null) return false;

        if (chessBoard[moveInfo.Start]?.Type != PieceType.King ||
            (IsInside(moveInfo.Target.Col - 2) && chessBoard[moveInfo.Target.Row, 0]?.Type != PieceType.Rook))
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

        if (IsChecked(chessBoard))
            return false;

        PiecePosition? l1 = new PiecePosition { Row = moveInfo.Target.Row, Col = moveInfo.Target.Col - 1 };
        chessBoard[l1] = chessBoard[moveInfo.Start];
        if (IsChecked(chessBoard, l1))
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
        if (IsChecked(chessBoard, l2))
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
            !IsInside(moveInfo.Target.Col + 1))
            return false;

        if (chessBoard[moveInfo.Target.Row, moveInfo.Target.Col] != null
            || chessBoard[moveInfo.Target.Row, moveInfo.Target.Col - 1] != null)
            return false;

        if (IsChecked(chessBoard))
            return false;

        PiecePosition? r1 = new PiecePosition { Row = moveInfo.Target.Row, Col = moveInfo.Target.Col - 1 };
        chessBoard[r1] = chessBoard[moveInfo.Start];
        if (IsChecked(chessBoard, r1))
        {
            // THIS POSITION IS UNDER CHECK 
            chessBoard[r1] = null; r1 = null; return false;
        }
        else { chessBoard[r1] = null; r1 = null; }

        PiecePosition? r2 = moveInfo.Target;
        chessBoard[r2] = chessBoard[moveInfo.Start];
        if (IsChecked(chessBoard, r2))
        {
            // THIS POSITION IS UNDER CHECK  
            chessBoard[r2] = null; r2 = null; return false;
        }
        else { chessBoard[r2] = null; r2 = null; }
        return true;
    }

}
