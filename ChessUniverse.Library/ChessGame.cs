using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public sealed class ChessGame
{
    public ChessBoard Board { get; private set; } = null!;

    public PieceColor ActiveTurn { get; private set; }

    public MoveInfo? PreviousMove { get; private set; }

    private readonly List<Piece> _capturedPieces = new();

    private readonly Stack<GameSnapshot> _previousSnapshots = new();
    private readonly Stack<GameSnapshot> _nextSnapshots = new();

    private GameSnapshot? PendingPromotionSnapshot { get; set; }

    public bool CanUndo => _previousSnapshots.Count > 0;

    public bool CanRedo => _nextSnapshots.Count > 0;

    public IReadOnlyList<Piece> CapturedPieces =>
        _capturedPieces.AsReadOnly();

    private Piece? PendingPromotionCapturedPiece { get; set; }
    private MoveInfo? PendingPromotionMove { get; set; }

    public bool IsPromotionPending => PendingPromotionMove is not null;

    public ChessGame()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        Board = new ChessBoard();
        Board.SetStartPosition();

        ActiveTurn = PieceColor.White;
        PreviousMove = null;
        PendingPromotionMove = null;
        _capturedPieces.Clear();
        PendingPromotionCapturedPiece = null;

        _previousSnapshots.Clear();
        _nextSnapshots.Clear();
        PendingPromotionSnapshot = null;
    }

    public MoveResult TryMove(MoveInfo moveInfo)
    {
        if (IsPromotionPending)
            return InvalidMoveResult();

        if (moveInfo.Start is null || moveInfo.Target is null)
            return InvalidMoveResult();

        Piece? movingPiece = Board[moveInfo.Start];

        if (movingPiece is null || movingPiece.Color != ActiveTurn)
            return InvalidMoveResult();

        GameSnapshot snapshotBeforeMove = CreateSnapshot();

        ChessBoard boardAfterMove = (ChessBoard)Board.Clone();
        MoveType moveType;

        if (PreviousMove is not null &&
            ChessRules.TryEnPassant(boardAfterMove, moveInfo, PreviousMove))
        {
            moveType = MoveType.EnPassant;
        }
        else
        {
            if (!ChessRules.MoveValidation(
                    Board,
                    moveInfo.Start, moveInfo.Target,
                    ActiveTurn))
            {
                return InvalidMoveResult();
            }

            if (ChessRules.RequiresPawnPromotion(boardAfterMove, moveInfo))
            {
                Game.RegularMove(boardAfterMove, moveInfo);

                if (LeavesOwnKingInCheck(boardAfterMove))
                    return InvalidMoveResult();

                Board = boardAfterMove;
                PendingPromotionMove = new MoveInfo(moveInfo);
                PendingPromotionSnapshot = snapshotBeforeMove;

                return new MoveResult(
                    Board,
                    MoveType.PawnPromotion,
                    BoardState.Ongoing,
                    ActiveTurn);
            }

            if (CastlingRules.IsCastlingLeftPossible(boardAfterMove, moveInfo))
            {
                Game.Castling(boardAfterMove, moveInfo);
                moveType = MoveType.LeftCastling;
            }
            else if (CastlingRules.IsCastlingRightPossible(boardAfterMove, moveInfo))
            {
                Game.Castling(boardAfterMove, moveInfo);
                moveType = MoveType.RightCastling;
            }
            else
            {
                Game.RegularMove(boardAfterMove, moveInfo);
                moveType = MoveType.RegularMove;
            }
        }

        if (LeavesOwnKingInCheck(boardAfterMove))
            return InvalidMoveResult();

        PieceColor nextTurn = ChangeTurn(ActiveTurn);
        BoardState boardState = GetBoardState(boardAfterMove, nextTurn);

        AddCapturedPiece(Board, moveInfo, moveType);
        PendingPromotionCapturedPiece = GetCapturedPiece(Board,
                                            moveInfo, MoveType.PawnPromotion);
        Board = boardAfterMove;
        PreviousMove = new MoveInfo(moveInfo);
        ActiveTurn = nextTurn;

        SaveSnapshot(snapshotBeforeMove);

        return new MoveResult(
            Board,
            moveType,
            boardState,
            ActiveTurn);
    }

    public MoveResult PromotePawn(PieceType promotionType)
    {
        if (PendingPromotionMove?.Target is null)
            return InvalidMoveResult();

        PiecePosition target = PendingPromotionMove.Target;

        if (!Game.PromotePawn(Board, target, promotionType))
            return InvalidMoveResult();

        PieceColor nextTurn = ChangeTurn(ActiveTurn);
        BoardState boardState = GetBoardState(Board, nextTurn);

        if (PendingPromotionCapturedPiece is not null)
            _capturedPieces.Add(
                (Piece)PendingPromotionCapturedPiece.Clone());

        PreviousMove = new MoveInfo(PendingPromotionMove);
        PendingPromotionMove = null;
        PendingPromotionCapturedPiece = null;
        ActiveTurn = nextTurn;

        if (PendingPromotionSnapshot is not null)
            SaveSnapshot(PendingPromotionSnapshot);

        PendingPromotionSnapshot = null;

        return new MoveResult(
            Board,
            MoveType.PawnPromotion,
            boardState,
            ActiveTurn);
    }

    public void RestoreSnapshot(GameSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        Board = (ChessBoard)snapshot.Board.Clone();
        ActiveTurn = snapshot.ActiveTurn;

        PreviousMove = snapshot.PreviousMove is null
            ? null
            : new MoveInfo(snapshot.PreviousMove);

        PendingPromotionMove = null;

        PendingPromotionCapturedPiece = null;
        PendingPromotionSnapshot = null;

        _capturedPieces.Clear();

        foreach (Piece piece in snapshot.CapturedPieces)
            _capturedPieces.Add((Piece)piece.Clone());
    }

    private bool LeavesOwnKingInCheck(ChessBoard board)
    {
        PiecePosition? kingPosition =
            ChessBoard.GetKingPosition(board, ActiveTurn);

        return kingPosition is null ||
               ChessRules.IsChecked(board, kingPosition, ActiveTurn);
    }

    private static BoardState GetBoardState(
        ChessBoard board,
        PieceColor colorToMove)
    {
        if (ChessRules.IsCheckmate(board, colorToMove))
            return BoardState.CheckMate;

        if (ChessRules.IsStaleMate(board, colorToMove))
            return BoardState.StaleMate;

        PiecePosition? kingPosition =
            ChessBoard.GetKingPosition(board, colorToMove);

        if (kingPosition is not null &&
            ChessRules.IsChecked(board, kingPosition, colorToMove))
        {
            return BoardState.Check;
        }

        return BoardState.Ongoing;
    }

    private static PieceColor ChangeTurn(PieceColor currentTurn)
        => currentTurn == PieceColor.White
            ? PieceColor.Black
            : PieceColor.White;
    private MoveResult InvalidMoveResult()
        => new MoveResult(
            Board,
            MoveType.InvalidMove,
            BoardState.InvalidMove,
            ActiveTurn);
    private void AddCapturedPiece(
    ChessBoard boardBeforeMove,
    MoveInfo moveInfo,
    MoveType moveType)
    {
        Piece? capturedPiece = GetCapturedPiece(
            boardBeforeMove,
            moveInfo,
            moveType);

        if (capturedPiece is not null)
            _capturedPieces.Add((Piece)capturedPiece.Clone());
    }
    private static Piece? GetCapturedPiece(
        ChessBoard board,
        MoveInfo moveInfo,
        MoveType moveType)
    {
        if (moveInfo.Start is null || moveInfo.Target is null)
            return null;

        if (moveType == MoveType.EnPassant)
            return board[moveInfo.Start.Row, moveInfo.Target.Col];

        return board[moveInfo.Target];
    }
    public bool Undo()
    {
        if (IsPromotionPending || !CanUndo)
            return false;

        _nextSnapshots.Push(CreateSnapshot());

        GameSnapshot previousSnapshot = _previousSnapshots.Pop();
        RestoreSnapshot(previousSnapshot);

        return true;
    }
    public bool Redo()
    {
        if (IsPromotionPending || !CanRedo)
            return false;

        _previousSnapshots.Push(CreateSnapshot());

        GameSnapshot nextSnapshot = _nextSnapshots.Pop();
        RestoreSnapshot(nextSnapshot);

        return true;
    }
    private GameSnapshot CreateSnapshot()
    {
        return new GameSnapshot(
            Board,
            ActiveTurn,
            PreviousMove,
            CapturedPieces);
    }
    private void SaveSnapshot(GameSnapshot snapshot)
    {
        _previousSnapshots.Push(snapshot);
        _nextSnapshots.Clear();
    }
}