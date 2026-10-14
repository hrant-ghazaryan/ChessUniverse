using ChessUniverse.Library;
using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;

namespace ChessTests;

public sealed class PuzzleCatalogTests
{
    [Fact]
    public void MateInTwoPuzzle_CompletesAfterItsSolution()
    {
        ChessPuzzle puzzle = PuzzleCatalog
            .GetPuzzles(PuzzleCategory.MateInTwo)
            .Single();

        ChessPuzzleSession session = new(puzzle);

        PuzzleMoveResult firstMove = session.TryMove(new MoveInfo(
            new PiecePosition(3, 5),
            new PiecePosition(3, 6)));

        Assert.Equal(PuzzleMoveStatus.CorrectMove, firstMove.Status);
        Assert.IsType<King>(session.Board[1, 7]);

        PuzzleMoveResult finalMove = session.TryMove(new MoveInfo(
            new PiecePosition(3, 6),
            new PiecePosition(1, 6)));

        Assert.Equal(PuzzleMoveStatus.Completed, finalMove.Status);
        Assert.IsType<Queen>(session.Board[1, 6]);
    }
}
