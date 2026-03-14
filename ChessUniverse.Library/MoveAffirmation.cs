using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;

namespace ChessUniverse.Library
{
    public class MoveAffirmation
    {
        public PiecePosition Start { get; set; }
        public PiecePosition Target { get; set; }
        public (bool, PieceColor) Castling { get; set; }

        public PieceColor Turn;
        public Piece? MovedPiece { get; set; }
        public Piece? CapturedPiece { get; set; }

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

                    if (ChessRules.IsChecked(board).Item1)
                    {
                        Console.WriteLine();
                        Console.WriteLine("        CHECK!!!      ");
                        Console.WriteLine();
                    }

                    return board;
                }


            }
            else if (ChessRules.IsChecked(board).Item1)
            {
                if (ChessRules.MoveValidation(board, Start, end, T))
                {
                    if (Start.Col - end.Col == 2 && piece?.Type == PieceType.King
                        && board[end.Row, end.Col - 2]?.Type == PieceType.Rook)
                    {
                        if (board[Start]?.HasMoved == false
                            && board[end.Row, end.Col - 2]?.HasMoved == false)
                        {
                            board[end] = board[Start];
                            board[Start] = null;
                            var temp = board[end.Row, end.Col - 2];
                            board[end.Row, end.Col + 1] = board[end.Row, end.Col - 2];
                            board[end.Row, end.Col - 2] = null;
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
                            board[Start] = null;
                            board[end.Row, end.Col - 1] = board[end.Row, end.Col + 1];
                            board[end.Row, end.Col + 1] = null;
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

                    if (ChessRules.IsChecked(board).Item1)
                    {
                        MoveBack(board, Start);
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

                    return board;
                }
            }
            else if (ChessRules.IsChecked(board).Item1 && ChessRules.IsChecked(board).Item2 > 1)
            {
                if (ChessRules.MoveValidation(board, Start, end, T) && board[Start]?.Type == PieceType.King)
                {
                    if (Start.Col - end.Col == 2 && piece?.Type == PieceType.King
                        && board[end.Row, end.Col - 2]?.Type == PieceType.Rook)
                    {
                        if (board[Start]?.HasMoved == false
                            && board[end.Row, end.Col - 2]?.HasMoved == false)
                        {
                            board[end] = board[Start];
                            board[Start] = null;
                            var temp = board[end.Row, end.Col - 2];
                            board[end.Row, end.Col + 1] = board[end.Row, end.Col - 2];
                            board[end.Row, end.Col - 2] = null;
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
                            board[Start] = null;
                            board[end.Row, end.Col - 1] = board[end.Row, end.Col + 1];
                            board[end.Row, end.Col + 1] = null;
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

    }
}
