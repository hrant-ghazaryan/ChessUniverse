using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;

namespace ChessUniverse.Library;

public class MoveAffirmation
{
    public PiecePosition Start { get; set; }
    public PiecePosition Target { get; set; }
    public (bool, PieceColor) Castling { get; set; }
    public Piece? MovedPiece { get; set; }
    public Piece? CapturedPiece { get; set; }
    public PieceColor T { get; set; }
    public Game game = new Game();

    public MoveAffirmation(PiecePosition start, PiecePosition target)
    {
        Start = start;
        Target = target;
    }
    public MoveAffirmation(PiecePosition start, PiecePosition target, Piece movedPiece, Piece? capturedFigure, (bool,PieceColor) castling)
    {
        Start = start;
        Target = target;
        MovedPiece = movedPiece;
        CapturedPiece = capturedFigure;
        Castling = castling;
    }

    public ChessBoard? MoveBack(ChessBoard board, PiecePosition start, ref PieceColor T)
    {
        if (board[Target] != null)
        {
            Piece piece = board[Target]!;
            board[Start] = piece;

            if (Game.moves?[Game.moves.Count - 1].CapturedPiece != null)
                board[Target] = Game.moves?[Game.moves.Count - 1].CapturedPiece;
            else
                board[Target] = null; 

            if (T == PieceColor.White)
                T = PieceColor.Black;
            else
                T = PieceColor.White;
            Console.WriteLine();
            Console.WriteLine(" INVALID MOVE! ");
            Console.WriteLine();
            return board;
            
        }
        return null;
    }
    public MoveAffirmation Move(ChessBoard board, PiecePosition end, ref PieceColor T)
    {
        Piece? piece = board[Start];
        Piece? targetPiece = board[end];

        if (ChessRules.IsChecked(board).Item1 == false)
        {
            if (ChessRules.MoveValidation(board, Start, end, T))
            {
                if (CastlingLeft(board, end, ref T).Item2)
                {
                    Game.moves?.Add(new MoveAffirmation(Start, Target, piece!, board[end], Castling));
                    return new MoveAffirmation(Start, Target, piece!, board[end], Castling);
                }
                if (CastlingRight(board, end, ref T).Item2)
                {
                    Game.moves?.Add(new MoveAffirmation(Start, Target, piece!, board[end], Castling));
                    return new MoveAffirmation(Start, Target, piece!, board[end], Castling);
                }
                RegularMove(board, end, ref T);
                PawnPromotionMove(board, end);
                Game.moves?.Add(new MoveAffirmation(Start, Target, piece!, targetPiece, Castling));

                if (ChessRules.IsChecked(board).Item1)
                {
                    PiecePosition? kingposition = ChessBoard.GetKingPosition(board, T);
                    if (ChessRules.IsCheckMate(board, kingposition))
                    {
                        Console.WriteLine("        CHECKMATE!!!      ");
                        return new MoveAffirmation(Start, Target, piece!, board[end], Castling);
                    }
                }
                else if (ChessRules.IsChecked(board).Item1)
                    Console.WriteLine("        CHECK!!!      ");

                return new MoveAffirmation(Start, Target, piece!, board[end], Castling);
            }

        }
        else if (ChessRules.IsChecked(board).Item1)
        {
            if (ChessRules.MoveValidation(board, Start, end, T))
            {
                RegularMove(board, end, ref T);

                if (ChessRules.IsChecked(board).Item1)
                    MoveBack(board, Start, ref T);
                else
                    Game.moves?.Add(new MoveAffirmation(Start, Target, piece!, board[end], Castling));

                if (ChessRules.IsChecked(board).Item1)
                    Console.WriteLine("        CHECK!!!      ");
                return new MoveAffirmation(Start, Target, piece!, board[end], Castling);
            }

        }
        else if (ChessRules.IsChecked(board).Item1 && ChessRules.IsChecked(board).Item2 > 1)
        {
            if (ChessRules.MoveValidation(board, Start, end, T) && board[Start]?.Type == PieceType.King)
            {
                RegularMove(board, end, ref T);

                if (ChessRules.IsChecked(board).Item1)
                    MoveBack(board, Start, ref T);
                else
                    Game.moves?.Add(new MoveAffirmation(Start, Target, piece!, board[end], Castling));
            }
        }
        return new MoveAffirmation(Start, Target, piece!, board[end], Castling);
    }
    public void RegularMove(ChessBoard board, PiecePosition end, ref PieceColor T)
    {
        Piece? piece = board[Start];
        board[end] = piece;
        board[Start] = null;
        piece?.Position = end;
        piece?.HasMoved = true;
        if (T == PieceColor.White)
            T = PieceColor.Black;
        else
            T = PieceColor.White;
    }
    public (ChessBoard,bool) CastlingLeft(ChessBoard board, PiecePosition end, ref PieceColor T)
    {
        Piece? piece = board[Start];
        if (Start.Col - end.Col == 2 && piece?.Type == PieceType.King
                    && board[end.Row, end.Col - 2]?.Type == PieceType.Rook)
        {
            if (board[Start]?.HasMoved == false
                && board[end.Row, end.Col - 2]?.HasMoved == false)
            {
                board[end] = board[Start];
                board[end]?.Position = end;
                board[Start] = null;
                board[end.Row, end.Col + 1] = board[end.Row, end.Col - 2];
                board[end.Row, end.Col + 1]?.Position =
                    new PiecePosition { Row = end.Row, Col = end.Col + 1 }; board[end.Row, end.Col - 2] = null;
                Castling = (true, piece.Color);
                if (ChessRules.IsChecked(board).Item1)
                {
                    MoveBack(board, Start, ref T);
                    return (board, false);
                }
                if (T == PieceColor.White)
                    T = PieceColor.Black;
                else
                    T = PieceColor.White;
                return (board,true);
            }
        }
        return (board,false);
    }
    public (ChessBoard,bool) CastlingRight(ChessBoard board, PiecePosition end, ref PieceColor T)
    {
        Piece? piece = board[Start];
        if (end.Col - Start.Col == 2 && piece?.Type == PieceType.King
                    && board[end.Row, end.Col + 1]?.Type == PieceType.Rook)
        {
            if (board[Start]?.HasMoved == false
                && board[end.Row, end.Col + 1]?.HasMoved == false)
            {
                board[end] = board[Start];
                board[end]?.Position = end;
                board[Start] = null;
                board[end.Row, end.Col - 1] = board[end.Row, end.Col + 1];
                board[end.Row, end.Col - 1]?.Position =
                    new PiecePosition { Row = end.Row, Col = end.Col - 1 }; board[end.Row, end.Col + 1] = null;
                Castling = (true, piece.Color);
                if (ChessRules.IsChecked(board).Item1)
                {
                    MoveBack(board, Start, ref T);
                    return (board,false);
                }
                if (T == PieceColor.White)
                    T = PieceColor.Black;
                else
                    T = PieceColor.White;
                return (board,true);
            }
        }
        return (board,false);
    }
    public void PawnPromotionMove(ChessBoard board, PiecePosition end)
    {
        if (ChessRules.PawnPromotion(board, end))
        {
            Console.Write("INSERT NEW TYPE FOR PAWN: ");
            string? newFigure = Console.ReadLine();
            Piece newPiece = newFigure switch
            {
                "T" => new Knight(board[end].Color),
                "B" => new Bishop(board[end].Color),
                "R" => new Rook(board[end].Color),
                "Q" => new Queen(board[end].Color),
                _ => throw new Exception()
            };

            newPiece.Position = end;
            newPiece.Symbol = newPiece.GetSymbol(newPiece.Color);

            board[end] = newPiece;
        }
    }
}
