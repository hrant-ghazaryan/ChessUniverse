//using ChessUniverse.Library.Enums;

//namespace ChessUniverse.Library;

//public delegate MoveAffirmation MoveDelegate(MoveAffirmation movedatails);
//public class MoveAffirmation
//{
//    //public PiecePosition Start { get; set; }
//    //public PiecePosition Target { get; set; }
//    //public (bool, PieceColor) Castling { get; set; }
//    //public Piece? MovedPiece { get; set; }
//    //public Piece? CapturedPiece { get; set; }
//    //public PieceColor T { get; set; }
//    //public Game game = new Game();

//    //Մեթոդների ցուցակ որոնք պետք է կանչվեն քայլը կատարվելուց անմիջապես հետո
//    //public event MoveDelegate OnMove;
//    //MoveAffirmation MoveChanger(MoveAffirmation obj)
//    //{
//    //    MoveAffirmation copy = new MoveAffirmation(obj);
//    //    if (copy.T == PieceColor.White)
//    //        copy.T = PieceColor.Black;
//    //    else
//    //        copy.T = PieceColor.White;
//    //    return copy;
//    //}
//    //public MoveAffirmation(PiecePosition start, PiecePosition target)
//    //{
//    //    Start = start;
//    //    Target = target;
//    //}
//    //public MoveAffirmation(MoveAffirmation original)
//    //{
//    //    this.Start = original.Start;        
//    //    this.Target = original.Target; 
//    //    this.Castling = original.Castling;
//    //    this.MovedPiece = (Piece?)original.MovedPiece?.Clone();
//    //    this.CapturedPiece = (Piece?)original.CapturedPiece?.Clone();
//    //    this.T = original.T;
//    //    this.game = original.game;
//    //}
//    //public MoveAffirmation()
//    //{
//    //    OnMove += MoveChanger;
//    //}
//    //public MoveAffirmation(PiecePosition start, PiecePosition target, Piece movedPiece, Piece? capturedFigure, (bool, PieceColor) castling)
//    //{
//    //    Start = start;
//    //    Target = target;
//    //    MovedPiece = movedPiece;
//    //    CapturedPiece = capturedFigure;
//    //    Castling = castling;
//    //}

//    //public bool Move(ref ChessBoard board, PiecePosition end, ref PieceColor T)
//    //{
//    //    ChessBoard newBoard = (ChessBoard)board.Clone();
//    //    PieceColor tempT = T;

//    //    if (ChessRules.MoveValidation(board, Start, end, T))
//    //    {
//    //        CastlingLeft(newBoard, end, ref tempT);
//    //        CastlingRight(newBoard, end, ref tempT);
//    //        RegularMove(newBoard, end, ref T);
//    //        if (OnMove != null)
//    //            OnMove(this);
//    //    }
//    //    else
//    //    {
//    //        Console.WriteLine("Invalid Move:  !!!");
//    //        return false;
//    //    }

//    //    PiecePosition? TKing = ChessBoard.GetKingPosition(newBoard, T);
//    //    if (TKing is not null && newBoard[TKing] is not null)
//    //    {
//    //        if (ChessRules.IsChecked(newBoard, TKing))
//    //        {
//    //            Console.WriteLine(" Invalid Move:  Check way was opened! ");
//    //            return false;
//    //        }

//    //        else
//    //        {
//    //            board = (ChessBoard)newBoard;
//    //            Game.moves?.Add(new MoveAffirmation(Start, Target, board[Start]!, board[end], Castling));
//    //            if (T == PieceColor.White)
//    //                T = PieceColor.Black;
//    //            else
//    //                T = PieceColor.White;

//    //            PiecePosition? lastCheck = ChessBoard.GetKingPosition(board, T);
//    //            if (lastCheck is not null && ChessRules.IsCheckMate(board, lastCheck, Target))
//    //                Console.WriteLine("        CHECKMATE!!!      ");
//    //            else if (lastCheck is not null && ChessRules.IsChecked(board, lastCheck))
//    //                Console.WriteLine("        CHECK!!!      ");
//    //            return true;
//    //        }
//    //    }
//    //    else
//    //    {
//    //        Console.WriteLine(" Invalid Move!!! ");
//    //        return false;
//    //    }

//    //}
//    //public void RegularMove(ChessBoard board, PiecePosition end, ref PieceColor T)
//    //{
//    //    Piece? piece = board[Start];
//    //    board[end] = piece;
//    //    board[Start] = null;
//    //    piece?.Position = end;
//    //    piece?.HasMoved = true;
//    //    PawnPromotionMove(board, end);
//    //}
//    //private (ChessBoard, bool) CastlingLeft(ChessBoard board, PiecePosition end, ref PieceColor T)
//    //{
//    //    Piece? piece = board[Start];
//    //    if (ChessRules.IsChecked(board).Item1 == false &&
//    //    Start.Col - end.Col == 2 && piece?.Type == PieceType.King && board[end.Row, end.Col - 2]?.Type == PieceType.Rook)
//    //    {
//    //        if (board[Start]?.HasMoved == false
//    //            && board[end.Row, end.Col - 2]?.HasMoved == false)
//    //        {
//    //            board[end] = board[Start];
//    //            board[end]?.Position = end;
//    //            board[Start] = null;
//    //            board[end.Row, end.Col + 1] = board[end.Row, end.Col - 2];
//    //            board[end.Row, end.Col + 1]?.Position =
//    //                new PiecePosition { Row = end.Row, Col = end.Col + 1 }; board[end.Row, end.Col - 2] = null;
//    //            Castling = (true, piece.Color);
//    //            return (board, true);
//    //        }
//    //    }
//    //    return (board, false);
//    //}
//    //private (ChessBoard, bool) CastlingRight(ChessBoard board, PiecePosition end, ref PieceColor T)
//    //{
//    //    Piece? piece = board[Start];
//    //    if (ChessRules.IsChecked(board).Item1 == false &&
//    //        end.Col - Start.Col == 2 && piece?.Type == PieceType.King && board[end.Row, end.Col + 1]?.Type == PieceType.Rook)
//    //    {
//    //        if (board[Start]?.HasMoved == false
//    //            && board[end.Row, end.Col + 1]?.HasMoved == false)
//    //        {
//    //            board[end] = board[Start];
//    //            board[end]?.Position = end;
//    //            board[Start] = null;
//    //            board[end.Row, end.Col - 1] = board[end.Row, end.Col + 1];
//    //            board[end.Row, end.Col - 1]?.Position =
//    //                new PiecePosition { Row = end.Row, Col = end.Col - 1 }; board[end.Row, end.Col + 1] = null;
//    //            Castling = (true, piece.Color);
//    //            return (board, true);
//    //        }
//    //    }
//    //    return (board, false);
//    //}
//    //public void PawnPromotionMove(ChessBoard board, PiecePosition end)
//    //{
//    //    if (ChessRules.PawnPromotion(board, end))
//    //    {
//    //        Console.Write("INSERT NEW TYPE FOR PAWN: ");
//    //        string? newFigure = Console.ReadLine();
//    //        if (board[end] is Piece pawn)
//    //        {
//    //            Piece newPiece = newFigure switch
//    //            {
//    //                "T" => new Knight(pawn.Color),
//    //                "B" => new Bishop(pawn.Color),
//    //                "R" => new Rook(pawn.Color),
//    //                "Q" => new Queen(pawn.Color),
//    //                _ => throw new Exception()
//    //            };
//    //            newPiece.Position = end;
//    //            newPiece.Symbol = newPiece.GetSymbol(newPiece.Color);

//    //            board[end] = newPiece;
//    //        }
//    //    }
//    //}
//}
