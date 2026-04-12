namespace ChessUniverse.Library;

public class PiecePosition
{
    public int Row { get; set; }
    public int Col { get; set; }
    public PiecePosition() { }
    public PiecePosition(int row, int col)
    {
        Row = row; Col = col;
    }
    public PiecePosition(string s)
    {
        if (s?.Length == 2 && s[0] - 'A' > 0 && s[0] - 'A' < 7
         && 7 - (s[1] - '1') > 0 && 7 - (s[1] - '1') < 7)
        {
            Col = s[0] - 'A';
            Row = 7 - (s[1] - '1');
        }
    }
    public override string ToString()
    {
        char file = (char)('A' + Col);
        int rank = 7 - (Row - 1);

        return $"{file}{rank}";
    }
}
