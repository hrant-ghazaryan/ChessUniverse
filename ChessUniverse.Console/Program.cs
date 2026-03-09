using ChessUniverse.Library;
using ChessUniverse.Library.Pieces;
using ChessUniverse.Library.Enums;

int q = 0;
ChessBoard chessBoard = new ChessBoard();
chessBoard.SetStartPosition();
PrintBoard(chessBoard);

do
{
    PiecePosition start = new PiecePosition();
    Console.Write("Enter start position: ");
    EnterNumber(ref start);
    PiecePosition target = new PiecePosition();
    Console.Write("Enter target position: ");
    EnterNumber(ref target);
    Move(ref chessBoard, ref start, ref target);
    PrintBoard(chessBoard);
    q++;
} while (q != 10);

//ChessBoard chessBoard = new ChessBoard();
//chessBoard.SetStartPosition();
//PiecePosition start = new PiecePosition();
//Console.Write("Enter start position: ");
//EnterNumber(ref start);
//PiecePosition target = new PiecePosition();
//Console.Write("Enter target position: ");
//EnterNumber(ref target);
//Bishop bishop = new Bishop(PieceColor.White);
//Console.WriteLine(bishop.IsMovePossible(chessBoard, start, target));


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
ChessBoard Move(ref ChessBoard board, ref PiecePosition start, ref PiecePosition end)
{
    if (board[start.Row, start.Col].Type == PieceType.Queen || board[start.Row, start.Col].Type == PieceType.Bishop
        || board[start.Row, start.Col].Type == PieceType.Rook)
    {
        if (board[start.Row, start.Col].IsMovePossible(ref board, ref start, ref end) == false)
        {
            board[end.Row, end.Col] = board[start.Row, start.Col];
            board[start.Row, start.Col] = null;
            return board;
        }
    }
    else if(board[start.Row,start.Col].IsMovePossible(ref board,ref start,ref end))
    {
        board[end.Row,end.Col] = board[start.Row,start.Col];
        board[start.Row,start.Col] = null;
        return board;
    }
    return board;
}
PiecePosition EnterNumber(ref PiecePosition position)
{
    string? coordinate;
    do
    {
        coordinate = Console.ReadLine();
    } while (coordinate?.Length != 2 || coordinate[0] - 'A' < 0 || coordinate[0] - 'A' > 7 
                         || 7 - (coordinate[1] - '1') < 0 || 7 - (coordinate[1] - '1') > 7 );

    char file = coordinate[0]; // A - H
    char rank = coordinate[1]; // 1 - 8
    position.Row = 7 - (rank - '1');
    position.Col = file - 'A';

    return position;
}