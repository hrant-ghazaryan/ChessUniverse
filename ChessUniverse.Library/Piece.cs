using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public abstract class Piece(PieceColor color, PieceType type, char symbol, PiecePosition position) : ICloneable
{
    public PieceColor Color { get; } = color;
    public PieceType Type { get; set;  } = type;
    public char Symbol { get; set; } = symbol;
    public PiecePosition Position { get; set; } = position;
    public bool HasMoved { get; set; }

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
    public abstract bool IsMovePossible(ChessBoard chessBoard, PiecePosition target);
    public virtual List<PiecePosition> GetPossibleMoves(ChessBoard chessBoard)
        => new List<PiecePosition>();
    public static ChessBoard SwitchPositions(ChessBoard board, PiecePosition startPosition, PiecePosition targetPosition)
    {
        if (startPosition is not null && targetPosition is not null 
            &&  board[startPosition] is not null)
        {
            board[targetPosition] = board[startPosition];
            board[targetPosition]?.Position = targetPosition;
            board[startPosition] = null;
        }
        return board;
    }
    public abstract object Clone();

};


