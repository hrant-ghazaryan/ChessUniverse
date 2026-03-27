using ChessUniverse.Library.Enums;
using System.Security.Cryptography.X509Certificates;

namespace ChessUniverse.Library;

public static class ChessRules
{
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
    //checkk
    public static bool IsChecked(ChessBoard chessBoard, PiecePosition position)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var piece = chessBoard[i, j];
                var pieceparam = chessBoard[position];
                if (piece is not null )
                {
                    if (piece?.Color == PieceColor.White && pieceparam?.Color == PieceColor.Black
                    && piece.IsMovePossible(chessBoard, position))
                        return true;
                    if (piece?.Color == PieceColor.Black && pieceparam?.Color == PieceColor.White
                    && piece.IsMovePossible(chessBoard, position))
                        return true;
                }
            }
        }
        return false;
    }
    public static bool MoveValidation(ChessBoard? board, PiecePosition? start, PiecePosition? end,
         PieceColor? T)
    {
        if (start is not null && board is not null && board[start] is Piece piece && end is not null)
        {
            Piece? endPiece = board[end];

            if (T == piece?.Color)
            {
                if (endPiece == null || piece?.Color != endPiece.Color)
                {
                    if (piece?.Type == PieceType.King && piece!.IsMovePossible(board, end) && !IsChecked(board, end))
                        return true;
                    else if (piece!.IsMovePossible(board, end))
                        return true;
                }
            }
        }
        return false;
    }
    public static bool PawnPromotion(ChessBoard board, PiecePosition start)
    {
        Piece? piece = board[start];

        if (piece?.Type == PieceType.Pawn)
            return (piece.Position.Row == 7 || piece.Position.Row == 0);
        else
            return false;
    }
    public static bool IsCheckMate(ChessBoard board, PiecePosition kingposition, PiecePosition attacker)
    {
        int safeMovesCount = 0;
        List<PiecePosition> kingmoves = new List<PiecePosition>();
        List<PiecePosition> attackermoves = new List<PiecePosition>();
        if (Math.Abs(attacker.Row - kingposition.Row) == Math.Abs(attacker.Col - kingposition.Col) &&
            Math.Abs(attacker.Row - kingposition.Row) > 1 && Math.Abs(attacker.Col - kingposition.Col) > 1 )
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
            ChessBoard newBoard = new ChessBoard();
            newBoard = (ChessBoard)board.Clone();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = board[i, j];
                    {
                        if (piece != null && piece.Color == board[kingposition]!.Color)
                        {
                            foreach (var move in attackermoves)
                            {
                                if (MoveValidation(board, piece.Position, attacker, board[kingposition]?.Color))
                                    return false;
                                else if (MoveValidation(newBoard, piece.Position, move, board[kingposition]?.Color))
                                {
                                    Piece.SwitchPositions(newBoard, piece.Position, move);

                                    if (!IsChecked(newBoard, kingposition))
                                        return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;

        }
        else
        {
            int[] dr = [1, 1, 1, 0, 0, -1, -1, -1];
            int[] dc = [-1, 0, 1, -1, 1, -1, 0, 1];

            for (int i = 0; i < 8; i++)
            {
                int newRow = kingposition.Row + dr[i];
                int newCol = kingposition.Col + dc[i];

                if (IsInside(newRow) && IsInside(newCol))
                {
                    kingmoves.Add(new PiecePosition { Row = newRow, Col = newCol });
                    safeMovesCount++;
                }
            }
            
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = board[i, j];
                    if (piece != null && piece.IsMovePossible(board, attacker))
                        return false;

                    foreach (var move in kingmoves)
                    {
                        ChessBoard newBoard = new ChessBoard();
                        newBoard = (ChessBoard)board.Clone();
                        Piece.SwitchPositions(newBoard, kingposition, move);
                        if (IsChecked(newBoard, move))
                            safeMovesCount--;
                    }
                }
            }

            if (safeMovesCount == 0)
                return true;
        }
        return false;
    }
    public static bool IsStaleMate(ChessBoard board, PieceColor T)
    {

        List<PiecePosition> allTPieces = ChessBoard.GetAllPiecePositions(board, T);
        int pieceCount = allTPieces.Count;
        foreach (var item in allTPieces)
        {
            if (board[item]?.GetPossibleMoves(board).Item2 == true)
                return false;
        }
        return true;

        
    }

    
}
