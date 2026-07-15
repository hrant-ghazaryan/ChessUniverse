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
}
