using ChessUniverse.Library.Enums;
using System.Reflection.Metadata.Ecma335;

namespace ChessUniverse.Library;

public class Piece(PieceColor color, PieceType type, char symbol, PiecePosition position) : ICloneable
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
    public virtual bool IsMovePossible(ChessBoard chessBoard, PiecePosition target)
        => true;
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
    public object Clone()
    {
        return new Piece(this.Color, this.Type, this.Symbol, new PiecePosition { Row = this.Position.Row, Col = this.Position.Col })
        {
            HasMoved = this.HasMoved
        };
    }
};


