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
    Move(chessBoard, start, target, ref T, ref C);
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
(bool,int) IsChecked(ChessBoard chessBoard)
{
    int k = 0;
    bool C = false;
    PiecePosition? BKP = null;
    PiecePosition? WKP = null;
    BKP = GetBlackKingPosition(chessBoard, ref BKP);
    WKP = GetWhiteKingPosition(chessBoard, ref WKP);
    for (int i = 0; i < 8; i++)
    {
        for (int j = 0; j < 8; j++)
        {
            if (chessBoard[i, j]?.Color == PieceColor.White
                && chessBoard[i, j].IsMovePossible(chessBoard, chessBoard[i, j]?.Position, BKP))
            {
                Console.WriteLine();
                Console.WriteLine("        CHECK!!!      ");
                Console.WriteLine();
                C = true;
                k++; 
            }
            if (chessBoard[i, j]?.Color == PieceColor.Black
                && chessBoard[i, j].IsMovePossible(chessBoard, chessBoard[i, j]?.Position, WKP))
            {
                Console.WriteLine();
                Console.WriteLine("        CHECK!!!      ");
                Console.WriteLine();
                C = true;
                k++;
            }
        }
    }
    return (C, k);
}
ChessBoard MoveBack(ChessBoard board, PiecePosition start, PiecePosition end)
{
    PiecePosition temp = end;
    if (IsChecked(board).Item1 && IsChecked(board).Item2 != 0)
    {
        board[start.Row, start.Col] = board[end.Row, end.Col];
        board[end.Row, end.Col] = null;
        return board;
    }
    return board;
}
ChessBoard Move(ChessBoard board, PiecePosition start, PiecePosition end , ref bool T, ref bool C)
{
    PiecePosition temp;
    if (C == false)
    {
        if (T == true && board[start.Row, start.Col]?.Color == PieceColor.White)
        {
            if (board[start.Row, start.Col]!.IsMovePossible(board, start, end))
            {
                temp = end;
                board[end.Row, end.Col] = board[start.Row, start.Col];
                board[end.Row, end.Col].Position = end;
                board[start.Row, start.Col] = null;
                T = false;
                C = IsChecked(chessBoard).Item1;
                return board;
            }
        }
        if (T == false && board[start.Row, start.Col]?.Color == PieceColor.Black)
        {
            if (board[start.Row, start.Col]!.IsMovePossible(board, start, end))
            {
                temp = end;
                board[end.Row, end.Col] = board[start.Row, start.Col];
                board[end.Row, end.Col].Position = end;
                board[start.Row, start.Col] = null;
                T = true;
                C = IsChecked(board).Item1;
                return board;
            }
        }
        return board;
    }
    if (C == true && IsChecked(board).Item2 > 1)
    {
        if (T == false && board[start.Row, start.Col]?.Color == PieceColor.Black
        && board[start.Row, start.Col]?.Type == PieceType.King)
        {
            if (board[start.Row, start.Col]!.IsMovePossible(board, start, end))
            {
                board[end.Row, end.Col] = board[start.Row, start.Col];
                board[end.Row, end.Col].Position = end;
                board[start.Row, start.Col] = null;
                T = true;
                C = IsChecked(board).Item1;
                if (C)
                {
                    MoveBack(board, start, end);
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
                board[end.Row, end.Col] = board[start.Row, start.Col];
                board[end.Row, end.Col].Position = end;
                board[start.Row, start.Col] = null;
                T = false;
                C = IsChecked(chessBoard).Item1;
                if (C)
                {
                    MoveBack(board, start, end);
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
                board[end.Row, end.Col] = board[start.Row, start.Col];
                board[end.Row, end.Col].Position = end;
                board[start.Row, start.Col] = null;
                T = true;
                C = IsChecked(board).Item1;
                if (C)
                {
                    MoveBack(board, start, end);
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
                board[end.Row, end.Col] = board[start.Row, start.Col];
                board[end.Row, end.Col].Position = end;
                board[start.Row, start.Col] = null;
                T = false;
                C = IsChecked(chessBoard).Item1;
                if (C)
                {
                    MoveBack(board, start, end);
                    C = false;
                    Console.WriteLine("You are in CHECK , please protect your KING");
                }
                return board;
            }
        }
    }
    return board;
}
PiecePosition GetWhiteKingPosition(ChessBoard chessBoard , ref PiecePosition WK)
{
    for (int i = 0; i < 8; i++)
    {
        for (int j = 0; j < 8; j++)
        {
            if (chessBoard[i, j]?.Type == PieceType.King 
                && chessBoard[i, j]?.Color == PieceColor.White)
                    WK = chessBoard[i, j].Position;
        }
    }
    return WK;
}
PiecePosition GetBlackKingPosition(ChessBoard chessBoard , ref PiecePosition BP)
{
    for (int i = 0; i < 8; i++)
    {
        for (int j = 0; j < 8; j++)
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

