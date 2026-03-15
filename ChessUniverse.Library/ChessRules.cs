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
    //public static (ChessBoard, Piece?, string? newFigure) PawnSwitch(ChessBoard board, Piece? piece)
    //{
    //    Console.Write("INSERT NEW TYPE FOR PAWN: ");
    //    string? newFigure = Console.ReadLine();

    //    if (piece?.Type == PieceType.Pawn)
    //    {
    //        if (piece.Position.Row == 7 || piece.Position.Row == 0)
    //        {
    //            while (newFigure != "Q" || newFigure != "K"
    //                  || newFigure != "T" || newFigure != "B")
    //            {
    //                Console.Write("INSERT NEW TYPE FOR PAWN: ");
    //                newFigure = Console.ReadLine();
    //            }
    //            return (board, piece, newFigure);
    //        }
    //        else
    //            return (board, piece, newFigure);
    //    }
    //    else
    //        return (board, piece, newFigure);
    //}
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
}
