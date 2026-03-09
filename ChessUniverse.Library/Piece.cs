using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public class Piece(PieceColor color, PieceType type, char symbol)
{
    public PieceColor Color { get; } = color;
    public PieceType Type { get; } = type;
    public char Symbol { get; set; } = symbol;
    public PiecePosition Position { get; set; }
    
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

    public virtual bool IsMovePossible(ref ChessBoard chessBoard, 
        ref PiecePosition start, ref PiecePosition target)
        => false;
};


