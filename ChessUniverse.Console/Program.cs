using ChessUniverse.Library;
using ChessUniverse.Library.Enums;

int q = 0;
bool h = true;
ChessBoard chessBoard = new ChessBoard();
chessBoard.SetStartPosition();
PrintBoard(chessBoard);

do
{
    if (h == true)
        Console.WriteLine("White Please: ");
    if (h == false)
        Console.WriteLine("Black Please: ");
    PiecePosition start = new PiecePosition();
    Console.Write("Enter start position: ");
    EnterNumber(start);
    PiecePosition target = new PiecePosition();
    Console.Write("Enter target position: ");
    EnterNumber(target);
    Move(chessBoard, start, target, ref h);
    PrintBoard(chessBoard);
    q++;
} while (q != 10);

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
bool IsChecked(ChessBoard chessBoard)
{
    PiecePosition BKP = null;
    PiecePosition WKP;
    BKP = GetBlackKingPosition(chessBoard, ref BKP);
    WKP = GetWhiteKingPosition(chessBoard);
    for (int i = 0; i < 7; i++)
    {
        for (int j = 0; j < 7; j++)
        {
            if (chessBoard[i, j]?.Color == PieceColor.White
                && chessBoard[i, j].IsMovePossible(chessBoard, chessBoard[i, j]?.Position, BKP))
            {
                Console.WriteLine();
                Console.WriteLine("        CHECK!!!      ");
                Console.WriteLine();
                return true;
            }
            if (chessBoard[i, j]?.Color == PieceColor.Black
                && chessBoard[i, j].IsMovePossible(chessBoard, chessBoard[i, j]?.Position, WKP))
            {
                Console.WriteLine();
                Console.WriteLine("        CHECK!!!      ");
                Console.WriteLine();
                return true;
            }
        }
    }
    return false;
}
ChessBoard Move(ChessBoard board, PiecePosition start, PiecePosition end , ref bool h)
{
    if (h == true && board[start.Row,start.Col]?.Color == PieceColor.White)
    {
        if (board[start.Row, start.Col]!.IsMovePossible(board, start, end))
        {
            board[end.Row, end.Col] = board[start.Row, start.Col];
            board[start.Row, start.Col] = null;
            h = false;
            IsChecked(chessBoard);
            return board;
        }
    }
    if (h == false && board[start.Row, start.Col]?.Color == PieceColor.Black)
    {
        if (board[start.Row, start.Col]!.IsMovePossible(board, start, end))
        {
            board[end.Row, end.Col] = board[start.Row, start.Col];
            board[start.Row, start.Col] = null;
            h = true;
            //IsChecked(chessBoard);
            return board;
        }
    }
    return board;
}
PiecePosition? GetWhiteKingPosition(ChessBoard chessBoard)
{
    for (int i = 0; i < 7; i++)
    {
        for (int j = 0; j < 7; j++)
        {
            if (chessBoard[i, j]?.Type == PieceType.King 
                && chessBoard[i, j]?.Color == PieceColor.White)
                    return chessBoard[i, j].Position;
        }
    }
    return null;
}
PiecePosition GetBlackKingPosition(ChessBoard chessBoard , ref PiecePosition BP)
{
    for (int i = 0; i < 7; i++)
    {
        for (int j = 0; j < 7; j++)
        {
            if (chessBoard[i, j]?.Type == PieceType.King
                && chessBoard[i, j]?.Color == PieceColor.Black)
                    BP = chessBoard[i,j].Position;
        }
    }
    return BP;
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
