using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;

namespace ChessUniverse.Library;

public static class Game
{
    public static void RegularMove(ChessBoard board, MoveInfo moveInfo)
    {
        if (moveInfo.Start is not null 
            && moveInfo.Target is not null)
        {
            Piece? piece = board[moveInfo.Start];
            board[moveInfo.Start] = null;
            board[moveInfo.Target] = piece; // setter-ը ինքն է թարմացնում piece.Position-ը
            piece?.HasMoved = true;
        }
    }
    public static ChessBoard Castling(ChessBoard chessBoard, MoveInfo moveInfo)
    {
        bool canCastle =
            CastlingRules.IsCastlingLeftPossible(chessBoard, moveInfo) ||
            CastlingRules.IsCastlingRightPossible(chessBoard, moveInfo);

        if (!canCastle)
            return chessBoard;

        if (!CastlingRules.ValidateCastlingParameters(chessBoard, moveInfo))
            return chessBoard;

        var startPosition = moveInfo.Start!;
        var targetPosition = moveInfo.Target!;

        var color = chessBoard[startPosition]!.Color;

        var rookMove = CastlingRules.GetCastlingRookMove(moveInfo);

        MoveKingForCastling(chessBoard, startPosition, targetPosition);
        MoveRookForCastling(chessBoard, rookMove);

        moveInfo.Castling = (true, color);

        return chessBoard;
    }
    public static void MoveKingForCastling(ChessBoard chessBoard, 
        PiecePosition startPosition, PiecePosition targetPosition)
    {
        Piece? king = chessBoard[startPosition];
        chessBoard[startPosition] = null;
        chessBoard[targetPosition] = king;
        king?.HasMoved = true;
    }
    public static void MoveRookForCastling(ChessBoard chessBoard, CastlingRookMove rookPositions)
    {
        var rookStartPosition = rookPositions.StartPosition!;
        var rookTargetPosition = rookPositions.TargetPosition!;

        Piece? rook = chessBoard[rookStartPosition];
        chessBoard[rookStartPosition] = null;
        chessBoard[rookTargetPosition] = rook;
        rook?.HasMoved = true;
    }
    public static bool PromotePawn(
    ChessBoard board,
    PiecePosition position,
    PieceType promotionType)
    {
        Piece? pawn = board[position];

        if (pawn is null || pawn.Type != PieceType.Pawn)
            return false;

        bool isOnLastRank =
            (pawn.Color == PieceColor.White && position.Row == 0) ||
            (pawn.Color == PieceColor.Black && position.Row == 7);

        if (!isOnLastRank)
            return false;

        Piece? promotedPiece = promotionType switch
        {
            PieceType.Queen => new Queen(pawn.Color),
            PieceType.Rook => new Rook(pawn.Color),
            PieceType.Bishop => new Bishop(pawn.Color),
            PieceType.Knight => new Knight(pawn.Color),
            _ => null
        };

        if (promotedPiece is null)
            return false;

        promotedPiece.Position = position;
        promotedPiece.HasMoved = true;

        board[position] = promotedPiece;

        return true;
    }
}