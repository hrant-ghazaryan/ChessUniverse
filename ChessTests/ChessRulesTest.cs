using ChessUniverse.Library;
using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;

namespace ChessTests;

public class ChessRulesTest
{
    [Fact]
    public void PuzzleSession_PlaysOpponentMoveAfterPromotion()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        pieces[1, 0] = new Pawn(PieceColor.White)
        {
            Position = new PiecePosition(1, 0)
        };

        pieces[1, 7] = new Pawn(PieceColor.Black)
        {
            Position = new PiecePosition(1, 7)
        };

        pieces[7, 4] = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4)
        };

        pieces[1, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(1, 4)
        };

        pieces[7, 6] = new Knight(PieceColor.White)
        {
            Position = new PiecePosition(7, 6)
        };

        var puzzle = new ChessPuzzle(
            id: 4,
            worldId: 1,
            levelNumber: 4,
            title: "Promotion-ից հետո պատասխան",
            initialBoard: new ChessBoard(pieces),
            activeTurn: PieceColor.White,
            solutionMoves:
            [
                new MoveInfo(
                new PiecePosition(1, 0),
                new PiecePosition(0, 0),
                PieceType.Queen),

            new MoveInfo(
                new PiecePosition(1, 7),
                new PiecePosition(2, 7)),

            new MoveInfo(
                new PiecePosition(7, 6),
                new PiecePosition(5, 5))
            ]);

        var session = new ChessPuzzleSession(puzzle);

        session.TryMove(new MoveInfo(
            new PiecePosition(1, 0),
            new PiecePosition(0, 0)));

        PuzzleMoveResult promotionResult =
            session.PromotePawn(PieceType.Queen);

        Assert.Equal(PuzzleMoveStatus.CorrectMove, promotionResult.Status);
        Assert.Single(promotionResult.AutomaticMoveResults);

        Assert.Equal(2, session.CurrentStep);
        Assert.Equal(PieceColor.White, session.ActiveTurn);

        Assert.IsType<Queen>(session.Board[0, 0]);
        Assert.IsType<Pawn>(session.Board[2, 7]);

        PuzzleMoveResult finalResult = session.TryMove(
            new MoveInfo(
                new PiecePosition(7, 6),
                new PiecePosition(5, 5)));

        Assert.Equal(PuzzleMoveStatus.Completed, finalResult.Status);
        Assert.True(session.IsCompleted);
    }
    [Fact]
    public void PuzzleSession_CompletesPromotion_WhenCorrectPieceIsSelected()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        pieces[1, 0] = new Pawn(PieceColor.White)
        {
            Position = new PiecePosition(1, 0)
        };

        pieces[7, 4] = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4)
        };

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4)
        };

        var puzzle = new ChessPuzzle(
            id: 3,
            worldId: 1,
            levelNumber: 3,
            title: "Փոխակերպում Queen-ի",
            initialBoard: new ChessBoard(pieces),
            activeTurn: PieceColor.White,
            solutionMoves:
            [
                new MoveInfo(
                new PiecePosition(1, 0),
                new PiecePosition(0, 0),
                PieceType.Queen)
            ]);

        var session = new ChessPuzzleSession(puzzle);

        PuzzleMoveResult moveResult = session.TryMove(
            new MoveInfo(
                new PiecePosition(1, 0),
                new PiecePosition(0, 0)));

        Assert.Equal(PuzzleMoveStatus.PromotionRequired, moveResult.Status);
        Assert.IsType<Pawn>(session.Board[0, 0]);

        PuzzleMoveResult wrongPromotion =
            session.PromotePawn(PieceType.Knight);

        Assert.Equal(PuzzleMoveStatus.IncorrectMove, wrongPromotion.Status);
        Assert.IsType<Pawn>(session.Board[0, 0]);

        PuzzleMoveResult promotionResult =
            session.PromotePawn(PieceType.Queen);

        Assert.Equal(PuzzleMoveStatus.Completed, promotionResult.Status);
        Assert.True(session.IsCompleted);

        Assert.IsType<Queen>(session.Board[0, 0]);
        Assert.Equal(PieceColor.Black, session.ActiveTurn);
    }
    [Fact]
    public void PuzzleSession_GetHint_ReturnsNextExpectedMove()
    {
        ChessPuzzle puzzle = PuzzleCatalog.GetPuzzle(1)!;

        var session = new ChessPuzzleSession(puzzle);

        MoveInfo? hint = session.GetHint();

        Assert.NotNull(hint);
        Assert.Equal(new PiecePosition(2, 6), hint.Start);
        Assert.Equal(new PiecePosition(1, 6), hint.Target);

        hint.Target!.Col = 5;

        MoveInfo? nextHint = session.GetHint();

        Assert.NotNull(nextHint);
        Assert.Equal(new PiecePosition(1, 6), nextHint.Target);
    }
    [Fact]
    public void PuzzleSession_Restart_RestoresInitialPosition()
    {
        ChessPuzzle puzzle = PuzzleCatalog.GetPuzzle(1)!;

        var session = new ChessPuzzleSession(puzzle);

        session.TryMove(new MoveInfo(
            new PiecePosition(2, 6),
            new PiecePosition(1, 6)));

        Assert.True(session.IsCompleted);
        Assert.IsType<Queen>(session.Board[1, 6]);

        session.Restart();

        Assert.False(session.IsCompleted);
        Assert.Equal(0, session.CurrentStep);
        Assert.Equal(PieceColor.White, session.ActiveTurn);

        Assert.IsType<Queen>(session.Board[2, 6]);
        Assert.Null(session.Board[1, 6]);
    }
    [Fact]
    public void FenNotation_ParsesAndSerializesActiveTurn()
    {
        const string positionText =
            "7k/8/5KQ1/8/8/8/8/8 b";

        FenPosition position =
            FenNotation.ParsePosition(positionText);

        Assert.Equal(PieceColor.Black, position.ActiveTurn);
        Assert.IsType<King>(position.Board[0, 7]);
        Assert.IsType<Queen>(position.Board[2, 6]);

        string serializedPosition = FenNotation.ToPosition(
            position.Board,
            position.ActiveTurn);

        Assert.Equal(positionText, serializedPosition);
    }
    [Fact]
    public void FenNotation_ParsesAndSerializesBoardPlacement()
    {
        const string boardPlacement =
            "7k/8/5KQ1/8/8/8/8/8";

        ChessBoard board =
            FenNotation.ParseBoardPlacement(boardPlacement);

        Piece? blackKing = board[0, 7];
        Piece? whiteKing = board[2, 5];
        Piece? whiteQueen = board[2, 6];

        Assert.IsType<King>(blackKing);
        Assert.Equal(PieceColor.Black, blackKing.Color);

        Assert.IsType<King>(whiteKing);
        Assert.Equal(PieceColor.White, whiteKing.Color);

        Assert.IsType<Queen>(whiteQueen);
        Assert.Equal(PieceColor.White, whiteQueen.Color);

        string serializedBoardPlacement =
            FenNotation.ToBoardPlacement(board);

        Assert.Equal(boardPlacement, serializedBoardPlacement);
    }
    [Fact]
    public void PuzzleCatalog_ReturnsPlayableMateInOnePuzzle()
    {
        ChessWorld? world = PuzzleCatalog.GetWorld(1);
        ChessPuzzle? puzzle = PuzzleCatalog.GetPuzzle(1);

        Assert.NotNull(world);
        Assert.NotNull(puzzle);

        Assert.Equal("Սկսնակ աշխարհ", world.Title);
        Assert.Equal("Մատ մեկ քայլում", puzzle.Title);

        var session = new ChessPuzzleSession(puzzle);

        PuzzleMoveResult result = session.TryMove(
            new MoveInfo(
                new PiecePosition(2, 6),
                new PiecePosition(1, 6)));

        Assert.Equal(PuzzleMoveStatus.Completed, result.Status);
        Assert.True(session.IsCompleted);

        Assert.NotNull(result.GameMoveResult);
        Assert.Equal(
            BoardState.CheckMate,
            result.GameMoveResult.BoardState);
    }
    [Fact]
    public void ChessWorld_SortsPuzzlesByLevelNumber()
    {
        ChessBoard board = new ChessBoard();
        board.SetStartPosition();

        MoveInfo solutionMove = new MoveInfo(
            new PiecePosition(6, 4),
            new PiecePosition(4, 4));

        var secondLevel = new ChessPuzzle(
            id: 2,
            worldId: 1,
            levelNumber: 2,
            title: "Երկրորդ մակարդակ",
            initialBoard: board,
            activeTurn: PieceColor.White,
            solutionMoves: [solutionMove]);

        var firstLevel = new ChessPuzzle(
            id: 1,
            worldId: 1,
            levelNumber: 1,
            title: "Առաջին մակարդակ",
            initialBoard: board,
            activeTurn: PieceColor.White,
            solutionMoves: [solutionMove]);

        var world = new ChessWorld(
            id: 1,
            title: "Սկսնակ աշխարհ",
            puzzles: [secondLevel, firstLevel]);

        Assert.Equal(2, world.Puzzles.Count);
        Assert.Equal(1, world.Puzzles[0].LevelNumber);
        Assert.Equal(2, world.Puzzles[1].LevelNumber);
    }
    [Fact]
    public void PuzzleSession_PlaysOpponentMoveAutomatically()
    {
        ChessBoard board = new ChessBoard();
        board.SetStartPosition();

        var puzzle = new ChessPuzzle(
            id: 2,
            worldId: 1,
            levelNumber: 2,
            title: "Երկքայլանի խնդիր",
            initialBoard: board,
            activeTurn: PieceColor.White,
            solutionMoves:
            [
                new MoveInfo(
                new PiecePosition(6, 4),
                new PiecePosition(4, 4)), // e2 դեպի e4

            new MoveInfo(
                new PiecePosition(1, 4),
                new PiecePosition(3, 4)), // e7 դեպի e5

            new MoveInfo(
                new PiecePosition(7, 6),
                new PiecePosition(5, 5))  // g1 դեպի f3
            ]);

        var session = new ChessPuzzleSession(puzzle);

        PuzzleMoveResult firstResult = session.TryMove(
            new MoveInfo(
                new PiecePosition(6, 4),
                new PiecePosition(4, 4)));

        Assert.Equal(PuzzleMoveStatus.CorrectMove, firstResult.Status);
        Assert.Single(firstResult.AutomaticMoveResults);

        Assert.Equal(2, session.CurrentStep);
        Assert.Equal(PieceColor.White, session.ActiveTurn);

        Assert.IsType<Pawn>(session.Board[4, 4]);
        Assert.IsType<Pawn>(session.Board[3, 4]);

        PuzzleMoveResult secondResult = session.TryMove(
            new MoveInfo(
                new PiecePosition(7, 6),
                new PiecePosition(5, 5)));

        Assert.Equal(PuzzleMoveStatus.Completed, secondResult.Status);
        Assert.True(session.IsCompleted);

        Assert.IsType<Knight>(session.Board[5, 5]);
        Assert.Null(session.Board[7, 6]);
    }
    [Fact]
    public void PuzzleSession_RejectsIncorrectMove_WithoutChangingBoard()
    {
        ChessBoard board = new ChessBoard();
        board.SetStartPosition();

        var puzzle = new ChessPuzzle(
            id: 1,
            worldId: 1,
            levelNumber: 1,
            title: "Առաջին քայլ",
            initialBoard: board,
            activeTurn: PieceColor.White,
            solutionMoves:
            [
                new MoveInfo(
                new PiecePosition(6, 4),
                new PiecePosition(4, 4))
            ]);

        var session = new ChessPuzzleSession(puzzle);

        PuzzleMoveResult result = session.TryMove(
            new MoveInfo(
                new PiecePosition(6, 3),
                new PiecePosition(4, 3)));

        Assert.Equal(PuzzleMoveStatus.IncorrectMove, result.Status);
        Assert.False(session.IsCompleted);
        Assert.Equal(0, session.CurrentStep);

        Assert.IsType<Pawn>(session.Board[6, 3]);
        Assert.Null(session.Board[4, 3]);
    }
    [Fact]
    public void PuzzleSession_Completes_WhenPlayerMakesExpectedMove()
    {
        ChessBoard board = new ChessBoard();
        board.SetStartPosition();

        var puzzle = new ChessPuzzle(
            id: 1,
            worldId: 1,
            levelNumber: 1,
            title: "Առաջին քայլ",
            initialBoard: board,
            activeTurn: PieceColor.White,
            solutionMoves:
            [
                new MoveInfo(
                new PiecePosition(6, 4),
                new PiecePosition(4, 4))
            ]);

        var session = new ChessPuzzleSession(puzzle);

        PuzzleMoveResult result = session.TryMove(
            new MoveInfo(
                new PiecePosition(6, 4),
                new PiecePosition(4, 4)));

        Assert.Equal(PuzzleMoveStatus.Completed, result.Status);
        Assert.True(session.IsCompleted);
        Assert.Equal(1, session.CurrentStep);

        Assert.Null(session.Board[6, 4]);
        Assert.IsType<Pawn>(session.Board[4, 4]);
    }
    [Fact]
    public void MoveInfo_AreEqual_WhenStartAndTargetAreEqual()
    {
        MoveInfo firstMove = new(
            new PiecePosition(6, 4),
            new PiecePosition(4, 4),
            PieceColor.White);

        MoveInfo secondMove = new(
            new PiecePosition(6, 4),
            new PiecePosition(4, 4),
            PieceColor.Black);

        Assert.Equal(firstMove, secondMove);
        Assert.Equal(firstMove.GetHashCode(), secondMove.GetHashCode());
    }
    [Fact]
    public void ChessPuzzle_CreatesIndependentCopiesOfBoardAndSolution()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        pieces[7, 4] = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4)
        };

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4)
        };

        pieces[4, 3] = new Queen(PieceColor.White)
        {
            Position = new PiecePosition(4, 3)
        };

        ChessBoard initialBoard = new ChessBoard(pieces);

        MoveInfo solutionMove = new MoveInfo(
            new PiecePosition(4, 3),
            new PiecePosition(0, 3));

        var puzzle = new ChessPuzzle(
            id: 1,
            worldId: 1,
            levelNumber: 1,
            title: "Մատ մեկ քայլում",
            initialBoard: initialBoard,
            activeTurn: PieceColor.White,
            solutionMoves: [solutionMove]);

        initialBoard[4, 3] = null;
        solutionMove.Target!.Col = 2;

        Assert.Equal(1, puzzle.Id);
        Assert.Equal(1, puzzle.WorldId);
        Assert.Equal(1, puzzle.LevelNumber);
        Assert.Equal(PieceColor.White, puzzle.ActiveTurn);

        Assert.IsType<Queen>(puzzle.InitialBoard[4, 3]);

        MoveInfo savedMove = Assert.Single(puzzle.SolutionMoves);

        Assert.Equal(new PiecePosition(4, 3), savedMove.Start);
        Assert.Equal(new PiecePosition(0, 3), savedMove.Target);
    }
    [Fact]
    public void LoadPosition_LoadsIndependentBoardAndResetsHistory()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        pieces[7, 4] = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4)
        };

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4)
        };

        pieces[4, 3] = new Queen(PieceColor.White)
        {
            Position = new PiecePosition(4, 3)
        };

        ChessBoard puzzleBoard = new ChessBoard(pieces);

        var game = new ChessGame();

        game.TryMove(new MoveInfo(
            new PiecePosition(6, 4),
            new PiecePosition(4, 4)));

        Assert.True(game.CanUndo);

        game.LoadPosition(puzzleBoard, PieceColor.Black);

        Assert.Equal(PieceColor.Black, game.ActiveTurn);
        Assert.IsType<Queen>(game.Board[4, 3]);
        Assert.False(game.CanUndo);
        Assert.False(game.CanRedo);
        Assert.Empty(game.CapturedPieces);

        puzzleBoard[4, 3] = null;

        Assert.IsType<Queen>(game.Board[4, 3]);
    }
    [Fact]
    public void UndoAndRedo_RestorePawnPromotion()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        pieces[1, 0] = new Pawn(PieceColor.White)
        {
            Position = new PiecePosition(1, 0)
        };

        pieces[7, 4] = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4)
        };

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4)
        };

        var game = new ChessGame();

        game.RestoreSnapshot(new GameSnapshot(
            new ChessBoard(pieces),
            PieceColor.White,
            null,
            Array.Empty<Piece>()));

        game.TryMove(new MoveInfo(
            new PiecePosition(1, 0),
            new PiecePosition(0, 0)));

        game.PromotePawn(PieceType.Queen);

        Assert.IsType<Queen>(game.Board[0, 0]);
        Assert.Equal(PieceColor.Black, game.ActiveTurn);

        bool undoSucceeded = game.Undo();

        Assert.True(undoSucceeded);
        Assert.IsType<Pawn>(game.Board[1, 0]);
        Assert.Null(game.Board[0, 0]);
        Assert.Equal(PieceColor.White, game.ActiveTurn);

        bool redoSucceeded = game.Redo();

        Assert.True(redoSucceeded);
        Assert.IsType<Queen>(game.Board[0, 0]);
        Assert.Equal(PieceColor.Black, game.ActiveTurn);
    }
    [Fact]
    public void UndoAndRedo_RestoreCapturedPieces()
    {
        var game = new ChessGame();

        game.TryMove(new MoveInfo(
            new PiecePosition(6, 4),
            new PiecePosition(4, 4)));

        game.TryMove(new MoveInfo(
            new PiecePosition(1, 3),
            new PiecePosition(3, 3)));

        game.TryMove(new MoveInfo(
            new PiecePosition(4, 4),
            new PiecePosition(3, 3)));

        Assert.Single(game.CapturedPieces);
        Assert.IsType<Pawn>(game.CapturedPieces[0]);
        Assert.Equal(PieceColor.Black, game.CapturedPieces[0].Color);

        bool undoSucceeded = game.Undo();

        Assert.True(undoSucceeded);
        Assert.Empty(game.CapturedPieces);

        Assert.IsType<Pawn>(game.Board[4, 4]);
        Assert.IsType<Pawn>(game.Board[3, 3]);
        Assert.Equal(PieceColor.White, game.ActiveTurn);

        bool redoSucceeded = game.Redo();

        Assert.True(redoSucceeded);
        Assert.Single(game.CapturedPieces);

        Assert.IsType<Pawn>(game.Board[3, 3]);
        Assert.Equal(PieceColor.White, game.Board[3, 3]!.Color);
        Assert.Null(game.Board[4, 4]);
        Assert.Equal(PieceColor.Black, game.ActiveTurn);
    }
    [Fact]
    public void TryMove_ClearsRedoHistory_AfterUndo()
    {
        var game = new ChessGame();

        game.TryMove(new MoveInfo(
            new PiecePosition(6, 4),
            new PiecePosition(4, 4)));

        game.TryMove(new MoveInfo(
            new PiecePosition(1, 4),
            new PiecePosition(3, 4)));

        game.Undo();

        game.TryMove(new MoveInfo(
            new PiecePosition(1, 3),
            new PiecePosition(2, 3)));

        Assert.False(game.CanRedo);
        Assert.IsType<Pawn>(game.Board[2, 3]);
    }
    [Fact]
    public void UndoAndRedo_RestoreBoardAndTurn()
    {
        var game = new ChessGame();

        game.TryMove(new MoveInfo(
            new PiecePosition(6, 4),
            new PiecePosition(4, 4)));

        game.TryMove(new MoveInfo(
            new PiecePosition(1, 4),
            new PiecePosition(3, 4)));

        bool undoSucceeded = game.Undo();

        Assert.True(undoSucceeded);
        Assert.True(game.CanRedo);
        Assert.Equal(PieceColor.Black, game.ActiveTurn);

        Assert.IsType<Pawn>(game.Board[4, 4]);
        Assert.IsType<Pawn>(game.Board[1, 4]);
        Assert.Null(game.Board[3, 4]);

        bool redoSucceeded = game.Redo();

        Assert.True(redoSucceeded);
        Assert.Equal(PieceColor.White, game.ActiveTurn);

        Assert.IsType<Pawn>(game.Board[4, 4]);
        Assert.IsType<Pawn>(game.Board[3, 4]);
        Assert.Null(game.Board[1, 4]);
    }
    [Fact]
    public void IsCheckedTest()
    {
        Piece[,] checkBoard = new Piece[8, 8];
        checkBoard[0, 4] = new King(PieceColor.Black) { Position = new PiecePosition { Row = 0, Col = 4 } };
        checkBoard[5, 4] = new Rook(PieceColor.White) { Position = new PiecePosition { Row = 5, Col = 4 } };
        checkBoard[7, 4] = new King(PieceColor.White) { Position = new PiecePosition { Row = 7, Col = 4 } };
        ChessBoard board = new ChessBoard(checkBoard);


        var checkChecker = ChessRules.IsChecked(board, new PiecePosition(0, 4));
        Assert.True(checkChecker);
    }
    [Fact]
    public void IsStaleMateTest()
    {
        Piece[,] stalemateBoard = new Piece[8, 8];
        stalemateBoard[0, 7] = new King(PieceColor.Black) { Position = new PiecePosition { Row = 0, Col = 7 } };
        stalemateBoard[1, 5] = new Queen(PieceColor.White) { Position = new PiecePosition { Row = 1, Col = 5 } };
        stalemateBoard[2, 6] = new King(PieceColor.White) { Position = new PiecePosition { Row = 2, Col = 6 } };
        ChessBoard board = new ChessBoard(stalemateBoard);

        var isStalemateChecker = ChessRules.IsStaleMate(board, PieceColor.Black);
        Assert.True(isStalemateChecker);

    }
    public static IEnumerable<object[]> MoveTestCases => new List<object[]>
    {
        new object[]{ new PiecePosition(0,0) , new PiecePosition(0,3) , PieceColor.Black },
        new object[]{ new PiecePosition(0,1) , new PiecePosition(2,2) , PieceColor.Black },
        new object[]{ new PiecePosition(0,2) , new PiecePosition(3,5) , PieceColor.Black },
        new object[]{ new PiecePosition(0,3) , new PiecePosition(5,3) , PieceColor.Black },
        new object[]{ new PiecePosition(0,4) , new PiecePosition(0,5) , PieceColor.Black },
        new object[]{ new PiecePosition(1,0) , new PiecePosition(3,0) , PieceColor.Black }
    };
    [Fact]
    public void IsCheckmate_ReturnsTrue_WhenKingIsCheckedAndHasNoLegalMoves()
    {
        Piece?[,] checkmateBoard = new Piece?[8, 8];

        checkmateBoard[0, 7] = new King(PieceColor.Black) { Position = new PiecePosition(0, 7) };
        checkmateBoard[1, 6] = new Queen(PieceColor.White) { Position = new PiecePosition(1, 6) };
        checkmateBoard[2, 6] = new King(PieceColor.White) { Position = new PiecePosition(2, 6) };

        ChessBoard board = new ChessBoard(checkmateBoard);

        bool isCheckmate = ChessRules.IsCheckmate(board, PieceColor.Black);

        Assert.True(isCheckmate);
    }
    [Fact]
    public void IsCastlingLeftPossible_ReturnsTrue_WhenAllRulesAreSatisfied()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        pieces[7, 4] = new King(PieceColor.White) { Position = new PiecePosition(7, 4) };
        pieces[7, 0] = new Rook(PieceColor.White) { Position = new PiecePosition(7, 0) };
        pieces[0, 4] = new King(PieceColor.Black) { Position = new PiecePosition(0, 4) };

        ChessBoard board = new ChessBoard(pieces);

        bool canCastle = CastlingRules.IsCastlingLeftPossible(
            board, new MoveInfo(new PiecePosition(7, 4), new PiecePosition(7, 2)));

        Assert.True(canCastle);
    }
    [Fact]
    public void IsCastlingLeftPossible_ReturnsFalse_WhenBSquareIsOccupied()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        pieces[7, 4] = new King(PieceColor.White) { Position = new PiecePosition(7, 4) };
        pieces[7, 0] = new Rook(PieceColor.White) { Position = new PiecePosition(7, 0) };
        pieces[7, 1] = new Knight(PieceColor.White) { Position = new PiecePosition(7, 1) };
        pieces[0, 4] = new King(PieceColor.Black) { Position = new PiecePosition(0, 4) };

        ChessBoard board = new ChessBoard(pieces);

        bool canCastle = CastlingRules.IsCastlingLeftPossible(
            board, new MoveInfo(new PiecePosition(7, 4), new PiecePosition(7, 2)));

        Assert.False(canCastle);
    }
    [Fact]
    public void IsCastlingLeftPossible_ReturnsFalse_WhenKingPassesThroughCheck()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        pieces[7, 4] = new King(PieceColor.White) { Position = new PiecePosition(7, 4) };
        pieces[7, 0] = new Rook(PieceColor.White) { Position = new PiecePosition(7, 0) };
        pieces[0, 4] = new King(PieceColor.Black) { Position = new PiecePosition(0, 4) };
        pieces[0, 3] = new Rook(PieceColor.Black) { Position = new PiecePosition(0, 3) };

        ChessBoard board = new ChessBoard(pieces);

        bool canCastle = CastlingRules.IsCastlingLeftPossible(
            board, new MoveInfo(new PiecePosition(7, 4), new PiecePosition(7, 2)));

        Assert.False(canCastle);
    }
    [Fact]
    public void Castling_MovesWhiteKingAndRook_ForKingSideCastling()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        King whiteKing = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4)
        };

        Rook whiteRook = new Rook(PieceColor.White)
        {
            Position = new PiecePosition(7, 7)
        };

        pieces[7, 4] = whiteKing;
        pieces[7, 7] = whiteRook;

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4)
        };

        ChessBoard board = new ChessBoard(pieces);
        MoveInfo moveInfo = new MoveInfo(
            new PiecePosition(7, 4),
            new PiecePosition(7, 6));

        ChessBoard result = Game.Castling(board, moveInfo);

        Assert.Same(whiteKing, result[7, 6]);
        Assert.Same(whiteRook, result[7, 5]);

        Assert.Null(result[7, 4]);
        Assert.Null(result[7, 7]);

        Assert.True(whiteKing.HasMoved);
        Assert.True(whiteRook.HasMoved);
        Assert.Equal(new PiecePosition(7, 6), whiteKing.Position);
        Assert.Equal(new PiecePosition(7, 5), whiteRook.Position);
    }
    [Fact]
    public void Castling_MovesWhiteKingAndRook_ForQueenSideCastling()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        King whiteKing = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4)
        };

        Rook whiteRook = new Rook(PieceColor.White)
        {
            Position = new PiecePosition(7, 0)
        };

        pieces[7, 4] = whiteKing;
        pieces[7, 0] = whiteRook;

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4)
        };

        ChessBoard board = new ChessBoard(pieces);
        MoveInfo moveInfo = new MoveInfo(
            new PiecePosition(7, 4),
            new PiecePosition(7, 2));

        ChessBoard result = Game.Castling(board, moveInfo);

        Assert.Same(whiteKing, result[7, 2]);
        Assert.Same(whiteRook, result[7, 3]);

        Assert.Null(result[7, 4]);
        Assert.Null(result[7, 0]);

        Assert.True(whiteKing.HasMoved);
        Assert.True(whiteRook.HasMoved);
        Assert.Equal(new PiecePosition(7, 2), whiteKing.Position);
        Assert.Equal(new PiecePosition(7, 3), whiteRook.Position);
    }
    [Fact]
    public void PromotePawn_ReplacesWhitePawnWithQueen_OnLastRank()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        pieces[0, 0] = new Pawn(PieceColor.White)
        {
            Position = new PiecePosition(0, 0)
        };

        pieces[7, 4] = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4)
        };

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4)
        };

        ChessBoard board = new ChessBoard(pieces);

        bool promoted = Game.PromotePawn(
            board,
            new PiecePosition(0, 0),
            PieceType.Queen);

        Piece? promotedPiece = board[0, 0];

        Assert.True(promoted);
        Assert.NotNull(promotedPiece);
        Assert.IsType<Queen>(promotedPiece);
        Assert.Equal(PieceColor.White, promotedPiece.Color);
        Assert.Equal(new PiecePosition(0, 0), promotedPiece.Position);
        Assert.True(promotedPiece.HasMoved);
    }
    [Fact]
    public void PromotePawn_ReplacesBlackPawnWithKnight_OnLastRank()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        pieces[7, 7] = new Pawn(PieceColor.Black)
        {
            Position = new PiecePosition(7, 7)
        };

        pieces[7, 4] = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4)
        };

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4)
        };

        ChessBoard board = new ChessBoard(pieces);

        bool promoted = Game.PromotePawn(
            board,
            new PiecePosition(7, 7),
            PieceType.Knight);

        Piece? promotedPiece = board[7, 7];

        Assert.True(promoted);
        Assert.NotNull(promotedPiece);
        Assert.IsType<Knight>(promotedPiece);
        Assert.Equal(PieceColor.Black, promotedPiece.Color);
        Assert.Equal(new PiecePosition(7, 7), promotedPiece.Position);
        Assert.True(promotedPiece.HasMoved);
    }
    [Fact]
    public void PromotePawn_ReturnsFalse_WhenPawnIsNotOnLastRank()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        Pawn whitePawn = new Pawn(PieceColor.White)
        {
            Position = new PiecePosition(3, 3)
        };

        pieces[3, 3] = whitePawn;

        ChessBoard board = new ChessBoard(pieces);

        bool promoted = Game.PromotePawn(
            board,
            new PiecePosition(3, 3),
            PieceType.Queen);

        Assert.False(promoted);
        Assert.Same(whitePawn, board[3, 3]);
    }
    [Fact]
    public void TryEnPassant_ReturnsFalse_WhenPreviousPawnMovedOnlyOneSquare()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        Pawn whitePawn = new Pawn(PieceColor.White)
        {
            Position = new PiecePosition(3, 4)
        };

        Pawn blackPawn = new Pawn(PieceColor.Black)
        {
            Position = new PiecePosition(3, 3)
        };

        pieces[3, 4] = whitePawn;
        pieces[3, 3] = blackPawn;

        pieces[7, 4] = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4)
        };

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4)
        };

        ChessBoard board = new ChessBoard(pieces);

        MoveInfo previousMove = new MoveInfo(
            new PiecePosition(2, 3), // d6
            new PiecePosition(3, 3)); // d5

        MoveInfo enPassantMove = new MoveInfo(
            new PiecePosition(3, 4), // e5
            new PiecePosition(2, 3)); // d6

        bool wasCaptured = ChessRules.TryEnPassant(
            board,
            enPassantMove,
            previousMove);

        Assert.False(wasCaptured);
        Assert.Same(whitePawn, board[3, 4]);
        Assert.Same(blackPawn, board[3, 3]);
        Assert.Null(board[2, 3]);
    }
    [Fact]
    public void TryEnPassant_CapturesBlackPawn_AfterTwoSquarePawnMove()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        Pawn whitePawn = new Pawn(PieceColor.White)
        {
            Position = new PiecePosition(3, 4) // e5
        };

        Pawn blackPawn = new Pawn(PieceColor.Black)
        {
            Position = new PiecePosition(3, 3) // d5
        };

        pieces[3, 4] = whitePawn;
        pieces[3, 3] = blackPawn;

        pieces[7, 4] = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4)
        };

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4)
        };

        ChessBoard board = new ChessBoard(pieces);

        MoveInfo previousMove = new MoveInfo(
            new PiecePosition(1, 3), // d7
            new PiecePosition(3, 3)); // d5

        MoveInfo enPassantMove = new MoveInfo(
            new PiecePosition(3, 4), // e5
            new PiecePosition(2, 3)); // d6

        bool wasCaptured = ChessRules.TryEnPassant(
            board,
            enPassantMove,
            previousMove);

        Assert.True(wasCaptured);
        Assert.Same(whitePawn, board[2, 3]);
        Assert.Null(board[3, 4]);
        Assert.Null(board[3, 3]);
        Assert.True(whitePawn.HasMoved);
        Assert.Equal(new PiecePosition(2, 3), whitePawn.Position);
    }
    [Fact]
    public void TryEnPassant_CapturesWhitePawn_ForBlackPawn()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        Pawn blackPawn = new Pawn(PieceColor.Black)
        {
            Position = new PiecePosition(4, 5) // f4
        };

        Pawn whitePawn = new Pawn(PieceColor.White)
        {
            Position = new PiecePosition(4, 4) // e4
        };

        pieces[4, 5] = blackPawn;
        pieces[4, 4] = whitePawn;

        pieces[7, 4] = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4)
        };

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4)
        };

        ChessBoard board = new ChessBoard(pieces);

        MoveInfo previousMove = new MoveInfo(
            new PiecePosition(6, 4), // e2
            new PiecePosition(4, 4)); // e4

        MoveInfo enPassantMove = new MoveInfo(
            new PiecePosition(4, 5), // f4
            new PiecePosition(5, 4)); // e3

        bool wasCaptured = ChessRules.TryEnPassant(
            board,
            enPassantMove,
            previousMove);

        Assert.True(wasCaptured);
        Assert.Same(blackPawn, board[5, 4]);
        Assert.Null(board[4, 5]);
        Assert.Null(board[4, 4]);
    }
    [Fact]
    public void TryEnPassant_ReturnsFalse_WhenMoveExposesOwnKingToCheck()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        Pawn whitePawn = new Pawn(PieceColor.White)
        {
            Position = new PiecePosition(3, 4) // e5
        };

        Pawn blackPawn = new Pawn(PieceColor.Black)
        {
            Position = new PiecePosition(3, 3) // d5
        };

        pieces[3, 4] = whitePawn;
        pieces[3, 3] = blackPawn;

        pieces[7, 4] = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4) // e1
        };

        pieces[0, 0] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 0)
        };

        pieces[0, 4] = new Rook(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4) // e8
        };

        ChessBoard board = new ChessBoard(pieces);

        MoveInfo previousMove = new MoveInfo(
            new PiecePosition(1, 3), // d7
            new PiecePosition(3, 3)); // d5

        MoveInfo enPassantMove = new MoveInfo(
            new PiecePosition(3, 4), // e5
            new PiecePosition(2, 3)); // d6

        bool wasCaptured = ChessRules.TryEnPassant(
            board,
            enPassantMove,
            previousMove);

        Assert.False(wasCaptured);
        Assert.Same(whitePawn, board[3, 4]);
        Assert.Same(blackPawn, board[3, 3]);
    }
    [Fact]
    public void Castling_DoesNotMoveKing_WhenCastlingIsIllegal()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        King whiteKing = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4) // e1
        };

        pieces[7, 4] = whiteKing;

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4) // e8
        };

        ChessBoard board = new ChessBoard(pieces);

        MoveInfo moveInfo = new MoveInfo(
            new PiecePosition(7, 4), // e1
            new PiecePosition(7, 6)); // g1

        ChessBoard result = Game.Castling(board, moveInfo);

        Assert.Same(board, result);
        Assert.Same(whiteKing, result[7, 4]);
        Assert.Null(result[7, 6]);
        Assert.False(whiteKing.HasMoved);
    }
    [Fact]
    public void MoveInfoCopy_CreatesIndependentStartAndTargetPositions()
    {
        MoveInfo original = new MoveInfo(
            new PiecePosition(6, 4), // e2
            new PiecePosition(4, 4), // e4
            PieceColor.White);

        MoveInfo copy = new MoveInfo(original);

        original.Start!.Row = 5;
        original.Target!.Col = 3;

        Assert.Equal(new PiecePosition(6, 4), copy.Start);
        Assert.Equal(new PiecePosition(4, 4), copy.Target);

        Assert.NotSame(original.Start, copy.Start);
        Assert.NotSame(original.Target, copy.Target);

        Assert.Equal(PieceColor.White, copy.Turn);
    }
    [Fact]
    public void Pawn_CannotMoveTwoSquares_WhenItHasMovedBefore()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        Pawn whitePawn = new Pawn(PieceColor.White)
        {
            Position = new PiecePosition(6, 4), // e2
            HasMoved = true
        };

        pieces[6, 4] = whitePawn;

        ChessBoard board = new ChessBoard(pieces);

        bool canMoveTwoSquares = whitePawn.CanMove(
            board,
            new PiecePosition(4, 4)); // e4

        Assert.False(canMoveTwoSquares);
    }
    [Fact]
    public void Pawn_CannotMoveTwoSquares_WhenBlackPawnHasMovedBefore()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        Pawn blackPawn = new Pawn(PieceColor.Black)
        {
            Position = new PiecePosition(1, 4), // e7
            HasMoved = true
        };

        pieces[1, 4] = blackPawn;

        ChessBoard board = new ChessBoard(pieces);

        bool canMoveTwoSquares = blackPawn.CanMove(
            board,
            new PiecePosition(3, 4)); // e5

        Assert.False(canMoveTwoSquares);
    }
    [Fact]
    public void WhitePawn_CannotMoveBackward()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        Pawn pawn = new Pawn(PieceColor.White)
        {
            Position = new PiecePosition(4, 4)
        };

        pieces[4, 4] = pawn;

        ChessBoard board = new ChessBoard(pieces);

        Assert.False(pawn.CanMove(board, new PiecePosition(5, 4)));
    }
    [Fact]
    public void BlackPawn_CannotMoveBackward()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        Pawn pawn = new Pawn(PieceColor.Black)
        {
            Position = new PiecePosition(3, 3)
        };

        pieces[3, 3] = pawn;

        ChessBoard board = new ChessBoard(pieces);

        Assert.False(pawn.CanMove(board, new PiecePosition(2, 3)));
    }
    [Fact]
    public void GameSnapshot_CreatesIndependentCopyOfGameState()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        Rook whiteRook = new Rook(PieceColor.White)
        {
            Position = new PiecePosition(7, 0)
        };

        pieces[7, 0] = whiteRook;

        ChessBoard board = new ChessBoard(pieces);

        MoveInfo previousMove = new MoveInfo(
            new PiecePosition(6, 4),
            new PiecePosition(4, 4),
            PieceColor.White);

        Pawn capturedBlackPawn = new Pawn(PieceColor.Black)
        {
            Position = new PiecePosition(4, 4)
        };

        List<Piece> capturedPieces = new()
    {
        capturedBlackPawn
    };

        GameSnapshot snapshot = new GameSnapshot(
            board,
            PieceColor.Black,
            previousMove,
            capturedPieces);

        board[7, 0] = null;
        previousMove.Start!.Row = 5;
        capturedBlackPawn.HasMoved = true;

        Assert.IsType<Rook>(snapshot.Board[7, 0]);
        Assert.Equal(PieceColor.Black, snapshot.ActiveTurn);

        Assert.NotNull(snapshot.PreviousMove);
        Assert.Equal(new PiecePosition(6, 4), snapshot.PreviousMove.Start);

        Piece snapshotCapturedPiece = Assert.Single(snapshot.CapturedPieces);
        Assert.IsType<Pawn>(snapshotCapturedPiece);
        Assert.Equal(PieceColor.Black, snapshotCapturedPiece.Color);
        Assert.False(snapshotCapturedPiece.HasMoved);
    }
    [Fact]
    public void MoveValidation_ReturnsFalse_WhenMoveWouldCaptureKing()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        Rook whiteRook = new Rook(PieceColor.White)
        {
            Position = new PiecePosition(7, 4) // e1
        };

        King blackKing = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4) // e8
        };

        pieces[7, 4] = whiteRook;
        pieces[0, 4] = blackKing;

        ChessBoard board = new ChessBoard(pieces);

        bool isValid = ChessRules.MoveValidation(
            board,
            new PiecePosition(7, 4),
            new PiecePosition(0, 4),
            PieceColor.White);

        Assert.False(isValid);
    }
    [Fact]
    public void PromotePawn_AllowsPromotedQueenToGiveCheck()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        pieces[0, 1] = new Pawn(PieceColor.White)
        {
            Position = new PiecePosition(0, 1) // b8
        };

        pieces[7, 4] = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4) // e1
        };

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4) // e8
        };

        ChessBoard board = new ChessBoard(pieces);

        bool promoted = Game.PromotePawn(
            board,
            new PiecePosition(0, 1),
            PieceType.Queen);

        PiecePosition blackKingPosition = new PiecePosition(0, 4);

        Assert.True(promoted);
        Assert.IsType<Queen>(board[0, 1]);

        Assert.True(ChessRules.IsChecked(
            board,
            blackKingPosition,
            PieceColor.Black));
    }
    [Fact]
    public void TryMove_MovesWhitePawn_AndChangesTurn()
    {
        var game = new ChessGame();

        MoveResult result = game.TryMove(
            new MoveInfo(
                new PiecePosition(6, 4),
                new PiecePosition(4, 4)));

        Assert.Equal(MoveType.RegularMove, result.MoveType);
        Assert.Equal(BoardState.Ongoing, result.BoardState);

        Assert.Null(game.Board[6, 4]);
        Assert.IsType<Pawn>(game.Board[4, 4]);

        Assert.Equal(PieceColor.Black, game.ActiveTurn);

        Assert.NotNull(game.PreviousMove);
        Assert.Equal(new PiecePosition(6, 4), game.PreviousMove.Start);
        Assert.Equal(new PiecePosition(4, 4), game.PreviousMove.Target);
    }
    [Fact]
    public void TryMove_ReturnsInvalidMove_WhenWrongColorMoves()
    {
        var game = new ChessGame();

        MoveResult result = game.TryMove(
            new MoveInfo(
                new PiecePosition(1, 4),
                new PiecePosition(3, 4)));

        Assert.Equal(MoveType.InvalidMove, result.MoveType);
        Assert.Equal(PieceColor.White, game.ActiveTurn);

        Assert.IsType<Pawn>(game.Board[1, 4]);
        Assert.Null(game.Board[3, 4]);
    }
    [Fact]
    public void TryMove_RequiresPromotion_WhenPawnReachesLastRank()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        pieces[1, 0] = new Pawn(PieceColor.White)
        {
            Position = new PiecePosition(1, 0)
        };

        pieces[7, 4] = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4)
        };

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4)
        };

        var game = new ChessGame();

        game.RestoreSnapshot(new GameSnapshot(
            new ChessBoard(pieces),
            PieceColor.White,
            null,
            []));

        MoveResult result = game.TryMove(
            new MoveInfo(
                new PiecePosition(1, 0),
                new PiecePosition(0, 0)));

        Assert.Equal(MoveType.PawnPromotion, result.MoveType);
        Assert.True(game.IsPromotionPending);
        Assert.IsType<Pawn>(game.Board[0, 0]);
        Assert.Equal(PieceColor.White, game.ActiveTurn);
    }
    [Fact]
    public void PromotePawn_ReplacesPawnAndChangesTurn()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        pieces[1, 0] = new Pawn(PieceColor.White)
        {
            Position = new PiecePosition(1, 0)
        };

        pieces[7, 4] = new King(PieceColor.White)
        {
            Position = new PiecePosition(7, 4)
        };

        pieces[0, 4] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 4)
        };

        var game = new ChessGame();

        game.RestoreSnapshot(new GameSnapshot(
            new ChessBoard(pieces),
            PieceColor.White,
            null,
            []));

        game.TryMove(new MoveInfo(
            new PiecePosition(1, 0),
            new PiecePosition(0, 0)));

        MoveResult result = game.PromotePawn(PieceType.Queen);

        Assert.Equal(MoveType.PawnPromotion, result.MoveType);
        Assert.False(game.IsPromotionPending);
        Assert.IsType<Queen>(game.Board[0, 0]);
        Assert.Equal(PieceColor.Black, game.ActiveTurn);
    }
}
