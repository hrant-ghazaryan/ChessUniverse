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

        Piece? targetPiece = board[end];

        if (targetPiece?.Type == PieceType.King)
            return false;

        return piece!.CanMove(board, end);
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
    public static bool TryEnPassant(
    ChessBoard board,
    MoveInfo moveInfo,
    MoveInfo previousMove)
    {
        if (board is null ||
            moveInfo?.Start is null ||
            moveInfo.Target is null ||
            previousMove?.Start is null ||
            previousMove.Target is null)
        {
            return false;
        }

        PiecePosition start = moveInfo.Start;
        PiecePosition target = moveInfo.Target;
        PiecePosition previousStart = previousMove.Start;
        PiecePosition previousTarget = previousMove.Target;

        Piece? pawn = board[start];

        if (pawn is null || pawn.Type != PieceType.Pawn)
            return false;

        int direction = pawn.Color == PieceColor.White ? -1 : 1;

        // Զինվորը պետք է մեկ վանդակ անկյունագծով գնա դեպի դատարկ վանդակ։
        if (target.Row != start.Row + direction ||
            Math.Abs(target.Col - start.Col) != 1 ||
            board[target] is not null)
        {
            return false;
        }

        // Վերցվող զինվորը պետք է գտնվի կողքի վանդակում։
        Piece? capturedPawn = board[start.Row, target.Col];

        if (capturedPawn is null ||
            capturedPawn.Type != PieceType.Pawn ||
            capturedPawn.Color == pawn.Color)
        {
            return false;
        }

        // Նախորդ քայլը պարտադիր պետք է լինի հենց այս զինվորի
        // սկզբնական շարքից կատարված երկու վանդականոց քայլը։
        int capturedPawnStartRow =
            capturedPawn.Color == PieceColor.White ? 6 : 1;

        if (previousStart.Row != capturedPawnStartRow ||
            previousStart.Col != target.Col ||
            previousTarget.Row != start.Row ||
            previousTarget.Col != target.Col ||
            Math.Abs(previousTarget.Row - previousStart.Row) != 2)
        {
            return false;
        }

        // Նախ clone-ի վրա ստուգում ենք՝ քայլը սեփական թագավորին
        // շախի տակ չի՞ թողնում։
        ChessBoard boardAfterMove = (ChessBoard)board.Clone();

        Game.RegularMove(boardAfterMove, moveInfo);
        boardAfterMove[start.Row, target.Col] = null;

        PiecePosition? kingPosition =
            ChessBoard.GetKingPosition(boardAfterMove, pawn.Color);

        if (kingPosition is null ||
            ChessRules.IsChecked(boardAfterMove, kingPosition, pawn.Color))
        {
            return false;
        }

        // Միայն բոլոր ստուգումներից հետո փոխում ենք իրական տախտակը։
        Game.RegularMove(board, moveInfo);
        board[start.Row, target.Col] = null;

        return true;
    }


}
