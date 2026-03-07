using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public class Piece(PieceColor color, PieceType type , char symbol)
{
    public PieceColor Color { get; } = color;
    public PieceType Type { get; } = type;
    public char Symbol { get; } = symbol;
    
    public virtual char GetSymbol(PieceColor color)
    {
        if (color == PieceColor.White)
            return Symbol;
        else
        {
            Symbol.ToString().ToLower();
            return Symbol;
        }
    }
};


