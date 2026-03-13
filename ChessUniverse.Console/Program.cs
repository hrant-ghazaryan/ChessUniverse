using ChessUniverse.Library;
using ChessUniverse.Library.Enums;

int q = 0;
bool T = true;
bool C = false;
ChessBoard chessBoard = new ChessBoard();
chessBoard.SetStartPosition();
PrintBoard(chessBoard);

do
{
    if (T == true)
        Console.WriteLine("White Please: ");
    if (T == false)
        Console.WriteLine("Black Please: ");
    PiecePosition start = new PiecePosition();
    Console.Write("Enter start position: ");
    EnterNumber(start);
    PiecePosition target = new PiecePosition();
    Console.Write("Enter target position: ");
    EnterNumber(target);
    MoveValidation(chessBoard, start, target, ref T, ref C);
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

ChessBoard MoveValidation(ChessBoard board, PiecePosition start, PiecePosition end , ref bool T, ref bool C)
{
    if (C == false)
    {
        if (T == true && board[start.Row, start.Col]?.Color == PieceColor.White)
        {
            if (board[start.Row, start.Col]!.IsMovePossible(board, start, end))
            {
                board[start.Row, start.Col]?.Move(board, end);
                T = false;
                C = ChessBoard.IsChecked(chessBoard).Item1;
                return board;
            }
        }
        if (T == false && board[start.Row, start.Col]?.Color == PieceColor.Black)
        {
            if (board[start.Row, start.Col]!.IsMovePossible(board, start, end))
            {
                board[start.Row, start.Col]?.Move(board, end);
                T = true;
                C = ChessBoard.IsChecked(board).Item1;
                return board;
            }
        }
        return board;
    }
    if (C == true && ChessBoard.IsChecked(board).Item2 > 1)
    {
        if (T == false && board[start.Row, start.Col]?.Color == PieceColor.Black
        && board[start.Row, start.Col]?.Type == PieceType.King)
        {
            if (board[start.Row, start.Col]!.IsMovePossible(board, start, end))
            {
                board[start.Row, start.Col]?.Move(board, end);
                T = true;
                C = ChessBoard.IsChecked(board).Item1;
                if (C)
                {
                    board[start.Row, start.Col]?.Move(board, end);
                    C = false;
                    Console.WriteLine("You are in CHECK , please protect your KING");
                }
                return board;
            }
        }
        else if (T == true && board[start.Row, start.Col]?.Color == PieceColor.White
        && board[start.Row, start.Col]?.Type == PieceType.King)
        {
            if (board[start.Row, start.Col]!.IsMovePossible(board, start, end))
            {
                board[start.Row, start.Col]?.Move(board, end);
                T = false;
                C = ChessBoard.IsChecked(chessBoard).Item1;
                if (C)
                {
                    board[start.Row, start.Col]?.Move(board, end);
                    C = false;
                    Console.WriteLine("You are in CHECK , please protect your KING");
                }
                return board;
            }
        }
    }
    else if (C == true)
    {
        if (T == false && board[start.Row, start.Col]?.Color == PieceColor.Black)
        {
            if (board[start.Row, start.Col]!.IsMovePossible(board, start, end))
            {
                board[start.Row, start.Col]?.Move(board, end);
                T = true;
                C = ChessBoard.IsChecked(board).Item1;
                if (C)
                {
                    board[end.Row, end.Col]?.MoveBack(board, start);
                    C = false;
                    Console.WriteLine("You are in CHECK , please protect your KING");
                }
                return board;
            }
        }
        else if (T == true && board[start.Row, start.Col]?.Color == PieceColor.White)
        {
            if (board[start.Row, start.Col]!.IsMovePossible(board, start, end))
            {
                board[start.Row, start.Col]?.Move(board, end);
                T = false;
                C = ChessBoard.IsChecked(chessBoard).Item1;
                if (C)
                {
                    board[start.Row, start.Col]?.MoveBack(board, end);
                    C = false;
                    Console.WriteLine("You are in CHECK , please protect your KING");
                }
                return board;
            }
        }
    }
    return board;
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

