using ChessUniverse.Library;

PiecePosition start = new PiecePosition(0, 1);
PiecePosition target = new PiecePosition(2, 2);
ChessBoard chessBoard = new ChessBoard();
chessBoard.SetStartPosition();
PrintBoard(chessBoard);
Move(chessBoard, start, target);
PrintBoard(chessBoard);


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

ChessBoard Move(ChessBoard board, PiecePosition start, PiecePosition end)
{
    if (board[start].IsMovePossible(start,end))
    {
        board[end.Row,end.Col] = board[start];
        board[start.Row,start.Col] = null;
        return board;
    }
    return board;
}