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
                    && piece.CanMove(chessBoard, BlackKing))
                    return true;
                if (piece?.Color == PieceColor.Black
                    && piece.CanMove(chessBoard, WhiteKing))
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
                    && piece.CanMove(chessBoard, activeKingPosition))
                        return true;
                    if (piece?.Color == PieceColor.Black && pieceparam?.Color == PieceColor.White
                    && piece.CanMove(chessBoard, activeKingPosition))
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
                    && piece!.CanMove(chessBoard, activeKingPosition))
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
        if (piece!.CanMove(board, end))
            return true;
        return false;
    }

    public static bool IsStaleMate(ChessBoard board, PieceColor color)
    {
        PiecePosition? kingPosition = ChessBoard.GetKingPosition(board, color);

        // Շախի մեջ գտնվող կողմը «պատ» չունի. այն կարող է ունենալ checkmate։
        if (kingPosition is null || IsChecked(board, kingPosition, color))
            return false;

        return !HasAnyLegalMove(board, color);
    }

    public static bool IsCheckmate(ChessBoard board, PieceColor color)
    {
        PiecePosition? kingPosition = ChessBoard.GetKingPosition(board, color);

        return kingPosition is not null &&
               IsChecked(board, kingPosition, color) &&
               !HasAnyLegalMove(board, color);
    }

    private static bool HasAnyLegalMove(ChessBoard board, PieceColor color)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var start = new PiecePosition(row, col);
                Piece? piece = board[start];

                if (piece is null || piece.Color != color)
                    continue;

                for (int targetRow = 0; targetRow < 8; targetRow++)
                {
                    for (int targetCol = 0; targetCol < 8; targetCol++)
                    {
                        var target = new PiecePosition(targetRow, targetCol);

                        if (!piece.CanMove(board, target))
                            continue;

                        ChessBoard boardAfterMove = (ChessBoard)board.Clone();
                        Game.RegularMove(boardAfterMove, new MoveInfo(start, target));

                        PiecePosition? kingAfterMove =
                            ChessBoard.GetKingPosition(boardAfterMove, color);

                        if (kingAfterMove is not null &&
                            !IsChecked(boardAfterMove, kingAfterMove, color))
                            return true;
                    }
                }
            }
        }

        return false;
    }
    
}
