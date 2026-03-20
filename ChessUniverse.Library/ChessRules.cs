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
    public static bool IsInside(int position)
    {
        if (position >= 0 && position < 8)
            return true;
        return false;
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
    public static (bool, Piece?) IsChecked(ChessBoard board, Piece? king)
    {
        //PiecePosition? kingposition = ChessBoard.GetKingPosition(board, PieceColor.White);
        if (king != null)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece1 = board[i, j];
                    if (MoveValidation(board, piece1?.Position, king.Position, piece1?.Color))
                    {
                        return (true, piece1);
                    }
                }
            }
        }
        return (false, null);
    }
    public static bool MoveValidation(ChessBoard? board, PiecePosition? start, PiecePosition? end,
         PieceColor? T)
    {
        if (start is not null && board is not null && board[start] is Piece piece && end is not null)
        {
            Piece? endPiece = board[end];

            if (endPiece == null || piece.Color != endPiece.Color)
            {
                if (T == piece?.Color && piece?.Type == PieceType.King)
                {
                    if (piece.IsMovePossible(board, end))
                    {
                        if (piece?.Color != board[end]?.Color)
                        {
                            //{
                            //        if (!IsChecked(board, end))
                            //            return true;
                            //        else
                            //            return false;
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else if (T == piece?.Color && board[start]?.Type != PieceType.King
                && board[start]!.IsMovePossible(board, end) && board[start]?.Color != board[end]?.Color)
                    return true;
            }
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
    public static bool IsPieceProtected(ChessBoard board, PiecePosition position)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece? piece = board[i, j];
                if (piece != null && piece?.Color == board[position]?.Color)
                {
                    if (piece!.IsMovePossible(board, position))
                        return true;
                }
            }
        }
        return false;
    }
    public static bool IsCheckMate(ChessBoard board, PiecePosition kingposition, PiecePosition attacker)
    {
        int k = 0;
        List<PiecePosition> kingmoves = new List<PiecePosition>();
        List<PiecePosition> attackermoves = new List<PiecePosition>();
        if (Math.Abs(attacker.Row - kingposition.Row) > 1 || Math.Abs(attacker.Col - kingposition.Col) > 1)
        {
            //1
            if (attacker.Row < kingposition.Row && attacker.Col < kingposition.Col)
            {
                for (int i = 1; i < Math.Abs(kingposition.Row - attacker.Row); i++)
                {
                    for (int j = 1; j < Math.Abs(kingposition.Col - attacker.Col); j++)
                    {
                        if (i == j)
                            attackermoves.Add(new PiecePosition { Row = attacker.Row + i, Col = attacker.Col + j });
                    }
                }
            }
            //2
            if (attacker.Row < kingposition.Row && attacker.Col == kingposition.Col)
            {
                for (int i = 1; i < Math.Abs(kingposition.Row - attacker.Row); i++)
                    attackermoves.Add(new PiecePosition { Row = attacker.Row + i, Col = attacker.Col });
            }
            //3
            if (attacker.Row < kingposition.Row && attacker.Col > kingposition.Col)
            {
                for (int i = 1; i < Math.Abs(kingposition.Row - attacker.Row); i++)
                {
                    for (int j = 1; j < Math.Abs(kingposition.Col - attacker.Col); j++)
                    {
                        if (i == j)
                            attackermoves.Add(new PiecePosition { Row = attacker.Row + i, Col = attacker.Col - j });
                    }
                }
            }
            //4
            if (attacker.Row == kingposition.Row && attacker.Col < kingposition.Col)
            {
                for (int j = 1; j < Math.Abs(kingposition.Col - attacker.Col); j++)
                    attackermoves.Add(new PiecePosition { Row = attacker.Row, Col = attacker.Col + j });
            }
            //5
            if (attacker.Row == kingposition.Row && attacker.Col > kingposition.Col)
            {
                for (int j = 1; j < Math.Abs(kingposition.Col - attacker.Col); j++)
                    attackermoves.Add(new PiecePosition { Row = attacker.Row, Col = attacker.Col - j });
            }
            //6
            if (attacker.Row > kingposition.Row && attacker.Col < kingposition.Col)
            {
                for (int i = 1; i < Math.Abs(kingposition.Row - attacker.Row); i++)
                {
                    for (int j = 1; j < Math.Abs(kingposition.Col - attacker.Col); j++)
                    {
                        if (i == j)
                            attackermoves.Add(new PiecePosition { Row = attacker.Row - i, Col = attacker.Col + j });
                    }
                }
            }
            //7
            if (attacker.Row > kingposition.Row && attacker.Col == kingposition.Col)
            {
                for (int i = 1; i < Math.Abs(kingposition.Row - attacker.Row); i++)
                    attackermoves.Add(new PiecePosition { Row = attacker.Row - i, Col = attacker.Col });
            }
            //8
            if (attacker.Row > kingposition.Row && attacker.Col > kingposition.Col)
            {
                for (int i = 1; i < Math.Abs(kingposition.Row - attacker.Row); i++)
                {
                    for (int j = 1; j < Math.Abs(kingposition.Col - attacker.Col); j++)
                    {
                        if (i == j)
                            attackermoves.Add(new PiecePosition { Row = attacker.Row - i, Col = attacker.Col - j });
                    }
                }
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = board[i, j];
                    if (piece != null && piece.Color == board[kingposition]!.Color)
                    {
                        foreach (var item in attackermoves)
                        {
                            if (piece.Color == board[kingposition]?.Color
                            && MoveValidation(board, piece.Position, item, board[kingposition]!.Color))
                                return false;
                        }
                    }
                }
            }
            return true;
        }
        else
        {
            if (IsInside(kingposition.Row + 1))
            {
                kingmoves.Add(new PiecePosition { Row = kingposition.Row + 1, Col = kingposition.Col });
                k++;
            }
            if (IsInside(kingposition.Row + 1) && IsInside(kingposition.Col + 1))
            {
                kingmoves.Add(new PiecePosition { Row = kingposition.Row + 1, Col = kingposition.Col + 1 });
                k++;
            }
            if (IsInside(kingposition.Row + 1) && IsInside(kingposition.Col - 1))
            {
                kingmoves.Add(new PiecePosition { Row = kingposition.Row + 1, Col = kingposition.Col - 1 });
                k++;
            }
            if (IsInside(kingposition.Col + 1))
            {
                kingmoves.Add(new PiecePosition { Row = kingposition.Row, Col = kingposition.Col + 1 });
                k++;
            }
            if (IsInside(kingposition.Col - 1))
            {
                kingmoves.Add(new PiecePosition { Row = kingposition.Row, Col = kingposition.Col - 1 });
                k++;
            }
            if (IsInside(kingposition.Row - 1))
            {
                kingmoves.Add(new PiecePosition { Row = kingposition.Row - 1, Col = kingposition.Col });
                k++;
            }
            if (IsInside(kingposition.Row - 1) && IsInside(kingposition.Col + 1))
            {
                kingmoves.Add(new PiecePosition { Row = kingposition.Row - 1, Col = kingposition.Col + 1 });
                k++;
            }
            if (IsInside(kingposition.Row - 1) && IsInside(kingposition.Col - 1))
            {
                kingmoves.Add(new PiecePosition { Row = kingposition.Row - 1, Col = kingposition.Col - 1 });
                k++;
            }
            foreach (var move in kingmoves)
            {
                ChessBoard newBoard = new ChessBoard();
                newBoard = (ChessBoard)board.Clone();
                Piece.SwitchPositions(newBoard, kingposition, move);
                if (MoveValidation(board, kingposition, move, board[kingposition]?.Color) && !IsChecked(newBoard).Item1)
                {
                    k--;
                }
            }

            if (k == 0)
                return true;
        }
        return false;
    }
    public static bool IsStaleMate(ChessBoard board, PieceColor T)
    {
        int pieceCount = ChessBoard.GetPiecePositions(board, T).Count;
        foreach (var item in ChessBoard.GetPiecePositions(board, T))
        {
            if (board[item]?.GetPossibleMoves(board).Count == 0)
                pieceCount--;
        }
        if (pieceCount == 0)
            return true;
        else
            return false;
    }
}
