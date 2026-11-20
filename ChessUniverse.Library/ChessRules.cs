using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public static class ChessRules
{
    public static bool IsInside(int position)
        => position >= 0 && position < 8;

    public static PieceColor Opposite(PieceColor color)
        => color == PieceColor.White ? PieceColor.Black : PieceColor.White;

    // ------------------------------------------------------------------
    // Հարձակում / շախ
    // ------------------------------------------------------------------

    // Արդյոք byColor գույնի որևէ խաղաքար հարձակվում է square վանդակի վրա։
    // Սա ՔԱՅԼԻ ստուգում չէ. թագավորը հարձակվում է միայն կողքի 8 վանդակների վրա
    // (փոխատեղումը հարձակում չէ), իսկ զինվորը՝ միայն անկյունագծով։
    public static bool IsSquareAttacked(ChessBoard board, PiecePosition square, PieceColor byColor)
    {
        if (!square.IsOnBoard)
            return false;

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Piece? piece = board[row, col];

                if (piece is null || piece.Color != byColor)
                    continue;

                if (Attacks(board, piece, row, col, square))
                    return true;
            }
        }

        return false;
    }

    private static bool Attacks(ChessBoard board, Piece piece, int row, int col, PiecePosition square)
    {
        int dRow = square.Row - row;
        int dCol = square.Col - col;

        switch (piece.Type)
        {
            case PieceType.King:
                return Math.Abs(dRow) <= 1 && Math.Abs(dCol) <= 1 && (dRow != 0 || dCol != 0);

            case PieceType.Pawn:
                int direction = piece.Color == PieceColor.White ? -1 : 1;
                return dRow == direction && Math.Abs(dCol) == 1;

            default:
                // Ձի, փիղ, նավակ, թագուհի. CanMove-ը ճիշտ է նաև դատարկ վանդակի համար։
                // Եթե վանդակում նույն գույնի խաղաքար է, CanMove-ը false է տալիս,
                // բայց այդ դեպքը շախի ստուգման համար էական չէ։
                return piece.CanMove(board, square);
        }
    }

    public static bool IsChecked(ChessBoard chessBoard)
    {
        PiecePosition? blackKing = ChessBoard.GetKingPosition(chessBoard, PieceColor.Black);
        PiecePosition? whiteKing = ChessBoard.GetKingPosition(chessBoard, PieceColor.White);

        if (blackKing is null || whiteKing is null)
            return false;

        return IsSquareAttacked(chessBoard, blackKing, PieceColor.White) ||
               IsSquareAttacked(chessBoard, whiteKing, PieceColor.Black);
    }

    public static bool IsChecked(ChessBoard chessBoard, PiecePosition activeKingPosition)
    {
        Piece? king = chessBoard[activeKingPosition];

        return king is not null &&
               IsSquareAttacked(chessBoard, activeKingPosition, Opposite(king.Color));
    }

    public static bool IsChecked(ChessBoard chessBoard, PiecePosition? activeKingPosition, PieceColor activeTurn)
    {
        if (activeKingPosition is null)
            return false;

        return IsSquareAttacked(chessBoard, activeKingPosition, Opposite(activeTurn));
    }

    // ------------------------------------------------------------------
    // Քայլի օրինականություն. ՄԵԿ ճշմարտության աղբյուր
    // ------------------------------------------------------------------

    // Pseudo-legal ստուգում. խաղաքարը կարո՞ղ է այդպես քայլել
    // (առանց սեփական թագավորի անվտանգության ստուգման և առանց en passant-ի)։
    public static bool MoveValidation(ChessBoard? board, PiecePosition? start, PiecePosition? end,
         PieceColor turn)
    {
        if (board is null) return false;
        if (start is null || end is null) return false;
        if (!start.IsOnBoard || !end.IsOnBoard) return false;

        if (start.Equals(end))
            return false;

        Piece? piece = board[start];
        if (piece is null || turn != piece.Color) return false;

        Piece? targetPiece = board[end];

        if (targetPiece?.Type == PieceType.King)
            return false;

        return piece.CanMove(board, end);
    }

    // Մաքուր (pure) ֆունկցիա. board-ը ՉԻ փոխվում։ Եթե քայլն օրինական է,
    // վերադարձնում է true և քայլից հետո տախտակը (boardAfterMove) ու քայլի տեսակը։
    // Փոխակերպման դեպքում boardAfterMove-ում զինվորը դեռ վերջին շարքում է,
    // իսկ moveType-ը PawnPromotion է։
    public static bool TrySimulateMove(
        ChessBoard board,
        MoveInfo moveInfo,
        PieceColor turn,
        MoveInfo? previousMove,
        out ChessBoard boardAfterMove,
        out MoveType moveType)
    {
        boardAfterMove = board;
        moveType = MoveType.InvalidMove;

        if (board is null || moveInfo?.Start is null || moveInfo.Target is null)
            return false;

        PiecePosition start = moveInfo.Start;
        PiecePosition target = moveInfo.Target;

        if (!start.IsOnBoard || !target.IsOnBoard)
            return false;

        Piece? movingPiece = board[start];

        if (movingPiece is null || movingPiece.Color != turn)
            return false;

        ChessBoard candidate = (ChessBoard)board.Clone();
        MoveType candidateType;

        if (previousMove is not null &&
            TryEnPassant(candidate, moveInfo, previousMove))
        {
            candidateType = MoveType.EnPassant;
        }
        else
        {
            if (!MoveValidation(board, start, target, turn))
                return false;

            if (CastlingRules.IsCastlingLeftPossible(candidate, moveInfo))
            {
                Game.Castling(candidate, new MoveInfo(moveInfo));
                candidateType = MoveType.LeftCastling;
            }
            else if (CastlingRules.IsCastlingRightPossible(candidate, moveInfo))
            {
                Game.Castling(candidate, new MoveInfo(moveInfo));
                candidateType = MoveType.RightCastling;
            }
            else
            {
                candidateType = RequiresPawnPromotion(board, moveInfo)
                    ? MoveType.PawnPromotion
                    : MoveType.RegularMove;

                Game.RegularMove(candidate, moveInfo);
            }
        }

        PiecePosition? kingPosition = ChessBoard.GetKingPosition(candidate, turn);

        if (kingPosition is null || IsChecked(candidate, kingPosition, turn))
            return false;

        boardAfterMove = candidate;
        moveType = candidateType;
        return true;
    }

    public static bool IsLegalMove(
        ChessBoard board,
        MoveInfo moveInfo,
        PieceColor turn,
        MoveInfo? previousMove = null)
        => TrySimulateMove(board, moveInfo, turn, previousMove, out _, out _);

    // Տվյալ վանդակի խաղաքարի բոլոր ՕՐԻՆԱԿԱՆ քայլերը (ներառյալ փոխատեղում և en passant)։
    public static IReadOnlyList<PiecePosition> GetLegalMoves(
        ChessBoard board,
        PiecePosition from,
        PieceColor turn,
        MoveInfo? previousMove = null)
    {
        List<PiecePosition> moves = [];

        if (!from.IsOnBoard || board[from]?.Color != turn)
            return moves;

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var target = new PiecePosition(row, col);

                if (IsLegalMove(board, new MoveInfo(from, target), turn, previousMove))
                    moves.Add(target);
            }
        }

        return moves;
    }

    public static bool HasAnyLegalMove(
        ChessBoard board,
        PieceColor color,
        MoveInfo? previousMove = null)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if (board[row, col]?.Color != color)
                    continue;

                if (GetLegalMoves(board, new PiecePosition(row, col), color, previousMove).Count > 0)
                    return true;
            }
        }

        return false;
    }

    public static bool IsStaleMate(ChessBoard board, PieceColor color, MoveInfo? previousMove = null)
    {
        PiecePosition? kingPosition = ChessBoard.GetKingPosition(board, color);

        // Շախի մեջ գտնվող կողմը «պատ» չունի. այն կարող է ունենալ checkmate։
        if (kingPosition is null || IsChecked(board, kingPosition, color))
            return false;

        return !HasAnyLegalMove(board, color, previousMove);
    }

    public static bool IsCheckmate(ChessBoard board, PieceColor color, MoveInfo? previousMove = null)
    {
        PiecePosition? kingPosition = ChessBoard.GetKingPosition(board, color);

        return kingPosition is not null &&
               IsChecked(board, kingPosition, color) &&
               !HasAnyLegalMove(board, color, previousMove);
    }

    public static BoardState GetBoardState(
        ChessBoard board,
        PieceColor colorToMove,
        MoveInfo? previousMove = null)
    {
        PiecePosition? kingPosition = ChessBoard.GetKingPosition(board, colorToMove);
        bool isInCheck = kingPosition is not null && IsChecked(board, kingPosition, colorToMove);
        bool hasMoves = HasAnyLegalMove(board, colorToMove, previousMove);

        if (isInCheck)
            return hasMoves ? BoardState.Check : BoardState.CheckMate;

        return hasMoves ? BoardState.Ongoing : BoardState.StaleMate;
    }

    // ------------------------------------------------------------------
    // Հատուկ քայլեր
    // ------------------------------------------------------------------

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

        if (!start.IsOnBoard || !target.IsOnBoard)
            return false;

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

    public static bool RequiresPawnPromotion(
    ChessBoard board,
    MoveInfo moveInfo)
    {
        if (moveInfo.Start is null || moveInfo.Target is null)
            return false;

        Piece? piece = board[moveInfo.Start];

        return piece is not null &&
               piece.Type == PieceType.Pawn &&
               (moveInfo.Target.Row == 0 || moveInfo.Target.Row == 7);
    }
}
