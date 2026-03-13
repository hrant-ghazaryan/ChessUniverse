using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public class Piece(PieceColor color, PieceType type, char symbol, PiecePosition position)
{
    public PieceColor Color { get; } = color;
    public PieceType Type { get; } = type;
    public char Symbol { get; set; } = symbol;
    public PiecePosition Position { get; set; } = position;
    public readonly bool HasMoved;

    public virtual ChessBoard Move(ChessBoard board,  PiecePosition end)
    {
        //PiecePosition temp = end;
        //board[end.Row, end.Col] = board[Position.Row, Position.Col];
        //board[end.Row, end.Col].Position = end;
        //board[this.Position.Row, this.Position.Col] = null;
        //return board;

        //temp = end;
        //board[end.Row, end.Col] = board[start.Row, start.Col];
        //board[end.Row, end.Col].Position = end;
        //board[start.Row, start.Col] = null;

        Piece? piece = board[Position.Row, Position.Col];
        if (Position.Col - end.Col == 2 && piece?.Type == PieceType.King
            && board[end.Row, end.Col - 2]?.Type == PieceType.Rook)
        {
            if (board[Position.Row, Position.Col]?.HasMoved == false
                && board[end.Row, end.Col - 2]?.HasMoved == false)
            {
                board[end.Row,end.Col] = board[Position.Row,Position.Col];
                board[Position.Row, Position.Col] = null;
                var temp = board[end.Row, end.Col - 2];
                board[end.Row, end.Col + 1] = board[end.Row, end.Col - 2];
                board[end.Row, end.Col - 2] = null;
                return board;
            }
        }
        if (end.Col - Position.Col == 2 && piece?.Type == PieceType.King
            && board[end.Row, end.Col + 1]?.Type == PieceType.Rook)
        {
            if (board[Position.Row, Position.Col]?.HasMoved == false
                && board[end.Row, end.Col + 1]?.HasMoved == false)
            {
                board[end.Row, end.Col] = board[Position.Row, Position.Col];
                board[Position.Row, Position.Col] = null;
                board[end.Row, end.Col - 1] = board[end.Row, end.Col + 1];
                board[end.Row, end.Col + 1] = null;
                return board;
            }
        }
        board[end.Row, end.Col] = piece;
        board[Position.Row, Position.Col] = null;
        piece.Position = end;

        return board;
    }
    public ChessBoard? MoveBack(ChessBoard board, PiecePosition start)
    {
        //7
        if (board[Position.Row, Position.Col] != null)
        {
            Piece piece = board[Position.Row, Position.Col]!; 
            if (ChessBoard.IsChecked(board).Item1)
            {

                board[start.Row, start.Col] = piece;
                board[Position.Row, Position.Col] = null;
                return board;
            }
            return board;
        }
        return null;

        //board[end.Row, end.Col] = piece;
        //board[Position.Row, Position.Col] = null;
        //piece.Position = end;
        //PiecePosition temp = end;
        
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
    public virtual bool IsMovePossible( ChessBoard chessBoard, 
         PiecePosition start,  PiecePosition? target)
        => false;

};


