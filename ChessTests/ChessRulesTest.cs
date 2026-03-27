using ChessUniverse.Library;
using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;

namespace ChessTests;

public class ChessRulesTest
{
    [Theory]
    [MemberData(nameof(MoveTestCases))]
    public void MoveValidationTest(PiecePosition? start, PiecePosition? end,
         PieceColor? T)
    {
        ChessBoard board = new ChessBoard();
        board.SetStartPosition();

        var isValid = ChessRules.MoveValidation(board, start, end, T);

        Assert.True(isValid);
    }

    [Fact]
    public void IsCheckedTest()
    {
        Piece[,] checkBoard = new Piece[8, 8];
        checkBoard[0, 4] = new King(PieceColor.Black) { Position = new PiecePosition { Row = 0, Col = 4 } };
        checkBoard[5, 4] = new Rook(PieceColor.White) { Position = new PiecePosition { Row = 5, Col = 4 } };
        checkBoard[7, 4] = new King(PieceColor.White) { Position = new PiecePosition { Row = 7, Col = 4 } };
        ChessBoard board = new ChessBoard(checkBoard);


        var checkChecker = ChessRules.IsChecked(board, new PiecePosition(0,4));
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
}
