namespace ChessUniverse.Library;

public class PiecePosition
{
    public int Row { get; set; }
    public int Col { get; set; }

    public PiecePosition()
    {
        
    }
    public PiecePosition(int row, int col)
    {
        Row = row; Col = col;
    }
    public PiecePosition(string s)
    {
        int col = s[0] - 'A';
        int row = 7 - (s[1] - '1');
    }
}
