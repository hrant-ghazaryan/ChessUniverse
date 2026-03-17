using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;
using Microsoft.Extensions.Logging;

namespace ChessUniverse.Library;

public class MoveAffirmation
{
    public PiecePosition Start { get; set; }
    public PiecePosition Target { get; set; }
    public (bool, PieceColor) Castling { get; set; }
    public Piece? MovedPiece { get; set; }
    public Piece? CapturedPiece { get; set; }


    private static readonly ILogger logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("MoveAffirmation");

    public List<ChessBoard> moves = new List<ChessBoard>();

    public MoveAffirmation(PiecePosition start, PiecePosition target)
    {
        Start = start;
        Target = target;
    }
    public ChessBoard? MoveBack(ChessBoard board, PiecePosition start)
    {
        if (board[Target] != null)
        {
            Piece piece = board[Target]!;
            board[Start] = piece;
            board[Target] = null;
            return board;
        }
        return null;
    }
    public ChessBoard Move(ChessBoard board, PiecePosition end, ref PieceColor T)
    {
        Piece? piece = board[Start];

        if (ChessRules.IsChecked(board).Item1 == false)
        {
            if (ChessRules.MoveValidation(board, Start, end, T))
            {
                logger.LogInformation($"{piece?.GetType().Name}: {Start.ToString()} - {end.ToString()}");
                //CastlingLeft(board, end, ref T);
                //CastlingRight(board, end, ref T);
                //RegularMove(board, end, ref T);
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
                            MoveBack(board, Start);
                        moves.Add(board);
                        if (T == PieceColor.White)
                            T = PieceColor.Black;
                        else
                            T = PieceColor.White;
                        return board;
                    }
                }
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
                            MoveBack(board, Start);
                        if (T == PieceColor.White)
                            T = PieceColor.Black;
                        else
                            T = PieceColor.White;
                        return board;
                    }
                }

                board[end] = piece;
                board[Start] = null;
                piece?.Position = end;
                piece?.HasMoved = true;
                if (T == PieceColor.White)
                    T = PieceColor.Black;
                else
                    T = PieceColor.White;

                if (ChessRules.PawnPromotion(board, end))
                {
                    Console.Write("INSERT NEW TYPE FOR PAWN: ");
                    string? newFigure = Console.ReadLine();

                    switch (newFigure)
                    {
                        case "T":
                            Piece knight = new Knight(piece.Color) { Position = end, Type = PieceType.Knight };
                            board[end] = null;
                            board[end] = knight;
                            board[end]?.Symbol = knight.GetSymbol(piece.Color);
                            break;
                        case "B":
                            Piece bishop = new Bishop(piece.Color) { Position = end, Type = PieceType.Bishop };
                            board[end] = null;
                            board[end] = bishop;
                            board[end]?.Symbol = bishop.GetSymbol(piece.Color);
                            break;
                        case "R":
                            Piece rook = new Rook(piece.Color) { Position = end, Type = PieceType.Rook };
                            board[end] = null;
                            board[end] = rook;
                            board[end]?.Symbol = rook.GetSymbol(piece.Color);
                            break;
                        case "Q":
                            Piece queen = new Queen(piece.Color) { Position = end, Type = PieceType.Queen };
                            board[end] = null;
                            board[end] = queen;
                            board[end]?.Symbol = queen.GetSymbol(piece.Color);
                            break;
                    }
                }

                if (ChessRules.IsCheckMate(board, T))
                {
                    Console.WriteLine();
                    Console.WriteLine("        CHECK!!!      ");
                    Console.WriteLine();
                }
                if (ChessRules.IsChecked(board).Item1)
                {
                    Console.WriteLine();
                    Console.WriteLine("        CHECK!!!      ");
                    Console.WriteLine();
                }

                return board;
            }
            else
                logger.LogInformation($"INVALID MOVE!!!");

        }
        else if (ChessRules.IsChecked(board).Item1)
        {
            if (ChessRules.MoveValidation(board, Start, end, T))
            {
                //RegularMove(board, end, ref T);

                board[end] = piece;
                board[Start] = null;
                piece?.Position = end;
                piece?.HasMoved = true;

                if (ChessRules.IsChecked(board).Item1)
                {
                    MoveBack(board, Start);
                    if (T == PieceColor.White)
                        T = PieceColor.Black;
                    else
                        T = PieceColor.White;
                    Console.WriteLine();
                    Console.WriteLine("YOU ARE IN CHECK , PROTECT YOUR KING");
                    Console.WriteLine();
                }
                else
                {
                    if (T == PieceColor.White)
                        T = PieceColor.Black;
                    else
                        T = PieceColor.White;
                }
                if (ChessRules.IsChecked(board).Item1)
                {
                    Console.WriteLine();
                    Console.WriteLine("        CHECK!!!      ");
                    Console.WriteLine();
                }
                return board;
            }
        }
        else if (ChessRules.IsChecked(board).Item1 && ChessRules.IsChecked(board).Item2 > 1)
        {
            if (ChessRules.MoveValidation(board, Start, end, T) && board[Start]?.Type == PieceType.King)
            {
                RegularMove(board, end, ref T);
                //board[end] = piece;
                //board[Start] = null;
                //piece?.Position = end;
                //piece?.HasMoved = true;
                //if (T == PieceColor.White)
                //    T = PieceColor.Black;
                //else
                //    T = PieceColor.White;

                if (ChessRules.IsChecked(board).Item1)
                {
                    MoveBack(board, Start);
                    Console.WriteLine();
                    Console.WriteLine("YOU ARE IN CHECK , PROTECT YOUR KING");
                    Console.WriteLine();
                }
            }
        }
        return board;

    }
    public ChessBoard CastlingLeft(ChessBoard board, PiecePosition end, ref PieceColor T)
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
                    MoveBack(board, Start);
                moves.Add(board);
                if (T == PieceColor.White)
                    T = PieceColor.Black;
                else
                    T = PieceColor.White;
                return board;
            }
        }
        return board;
    }
    public ChessBoard CastlingRight(ChessBoard board, PiecePosition end, ref PieceColor T)
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
                    MoveBack(board, Start);
                if (T == PieceColor.White)
                    T = PieceColor.Black;
                else
                    T = PieceColor.White;
                return board;
            }
        }
        return board;
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
}
