using ChessUniverse.Library.Enums;

namespace ChessUniverse.Library;

public abstract class Piece(PieceColor color, PieceType type, char symbol, PiecePosition position) : ICloneable
{
    public PieceColor Color { get; } = color;
    public PieceType Type { get; } = type;
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
    // Pseudo-legal. խաղաքարի շարժման ձևը (առանց սեփական թագավորի անվտանգության)։
    // Օրինական քայլերի համար օգտագործիր ChessRules.GetLegalMoves / ChessGame.GetLegalMoves։
    public abstract bool CanMove(ChessBoard chessBoard, PiecePosition target);
    public abstract object Clone();
};


