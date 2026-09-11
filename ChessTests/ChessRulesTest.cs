using ChessUniverse.Library;
using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;

namespace ChessTests;

public class ChessRulesTest
{
    /*[Theory]
    [MemberData(nameof(MoveTestCases))]
    public void MoveValidationTest(PiecePosition? start, PiecePosition? end,
         PieceColor? T)
    {
        ChessBoard board = new ChessBoard();
        board.SetStartPosition();

        var isValid = ChessRules.MoveValidation(board, start, end, T);

        Assert.True(isValid);
    }*/

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
}
