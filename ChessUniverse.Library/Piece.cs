using ChessUniverse.Library.Enums;

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
    public virtual bool IsMovePossible( ChessBoard chessBoard, PiecePosition target)
        => false;
    public virtual List<PiecePosition> GetPossibleMoves(ChessBoard chessBoard)
        => new List<PiecePosition>();

    //public static ChessBoard SwitchPositions(ChessBoard board, Piece piece1, Piece piece2)
    //{
    //    Piece? temp = null;
    //    if (piece1 is not null && piece2 is not null)
    //    {
    //        temp = piece1;
    //        piece1 = piece2;
    //        piece2 = temp;
    //    }
    //    return board;
    //}
    public static ChessBoard SwitchPositions(ChessBoard board, PiecePosition startPosition, PiecePosition targetPosition)
    {
        if (startPosition is not null && targetPosition is not null 
            &&  board[startPosition] is not null)
        {
            board[targetPosition] = board[startPosition];
            board[targetPosition]?.Position = board[startPosition]!.Position;
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


