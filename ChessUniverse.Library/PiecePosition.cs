namespace ChessUniverse.Library;

public struct PiecePosition(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
}
