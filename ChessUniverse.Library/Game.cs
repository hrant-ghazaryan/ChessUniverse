using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;

namespace ChessUniverse.Library;

public class Game
{
    public List<MoveInfo> moves = new List<MoveInfo>();
    public bool Move(ref ChessBoard board, ref MoveInfo moveInfo)
    {
        ChessBoard newBoard = (ChessBoard)board.Clone();
        PieceColor tempT = moveInfo.T;

        if (ChessRules.MoveValidation(board, moveInfo.Start, moveInfo.Target, moveInfo.T))
        {
            CastlingLeft(newBoard, moveInfo);
            CastlingRight(newBoard, moveInfo);
            RegularMove(newBoard, moveInfo);
        }
        else
        {
            Console.WriteLine("Invalid Move:  !!!");
            return false;
        }

        PiecePosition? TKing = ChessBoard.GetKingPosition(newBoard, moveInfo.T);
        if (TKing is not null && newBoard[TKing] is not null)
        {
            if (ChessRules.IsChecked(newBoard, TKing))
            {
                Console.WriteLine(" Invalid Move:  Check way was opened! ");
                return false;
            }

            else
            {
                board = (ChessBoard)newBoard;
                moves?.Add(new MoveInfo(moveInfo.Start, moveInfo.Target, board[moveInfo.Start]!, board[moveInfo.Target], moveInfo.Castling));
                moveInfo = TurnSwitcher(moveInfo);
                PiecePosition? lastCheck = ChessBoard.GetKingPosition(board, moveInfo.T);
                if (lastCheck is not null && ChessRules.IsCheckMate(board, lastCheck, moveInfo.Target))
                    Console.WriteLine("        CHECKMATE!!!      ");
                else if (lastCheck is not null && ChessRules.IsChecked(board, lastCheck))
                    Console.WriteLine("        CHECK!!!      ");
                return true;
            }
        }
        else
        {
            Console.WriteLine(" Invalid Move!!! ");
            return false;
        }

    }
    public void RegularMove(ChessBoard board, MoveInfo moveInfo)
    {
        if (moveInfo.Start is not null && moveInfo.Target is not null)
        {
            Piece? piece = board[moveInfo.Start];
            board[moveInfo.Target] = piece;
            board[moveInfo.Start] = null;
            piece?.Position = moveInfo.Target;
            piece?.HasMoved = true;
            //PawnPromotionMove(board, moveInfo);
        }
    }
    public void PawnPromotionMove(ChessBoard board, MoveInfo moveInfo)
    {
        if (ChessRules.PawnPromotion(board, moveInfo.Target))
        {
            Console.Write("INSERT NEW TYPE FOR PAWN: Q | R | B | T ");
            string? newFigure = Console.ReadLine();
            if (board[moveInfo.Target] is Piece pawn)
            {
                Piece newPiece = newFigure switch
                {
                    "T" => new Knight(pawn.Color),
                    "B" => new Bishop(pawn.Color),
                    "R" => new Rook(pawn.Color),
                    "Q" => new Queen(pawn.Color),
                    _ => throw new Exception()
                };
                newPiece.Position = moveInfo.Target;
                newPiece.Symbol = newPiece.GetSymbol(newPiece.Color);

                board[moveInfo.Target] = newPiece;
            }
        }
    }
    private MoveInfo TurnSwitcher(MoveInfo moveInfo)
    {
        if (moveInfo.T == PieceColor.White)
            moveInfo.T = PieceColor.Black;
        else
            moveInfo.T = PieceColor.White;
        return moveInfo;
    }
    public static (ChessBoard, bool) CastlingLeft(ChessBoard chessBoard, MoveInfo moveInfo)
    {
        var color = chessBoard[moveInfo.Start].Color;
        //Թագավորի դիրքի փոփոխություն – 2 դիրք ձախ.
        chessBoard[moveInfo.Target] = chessBoard[moveInfo.Start];
        chessBoard[moveInfo.Target]?.Position = moveInfo.Target;
        chessBoard[moveInfo.Start] = null;

        //Նավակի դիրքի փոփոփխություն – 3 դիրք աջ.
        chessBoard[moveInfo.Target.Row, moveInfo.Target.Col + 1] = chessBoard[moveInfo.Target.Row, moveInfo.Target.Col - 2];
        chessBoard[moveInfo.Target.Row, moveInfo.Target.Col + 1]?.Position =
            new PiecePosition { Row = moveInfo.Target.Row, Col = moveInfo.Target.Col + 1 };
        chessBoard[moveInfo.Target.Row, moveInfo.Target.Col - 2] = null;

        //Castling property ի փոփոխություն․
        moveInfo.Castling = (true, color);

        return (chessBoard, true);
    }
    public static (ChessBoard, bool) CastlingRight(ChessBoard chessBoard, MoveInfo moveInfo)
    {
        if (chessBoard[moveInfo.Start]?.Type != PieceType.King ||
            (ChessRules.IsInside(moveInfo.Target.Col + 1) && chessBoard[moveInfo.Target.Row, moveInfo.Target.Col + 1]?.Type != PieceType.Rook))
            return (chessBoard, false);

        var color = chessBoard[moveInfo.Start].Color;
        if (ChessRules.IsCastlingRightPossible(chessBoard, moveInfo))
        {
            chessBoard[moveInfo.Target] = chessBoard[moveInfo.Start];
            chessBoard[moveInfo.Target]?.Position = moveInfo.Target;
            chessBoard[moveInfo.Start] = null;
            chessBoard[moveInfo.Target.Row, moveInfo.Target.Col - 1] = chessBoard[moveInfo.Target.Row, moveInfo.Target.Col + 1];
            chessBoard[moveInfo.Target.Row, moveInfo.Target.Col - 1]?.Position =
                new PiecePosition { Row = moveInfo.Target.Row, Col = moveInfo.Target.Col - 1 }; chessBoard[moveInfo.Target.Row, moveInfo.Target.Col + 1] = null;
            moveInfo.Castling = (true, color);
            return (chessBoard, true);
        }
        else
            return (chessBoard, false);
    }
}
