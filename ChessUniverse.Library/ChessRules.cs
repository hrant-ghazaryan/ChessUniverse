using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library
{
    public static class ChessRules
    {
        public static (bool, int) IsChecked(ChessBoard chessBoard)
        {
            int k = 0;
            bool C = false;
            PiecePosition? BKP = ChessBoard.GetKingPosition(chessBoard, PieceColor.Black);
            PiecePosition? WKP = ChessBoard.GetKingPosition(chessBoard, PieceColor.White);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = chessBoard[i, j];
                    if (piece?.Color == PieceColor.White
                        && piece.IsMovePossible(chessBoard, BKP))
                    {
                        C = true;
                        k++;
                    }
                    if (piece?.Color == PieceColor.Black
                        && piece.IsMovePossible(chessBoard, WKP))
                    {
                        C = true;
                        k++;
                    }
                }
            }
            return (C, k);
        }
        public static bool IsChecked(ChessBoard chessBoard, PiecePosition position)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = chessBoard[i, j];
                    var pieceparam = chessBoard[position];
                    if (piece?.Color == PieceColor.White && pieceparam?.Color == PieceColor.Black
                        && piece.IsMovePossible(chessBoard, position))
                        return true;
                    if (piece?.Color == PieceColor.Black && pieceparam?.Color == PieceColor.White
                        && piece.IsMovePossible(chessBoard, position))
                        return true;
                }
            }
            return false;
        }
        //public static ChessBoard MoveValidation(ChessBoard board, PiecePosition start, PiecePosition end, ref bool T, ref bool C)
        //{
        //    if (C == false)
        //    {
        //        if (T == true && board[start]?.Color == PieceColor.White)
        //        {
        //            if (board[start]!.IsMovePossible(board, end))
        //            {
        //                board[start]?.Move(board, end);
        //                T = false;
        //                C = ChessBoard.IsChecked(board).Item1;
        //                return board;
        //            }
        //        }
        //        if (T == false && board[start]?.Color == PieceColor.Black)
        //        {
        //            if (board[start]!.IsMovePossible(board, end))
        //            {
        //                board[start]?.Move(board, end);
        //                T = true;
        //                C = ChessBoard.IsChecked(board).Item1;
        //                return board;
        //            }
        //        }
        //        return board;
        //    }
        //    if (C == true && ChessBoard.IsChecked(board).Item2 > 1)
        //    {
        //        if (T == false && board[start]?.Color == PieceColor.Black
        //        && board[start]?.Type == PieceType.King)
        //        {
        //            if (board[start]!.IsMovePossible(board, end))
        //            {
        //                board[start]?.Move(board, end);
        //                T = true;
        //                C = ChessBoard.IsChecked(board).Item1;
        //                if (C)
        //                {
        //                    board[start]?.Move(board, end);
        //                    C = false;
        //                    Console.WriteLine("You are in CHECK , please protect your KING");
        //                }
        //                return board;
        //            }
        //        }
        //        else if (T == true && board[start]?.Color == PieceColor.White
        //        && board[start]?.Type == PieceType.King)
        //        {
        //            if (board[start]!.IsMovePossible(board, end))
        //            {
        //                board[start]?.Move(board, end);
        //                T = false;
        //                C = ChessBoard.IsChecked(board).Item1;
        //                if (C)
        //                {
        //                    board[start]?.Move(board, end);
        //                    C = false;
        //                    Console.WriteLine("You are in CHECK , please protect your KING");
        //                }
        //                return board;
        //            }
        //        }
        //    }
        //    else if (C == true)
        //    {
        //        if (T == false && board[start]?.Color == PieceColor.Black)
        //        {
        //            if (board[start]!.IsMovePossible(board, end))
        //            {
        //                board[start]?.Move(board, end);
        //                T = true;
        //                C = ChessBoard.IsChecked(board).Item1;
        //                if (C)
        //                {
        //                    board[end]?.MoveBack(board, start);
        //                    C = false;
        //                    Console.WriteLine("You are in CHECK , please protect your KING");
        //                }
        //                return board;
        //            }
        //        }
        //        else if (T == true && board[start]?.Color == PieceColor.White)
        //        {
        //            if (board[start]!.IsMovePossible(board, end))
        //            {
        //                board[start]?.Move(board, end);
        //                T = false;
        //                C = ChessBoard.IsChecked(board).Item1;
        //                if (C)
        //                {
        //                    board[start]?.MoveBack(board, end);
        //                    C = false;
        //                    Console.WriteLine("You are in CHECK , please protect your KING");
        //                }
        //                return board;
        //            }
        //        }
        //    }
        //    return board;
        //}
        public static bool MoveValidation(ChessBoard board, PiecePosition start, PiecePosition end,
             PieceColor T)
        {
            if (T == board[start]?.Color)
            {
                if (board[start]!.IsMovePossible(board, end) && board[start]?.Color != board[end]?.Color )
                    return true;
            }
            return false;
        }
    }
}
