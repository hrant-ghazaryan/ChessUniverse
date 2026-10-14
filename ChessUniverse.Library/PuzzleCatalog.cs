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
        ChessPuzzle mateInTwo = CreateMateInTwoPuzzle();

        var firstWorld = new ChessWorld(
            id: 1,
            title: "Սկսնակ աշխարհ",
            puzzles: [mateInOne, mateInTwo]);

        return [firstWorld];
    }
    public static IReadOnlyList<PuzzleCategory> Categories { get; } =
    [
        PuzzleCategory.MateInOne,
        PuzzleCategory.MateInTwo
    ];

    public static IReadOnlyList<PuzzleDifficulty> Difficulties { get; } =
    [
        PuzzleDifficulty.Easy,
        PuzzleDifficulty.Medium,
        PuzzleDifficulty.Hard
    ];

    public static IReadOnlyList<ChessPuzzle> GetPuzzles(
    PuzzleCategory category)
          => Worlds
            .SelectMany(world => world.Puzzles)
            .Where(puzzle => puzzle.Category == category)
            .OrderBy(puzzle => puzzle.LevelNumber)
            .ToList()
            .AsReadOnly();

    public static IReadOnlyList<ChessPuzzle> GetPuzzles(
        PuzzleCategory category,
        PuzzleDifficulty difficulty)
        => Worlds
            .SelectMany(world => world.Puzzles)
            .Where(puzzle =>
                puzzle.Category == category &&
                puzzle.Difficulty == difficulty)
            .OrderBy(puzzle => puzzle.LevelNumber)
            .ToList()
            .AsReadOnly();

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
            category: PuzzleCategory.MateInOne,
            difficulty: PuzzleDifficulty.Easy,
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
    private static ChessPuzzle CreateMateInTwoPuzzle()
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

        pieces[3, 5] = new Queen(PieceColor.White)
        {
            Position = new PiecePosition(3, 5)
        };

        ChessBoard board = new ChessBoard(pieces);

        return new ChessPuzzle(
            id: 2,
            worldId: 1,
            levelNumber: 1,
            category: PuzzleCategory.MateInTwo,
            difficulty: PuzzleDifficulty.Easy,
            title: "Մատ երկու քայլում",
            initialBoard: board,
            activeTurn: PieceColor.White,
            solutionMoves:
            [
                // 1. Qg5 — սևի միակ պատասխանը Kh7 է։
                new MoveInfo(
                    new PiecePosition(3, 5),
                    new PiecePosition(3, 6)),

            // ... Kh7
            new MoveInfo(
                new PiecePosition(0, 7),
                new PiecePosition(1, 7)),

            // 2. Qg7#
            new MoveInfo(
                new PiecePosition(3, 6),
                new PiecePosition(1, 6))
            ]);
    }
}
