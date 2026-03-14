using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public class Piece(PieceColor color, PieceType type, char symbol, PiecePosition position)
{
    public PieceColor Color { get; } = color;
    public PieceType Type { get; } = type;
    public char Symbol { get; set; } = symbol;
    public PiecePosition Position { get; set; } = position;
    public bool HasMoved { get; set; }

    public  ChessBoard Move(ChessBoard board,  PiecePosition end)
    {
        Piece? piece = board[Position];
        if (Position.Col - end.Col == 2 && piece?.Type == PieceType.King
            && board[end.Row, end.Col - 2]?.Type == PieceType.Rook)
        {
            if (board[Position]?.HasMoved == false
                && board[end.Row, end.Col - 2]?.HasMoved == false)
            {
                board[end] = board[Position];
                board[Position] = null;
                var temp = board[end.Row, end.Col - 2];
                board[end.Row, end.Col + 1] = board[end.Row, end.Col - 2];
                board[end.Row, end.Col - 2] = null;
                return board;
            }
        }
        if (end.Col - Position.Col == 2 && piece?.Type == PieceType.King
            && board[end.Row, end.Col + 1]?.Type == PieceType.Rook)
        {
            if (board[Position]?.HasMoved == false
                && board[end.Row, end.Col + 1]?.HasMoved == false)
            {
                board[end] = board[Position];
                board[Position] = null;
                board[end.Row, end.Col - 1] = board[end.Row, end.Col + 1];
                board[end.Row, end.Col + 1] = null;
                return board;
            }
        }

        board[end] = piece;
        board[Position] = null;
        piece?.Position = end;
        piece?.HasMoved = true;

        return board;
    }
    public ChessBoard? MoveBack(ChessBoard board, PiecePosition start)
    {
        //7
        if (board[Position] != null)
        {
            Piece piece = board[Position]!; 
            if (ChessRules.IsChecked(board).Item1)
            {

                board[start] = piece;
                board[Position] = null;
                return board;
            }
            return board;
        }
        return null;
    }
    public virtual char GetSymbol(PieceColor color)
    {
        if (color == PieceColor.White)
        {
            string s = Symbol.ToString().ToUpper();
            bool b = char.TryParse(s, out char c);
            return c;
        }
        else
            return Symbol;
    }

    public virtual bool IsMovePossible( ChessBoard chessBoard, PiecePosition target)
        => false;

};


