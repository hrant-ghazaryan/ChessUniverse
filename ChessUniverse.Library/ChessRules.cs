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
    public static (bool, Piece?) IsChecked(ChessBoard board, Piece? piece)
    {
        PiecePosition? kingposition = ChessBoard.GetKingPosition(board, PieceColor.White);

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var piece1 = board[i, j];
                if (MoveValidation(board, piece1?.Position, kingposition, piece1.Color))
                {
                    return (true, piece1);
                }
            }
        }
        return (false, null);
    }
    public static bool MoveValidation(ChessBoard? board, PiecePosition? start, PiecePosition? end,
         PieceColor? T)
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
    //public static bool IsCheckMate(ChessBoard board, PieceColor color)
    //{
    //    List<PiecePosition> movestoking = new List<PiecePosition>();
    //    PiecePosition? kingposition = ChessBoard.GetKingPosition(board, color);
    //    for (int i = 0; i < 8; i++)
    //    {
    //        for (int j = 0; j < 8; j++)
    //        {
    //            Piece? piece = board[i, j];
    //            if (piece.Color != color)
    //            {
    //                if (IsChecked(board, piece).Item1)
    //                {
    //                    var piece1 = IsChecked(board, piece).Item2;
    //                    if (piece1?.Type == PieceType.Pawn || piece1?.Type == PieceType.Knight)
    //                    {
    //                        if (MoveValidation(board, piece.Position, piece1.Position, piece.Color))
    //                        {
    //                            return false;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if (piece1!.Position.Row < kingposition!.Row && piece1.Position.Col > kingposition.Col)
    //                        {
    //                            for (i = 1; i < Math.Abs(piece1.Position.Row - kingposition.Row); i++)
    //                            {
    //                                for (i = 1; j < Math.Abs(piece1.Position.Col - kingposition.Col); j++)
    //                                {
    //                                    if (i == j)
    //                                        movestoking.Add(board[kingposition.Row + i, kingposition.Col - j].Position);
    //                                }
    //                            }
    //                        }
    //                        else if (piece1.Position.Row < kingposition.Row && piece1.Position.Col < kingposition.Col)
    //                        {
    //                            for (i = 1; i < Math.Abs(piece1.Position.Row - kingposition.Row); i++)
    //                            {
    //                                for (i = 1; j < Math.Abs(piece1.Position.Col - kingposition.Col); j++)
    //                                {
    //                                    if (i == j)
    //                                    {
    //                                        movestoking.Add(board[kingposition.Row + i, kingposition.Col + j].Position);
    //                                    }
    //                                }
    //                            }
    //                        }
    //                        else if (piece1.Position.Row > kingposition.Row && piece1.Position.Col < kingposition.Col)
    //                        {
    //                            for (i = 1; i < Math.Abs(piece1.Position.Row - kingposition.Row); i++)
    //                            {
    //                                for (i = 1; j < Math.Abs(piece1.Position.Col - kingposition.Col); j++)
    //                                {
    //                                    if (i == j)
    //                                    {
    //                                        movestoking.Add(board[kingposition.Row - i, kingposition.Col + j].Position);
    //                                    }
    //                                }
    //                            }
    //                        }
    //                        else if (piece1.Position.Row > kingposition.Row && piece1.Position.Col > kingposition.Col)
    //                        {
    //                            for (i = 1; i < Math.Abs(piece1.Position.Row - kingposition.Row); i++)
    //                            {
    //                                for (i = 1; j < Math.Abs(piece1.Position.Col - kingposition.Col); j++)
    //                                {
    //                                    if (i == j)
    //                                    {
    //                                        movestoking.Add(board[kingposition.Row - i, kingposition.Col - j].Position);
    //                                    }
    //                                }
    //                            }
    //                        }
    //                        else if (piece1.Position.Row == kingposition.Row || piece1.Position.Col == kingposition.Col)
    //                        {
    //                            if (piece1.Position.Row == kingposition.Row && piece1.Position.Col > kingposition.Col)
    //                            {
    //                                for (j = 1; j < Math.Abs(piece1.Position.Col - kingposition.Col); j++)
    //                                {
    //                                    movestoking.Add(board[piece.Position.Row, piece.Position.Col - j].Position);

    //                                }
    //                            }
    //                            else if (piece1.Position.Row == kingposition.Row && piece1.Position.Col < kingposition.Col)
    //                            {
    //                                for (j = 1; j < Math.Abs(piece1.Position.Col - kingposition.Col); j++)
    //                                {
    //                                    movestoking.Add(board[piece.Position.Row, piece.Position.Col + j].Position);
    //                                }
    //                            }
    //                            else if (piece1.Position.Row > kingposition.Row && piece1.Position.Col == kingposition.Col)
    //                            {
    //                                for (j = 1; j < Math.Abs(piece1.Position.Row - kingposition.Row); j++)
    //                                {
    //                                    movestoking.Add(board[piece.Position.Row - j, piece.Position.Col].Position);

    //                                }
    //                            }
    //                            else if (piece1.Position.Row < kingposition.Row && piece1.Position.Col == kingposition.Col)
    //                            {
    //                                for (j = 1; j < Math.Abs(piece1.Position.Row - kingposition.Row); j++)
    //                                {
    //                                    movestoking.Add(board[piece.Position.Row + j, piece.Position.Col].Position);
    //                                }
    //                            }
    //                            return true;
    //                        }
    //                    }
    //                }
    //            }
                
    //            foreach (var item in movestoking)
    //            {
    //                if (MoveValidation(board, piece.Position, item, piece.Color))
    //                    return false;
    //                else
    //                {
    //                    Console.WriteLine("SITUATION IS CHECKMATE");
    //                    return true;
    //                }
    //            }
    //            return false;
    //        }
    //    }
    //    return false;
    //}
    public static bool IsCheckMate(ChessBoard board, PiecePosition start)
    {
        int k = 0;
        List<PiecePosition> kingmoves = new List<PiecePosition>();
        PiecePosition? kingposition = ChessBoard.GetKingPosition(board, board[start].Color);
        if (kingposition.Row + 1 > 0 && kingposition.Row + 1 < 8)
        {
            kingmoves.Add(new PiecePosition { Row = kingposition.Row + 1, Col = kingposition.Col });
            k++;
        }
        if (kingposition.Row + 1 > 0 && kingposition.Row + 1 < 8 && kingposition.Col + 1 > 0 && kingposition.Col + 1 < 8)
        { kingmoves.Add(new PiecePosition { Row = kingposition.Row + 1, Col = kingposition.Col + 1 }); k++; }
            
        if (kingposition.Row + 1 > 0 && kingposition.Row + 1 < 8 && kingposition.Col - 1 > 0 && start.Col - 1 < 8)
{            kingmoves.Add(new PiecePosition { Row = kingposition.Row + 1, Col = kingposition.Col - 1 });k++;
}        if (kingposition.Col + 1 > 0 && kingposition.Col + 1 < 8 )
{            kingmoves.Add(new PiecePosition { Row = kingposition.Row, Col = kingposition.Col + 1 });k++;
}        if (kingposition.Col - 1 > 0 && kingposition.Col - 1 < 8)
{            kingmoves.Add(new PiecePosition { Row = kingposition.Row, Col = kingposition.Col - 1 });k++;
}        if (kingposition.Row - 1 > 0 && kingposition.Row - 1 < 8)
{            kingmoves.Add(new PiecePosition { Row = kingposition.Row - 1, Col = kingposition.Col });k++;
}        if (kingposition.Row - 1 > 0 && kingposition.Row - 1 < 8 && kingposition.Col + 1 > 0 && kingposition.Col + 1 < 8)
{            kingmoves.Add(new PiecePosition { Row = kingposition.Row - 1, Col = kingposition.Col + 1 });k++;
}        if (kingposition.Row - 1 > 0 && kingposition.Row - 1 < 8 && kingposition.Col - 1 > 0 && kingposition.Col - 1 < 8)
{            kingmoves.Add(new PiecePosition { Row = kingposition.Row - 1, Col = kingposition.Col - 1 });k++;
}        foreach (var move in kingmoves)
        {
            if (!MoveValidation(board, kingposition, move, board[kingposition]?.Color))
                k--;
        }
        if (k == 0)
            return true;
        return false;
    }
}
