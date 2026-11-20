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

        if (moveInfo is null)
            return InvalidMoveResult();

        // Ամբողջ օրինականության ստուգումը մեկ տեղում է (ChessRules.TrySimulateMove).
        // այստեղ միայն պահպանում ենք արդյունքը խաղի վիճակում։
        if (!ChessRules.TrySimulateMove(
                Board, moveInfo, ActiveTurn, PreviousMove,
                out ChessBoard boardAfterMove, out MoveType moveType))
        {
            return InvalidMoveResult();
        }

        GameSnapshot snapshotBeforeMove = CreateSnapshot();

        // Board-ը դեռ քայլից առաջվա տախտակն է. այստեղից ենք վերցնում վերցված խաղաքարը։
        Piece? capturedPiece = GetCapturedPiece(Board, moveInfo, moveType);

        if (moveType == MoveType.PawnPromotion)
        {
            // Քայլը կիսատ է. սպասում ենք PromotePawn()-ին։ Վերցված խաղաքարը
            // պահում ենք մինչև այդ, որ PromotePawn-ը ավելացնի այն ցուցակում։
            Board = boardAfterMove;
            PendingPromotionMove = new MoveInfo(moveInfo);
            PendingPromotionSnapshot = snapshotBeforeMove;
            PendingPromotionCapturedPiece = capturedPiece;

            return new MoveResult(
                Board,
                MoveType.PawnPromotion,
                BoardState.Ongoing,
                ActiveTurn);
        }

        if (capturedPiece is not null)
            _capturedPieces.Add((Piece)capturedPiece.Clone());

        Board = boardAfterMove;
        PreviousMove = new MoveInfo(moveInfo);
        ActiveTurn = ChangeTurn(ActiveTurn);

        SaveSnapshot(snapshotBeforeMove);

        return new MoveResult(
            Board,
            moveType,
            ChessRules.GetBoardState(Board, ActiveTurn, PreviousMove),
            ActiveTurn);
    }

    // Ընտրված վանդակի խաղաքարի բոլոր օրինական քայլերը (UI-ի համար՝ հուշման կետեր)։
    public IReadOnlyList<PiecePosition> GetLegalMoves(PiecePosition from)
    {
        if (IsPromotionPending)
            return [];

        return ChessRules.GetLegalMoves(Board, from, ActiveTurn, PreviousMove);
    }

    // Ստուգում է՝ արդյոք քայլը մատ կտա, առանց խաղի վիճակը փոխելու։
    // Եթե քայլը փոխակերպում է, օգտագործվում է moveInfo.PromotionType-ը։
    public bool IsCheckmatingMove(MoveInfo moveInfo)
    {
        if (IsPromotionPending ||
            !ChessRules.TrySimulateMove(
                Board, moveInfo, ActiveTurn, PreviousMove,
                out ChessBoard boardAfterMove, out MoveType moveType))
        {
            return false;
        }

        if (moveType == MoveType.PawnPromotion &&
            (moveInfo.PromotionType is null ||
             !Game.PromotePawn(boardAfterMove, moveInfo.Target!, moveInfo.PromotionType.Value)))
        {
            return false;
        }

        return ChessRules.IsCheckmate(boardAfterMove, ChangeTurn(ActiveTurn), moveInfo);
    }

    public MoveResult PromotePawn(PieceType promotionType)
    {
        if (PendingPromotionMove?.Target is null)
            return InvalidMoveResult();

        PiecePosition target = PendingPromotionMove.Target;

        if (!Game.PromotePawn(Board, target, promotionType))
            return InvalidMoveResult();

        PieceColor nextTurn = ChangeTurn(ActiveTurn);
        BoardState boardState = ChessRules.GetBoardState(Board, nextTurn, PendingPromotionMove);

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

    private static PieceColor ChangeTurn(PieceColor currentTurn)
        => ChessRules.Opposite(currentTurn);
    private MoveResult InvalidMoveResult()
        => new MoveResult(
            Board,
            MoveType.InvalidMove,
            BoardState.InvalidMove,
            ActiveTurn);
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
    // Բեռնում է դիրքը FEN տեքստից (կարճ կամ ստանդարտ 6 դաշտանոց ձևով)։
    public void LoadFen(string fen)
    {
        FenPosition position = FenNotation.ParsePosition(fen);
        LoadPosition(position.Board, position.ActiveTurn, position.PreviousMove);
    }

    public void LoadPosition(
    ChessBoard board,
    PieceColor activeTurn,
    MoveInfo? previousMove = null)
    {
        ArgumentNullException.ThrowIfNull(board);

        Board = (ChessBoard)board.Clone();
        ActiveTurn = activeTurn;

        PreviousMove = previousMove is null
            ? null
            : new MoveInfo(previousMove);

        PendingPromotionMove = null;
        PendingPromotionCapturedPiece = null;
        PendingPromotionSnapshot = null;

        _capturedPieces.Clear();
        _previousSnapshots.Clear();
        _nextSnapshots.Clear();
    }
}