using ChessUniverse.Library;
using ChessUniverse.Library.Enums;

// Պարզ console տարբերակ debug-ի համար. քայլերը մուտքագրվում են "e2 e4" ձևով։
ChessGame game = new();

while (true)
{
    PrintBoard(game.Board);
    Console.WriteLine($"{game.ActiveTurn} to move (e.g. e2 e4, 'q' to quit): ");

    string? input = Console.ReadLine();
    if (input is null || input.Trim() == "q")
        break;

    string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length != 2 ||
        !PiecePosition.TryParse(parts[0], out PiecePosition? start) ||
        !PiecePosition.TryParse(parts[1], out PiecePosition? target))
    {
        Console.WriteLine("Invalid input.");
        continue;
    }

    MoveResult result = game.TryMove(new MoveInfo(start!, target!));

    if (result.MoveType == MoveType.PawnPromotion && game.IsPromotionPending)
        result = game.PromotePawn(PieceType.Queen);

    if (result.MoveType == MoveType.InvalidMove)
    {
        Console.WriteLine("Illegal move.");
        continue;
    }

    if (result.BoardState is BoardState.CheckMate or BoardState.StaleMate)
    {
        PrintBoard(game.Board);
        Console.WriteLine(result.BoardState);
        break;
    }

    if (result.BoardState == BoardState.Check)
        Console.WriteLine("Check!");
}

static void PrintBoard(ChessBoard board)
{
    for (int row = 0; row < 8; row++)
    {
        Console.Write($"{8 - row}  ");
        for (int col = 0; col < 8; col++)
        {
            bool isLightSquare = (row + col) % 2 == 0;
            Console.BackgroundColor = isLightSquare ? ConsoleColor.Gray : ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Black;
            Piece? piece = board[row, col];
            char symbol = piece?.GetSymbol(piece.Color) ?? '.';
            Console.Write($" {symbol} ");
        }
        Console.ResetColor();
        Console.WriteLine();
    }
    Console.WriteLine("    a  b  c  d  e  f  g  h");
}
