using ChessUniverse.Library;
using ChessUniverse.Library.Enums;

int q = 0;
PieceColor Turn = PieceColor.White;
ChessBoard chessBoard = new ChessBoard();
chessBoard.SetStartPosition();
PrintBoard(chessBoard);
Game game = new Game();
do
{
    Console.WriteLine(Turn == PieceColor.White ? "White Please: " : "Black Please: ");

    PiecePosition start = new PiecePosition();
    Console.Write("Enter start position: ");
    EnterNumber(start);

    PiecePosition target = new PiecePosition();
    Console.Write("Enter target position: ");
    EnterNumber(target);

    MoveInfo moveInfo = new MoveInfo(start, target, Turn);
    game.Move(ref chessBoard, ref moveInfo);
    PrintBoard(chessBoard);

    q++;
    Turn = moveInfo.T;
} while (q != 120);


void PrintBoard(ChessBoard chessBoard)
{
    for (int row = 0; row < 8; row++)
    {
        Console.Write($"{8 - row}  ");
        for (int col = 0; col < 8; col++)
        {
            bool isLightSquare = (row + col) % 2 == 0;
            Console.BackgroundColor = isLightSquare ? ConsoleColor.Gray : ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Black;
            var piece = chessBoard[row, col];
            char symbol = piece?.GetSymbol(piece.Color) ?? '.';
            Console.Write($" {symbol} ");
        }
        Console.ResetColor();
        Console.WriteLine();
    }
    Console.WriteLine("    A  B  C  D  E  F  G  H");
}
PiecePosition EnterNumber(PiecePosition position)
{
    string? coordinate;
    do
    {
        coordinate = Console.ReadLine();
    } while (coordinate?.Length != 2 || coordinate[0] - 'A' < 0 || coordinate[0] - 'A' > 7
                         || 7 - (coordinate[1] - '1') < 0 || 7 - (coordinate[1] - '1') > 7);

    char file = coordinate[0]; // A - H
    char rank = coordinate[1]; // 1 - 8
    position.Row = 7 - (rank - '1');
    position.Col = file - 'A';

    return position;
}


