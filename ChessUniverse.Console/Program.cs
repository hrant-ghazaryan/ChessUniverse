using ChessUniverse.Library;
using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;

//int q = 0;
//bool isCheckMate = false;
//PieceColor T = PieceColor.White;
//ChessBoard chessBoard = new ChessBoard();
//chessBoard.SetStartPosition();
//PrintBoard(chessBoard);

//do
//{
//    if (ChessRules.IsStaleMate(chessBoard, T))
//    {
//        Console.WriteLine(" StaleMate! ");
//        break;
//    }

//    if (T == PieceColor.White)
//        Console.WriteLine("White Please: ");
//    if (T == PieceColor.Black)
//        Console.WriteLine("Black Please: ");
//    PiecePosition start = new PiecePosition();
//    Console.Write("Enter start position: ");
//    EnterNumber(start);
//    PiecePosition target = new PiecePosition();
//    Console.Write("Enter target position: ");
//    EnterNumber(target);
//    MoveAffirmation ma = new MoveAffirmation(start, target);
//    isCheckMate = ma.Move(ref chessBoard, target, ref T);
//    PrintBoard(chessBoard);
//    q++;
//} while (q != 120);

Piece[,] stalemateBoard = new Piece[8, 8];
stalemateBoard[0, 7] = new King(PieceColor.Black) { Position = new PiecePosition { Row = 0, Col = 7 } };
stalemateBoard[1, 5] = new Queen(PieceColor.White) { Position = new PiecePosition { Row = 1, Col = 5 } };
stalemateBoard[2, 6] = new King(PieceColor.White) { Position = new PiecePosition { Row = 2, Col = 6 } };
ChessBoard board = new ChessBoard(stalemateBoard);

var isStalemateChecker = ChessRules.IsStaleMate(board, PieceColor.Black);
Console.WriteLine(isStalemateChecker); 

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

