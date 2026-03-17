using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public static class ChessRules
{

    public static char GetSymbol(Piece piece, PieceColor color)
    {
        if (color == PieceColor.White)
        {
            string s = piece.Symbol.ToString().ToUpper();
            bool b = char.TryParse(s, out char c);
            return c;
        }
        else
            return piece.Symbol;
    }
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
                if (BKP != null && WKP != null)
                {
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
    public static (bool,Piece?) IsChecked(ChessBoard board, Piece? piece)
    {
        PiecePosition? kingposition = ChessBoard.GetKingPosition(board, PieceColor.White);

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (MoveValidation(board, piece.Position, kingposition, piece.Color))
                {
                    return (true,piece);
                }
                else
                    return (false,null);
            }
        }
        return (false, null);
    }
    public static bool MoveValidation(ChessBoard board, PiecePosition start, PiecePosition end,
         PieceColor T)
    {
        if (T == board[start]?.Color)
        {
            if (board[start]!.IsMovePossible(board, end) && board[start]?.Color != board[end]?.Color)
                return true;
        }
        return false;
    }
    public static bool PawnPromotion(ChessBoard board, PiecePosition start)
    {
        Piece? piece = board[start];

        if (piece?.Type == PieceType.Pawn)
        {
            if (piece.Position.Row == 7 || piece.Position.Row == 0)
            {
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }
    //public static bool IsCheckmate(ChessBoard chessBoard, PieceColor color)
    //{
    //    //PiecePosition? kingposition = ChessBoard.GetKingPosition(chessBoard, color);
    //    if (IsChecked(chessBoard).Item1)
    //    {
    //        for (int i = 0; i < 8; i++)
    //        {
    //            for (int j = 0; j < 8; j++)
    //            {
    //                var piece = chessBoard[i, j];
    //                if (piece?.Color == color)
    //                {
    //                    for (int k = 0; k < 8; k++)
    //                    {
    //                        for (int l = 0; l < 8; l++)
    //                        {
    //                            if (MoveValidation(chessBoard, piece.Position, new PiecePosition { Row = k, Col = l }, color))
    //                            {
    //                                ChessBoard tempBoard = chessBoard;
    //                                tempBoard[new PiecePosition { Row = k, Col = l }] = tempBoard[piece.Position];
    //                                tempBoard[piece.Position] = null;
    //                                if (!IsChecked(tempBoard).Item1)
    //                                    return false;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        Console.WriteLine("SITUATION IN CHECKMATE");
    //        return true;
    //    }
    //    else
    //        return false;
    //}
    public static bool IsCheckMate(ChessBoard board, PieceColor color)
    {
        PiecePosition? kingposition = ChessBoard.GetKingPosition(board, color);
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece? piece = board[i, j];
                if (IsChecked(board, piece).Item1)
                {
                    var piece1 = IsChecked(board, piece).Item2;
                    if (piece1?.Type == PieceType.Pawn || piece1?.Type == PieceType.Knight)
                    {
                        if (MoveValidation(board, piece.Position, piece1.Position, piece.Color))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        return false;
    }
}
