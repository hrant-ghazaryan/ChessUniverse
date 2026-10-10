using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public sealed class ChessPuzzleSession
{
    private readonly ChessPuzzle _puzzle;
    private readonly ChessGame _game = new();

    private int _nextSolutionMoveIndex;

    public ChessBoard Board => _game.Board;

    public PieceColor ActiveTurn => _game.ActiveTurn;
    public PieceColor PlayerColor => _puzzle.ActiveTurn;

    public bool IsCompleted =>
        _nextSolutionMoveIndex >= _puzzle.SolutionMoves.Count;

    public int CurrentStep => _nextSolutionMoveIndex;
    private MoveInfo? _pendingPromotionMove;

    public ChessPuzzleSession(ChessPuzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        if (puzzle.SolutionMoves.Count == 0)
            throw new ArgumentException(
                "Puzzle must contain at least one solution move.",
                nameof(puzzle));

        _puzzle = puzzle;

        Restart();
    }

    public void Restart()
    {
        _game.LoadPosition(
            _puzzle.InitialBoard,
            _puzzle.ActiveTurn);

        _nextSolutionMoveIndex = 0;
        _pendingPromotionMove = null;
    }
    public MoveInfo? GetHint()
    {
        if (IsCompleted)
            return null;

        MoveInfo expectedMove =
            _puzzle.SolutionMoves[_nextSolutionMoveIndex];

        return new MoveInfo(expectedMove);
    }
    public PuzzleMoveResult PromotePawn(PieceType promotionType)
    {
        if (_pendingPromotionMove is null)
            return new PuzzleMoveResult(PuzzleMoveStatus.InvalidMove);

        if (_pendingPromotionMove.PromotionType != promotionType)
            return new PuzzleMoveResult(PuzzleMoveStatus.IncorrectMove);

        MoveResult gameMoveResult = _game.PromotePawn(promotionType);

        if (gameMoveResult.MoveType == MoveType.InvalidMove)
            return new PuzzleMoveResult(PuzzleMoveStatus.InvalidMove);

        _pendingPromotionMove = null;
        _nextSolutionMoveIndex++;

        return CompletePlayerMove(gameMoveResult);
    }
    public PuzzleMoveResult TryMove(MoveInfo moveInfo)
    {
        ArgumentNullException.ThrowIfNull(moveInfo);

        if (IsCompleted)
            return new PuzzleMoveResult(PuzzleMoveStatus.Completed);

        if (ActiveTurn != PlayerColor)
            return new PuzzleMoveResult(PuzzleMoveStatus.InvalidMove);

        MoveInfo expectedMove =
            _puzzle.SolutionMoves[_nextSolutionMoveIndex];

        if (!expectedMove.Equals(moveInfo))
            return new PuzzleMoveResult(PuzzleMoveStatus.IncorrectMove);

        MoveResult gameMoveResult = _game.TryMove(moveInfo);

        if (gameMoveResult.MoveType == MoveType.InvalidMove)
            return new PuzzleMoveResult(PuzzleMoveStatus.InvalidMove);

        if (gameMoveResult.MoveType == MoveType.PawnPromotion)
        {
            if (expectedMove.PromotionType is null)
            {
                return new PuzzleMoveResult(
                    PuzzleMoveStatus.InvalidPuzzle,
                    gameMoveResult);
            }

            _pendingPromotionMove = new MoveInfo(expectedMove);

            return new PuzzleMoveResult(
                PuzzleMoveStatus.PromotionRequired,
                gameMoveResult);
        }

        _nextSolutionMoveIndex++;

        return CompletePlayerMove(gameMoveResult);
    }

    private PuzzleMoveResult CompletePlayerMove(MoveResult gameMoveResult)
    {
        List<MoveResult> automaticMoves = [];

        while (!IsCompleted && ActiveTurn != PlayerColor)
        {
            MoveInfo opponentMove =
                _puzzle.SolutionMoves[_nextSolutionMoveIndex];

            MoveResult automaticMove = _game.TryMove(opponentMove);

            if (automaticMove.MoveType == MoveType.InvalidMove ||
                automaticMove.MoveType == MoveType.PawnPromotion)
            {
                return new PuzzleMoveResult(
                    PuzzleMoveStatus.InvalidPuzzle,
                    gameMoveResult,
                    automaticMoves);
            }

            automaticMoves.Add(automaticMove);
            _nextSolutionMoveIndex++;
        }

        PuzzleMoveStatus status = IsCompleted
            ? PuzzleMoveStatus.Completed
            : PuzzleMoveStatus.CorrectMove;

        return new PuzzleMoveResult(
            status,
            gameMoveResult,
            automaticMoves);
    }
}
