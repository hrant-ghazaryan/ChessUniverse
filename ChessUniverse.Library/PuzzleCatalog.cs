using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;

namespace ChessUniverse.Library;

public static class PuzzleCatalog
{
    public static IReadOnlyList<ChessWorld> Worlds { get; } =
        CreateWorlds();

    public static ChessWorld? GetWorld(int worldId)
    {
        return Worlds.FirstOrDefault(world => world.Id == worldId);
    }

    public static ChessPuzzle? GetPuzzle(int puzzleId)
    {
        return Worlds
            .SelectMany(world => world.Puzzles)
            .FirstOrDefault(puzzle => puzzle.Id == puzzleId);
    }

    private static IReadOnlyList<ChessWorld> CreateWorlds()
    {
        ChessPuzzle mateInOne = CreateMateInOnePuzzle();

        var firstWorld = new ChessWorld(
            id: 1,
            title: "Սկսնակ աշխարհ",
            puzzles: [mateInOne]);

        return [firstWorld];
    }

    private static ChessPuzzle CreateMateInOnePuzzle()
    {
        Piece?[,] pieces = new Piece?[8, 8];

        pieces[0, 7] = new King(PieceColor.Black)
        {
            Position = new PiecePosition(0, 7)
        };

        pieces[2, 5] = new King(PieceColor.White)
        {
            Position = new PiecePosition(2, 5)
        };

        pieces[2, 6] = new Queen(PieceColor.White)
        {
            Position = new PiecePosition(2, 6)
        };

        ChessBoard board = new ChessBoard(pieces);

        return new ChessPuzzle(
            id: 1,
            worldId: 1,
            levelNumber: 1,
            title: "Մատ մեկ քայլում",
            initialBoard: board,
            activeTurn: PieceColor.White,
            solutionMoves:
            [
                new MoveInfo(
                    new PiecePosition(2, 6),
                    new PiecePosition(1, 6))
            ]);
    }
}
