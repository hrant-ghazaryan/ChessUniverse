namespace ChessUniverse.Library;

public class PiecePosition : IEquatable<PiecePosition>
{
    public int Row { get; set; }
    public int Col { get; set; }

    // Ճիշտ է միայն այն դեպքում, երբ դիրքը 8x8 տախտակի ներսում է։
    public bool IsOnBoard => Row is >= 0 and < 8 && Col is >= 0 and < 8;

    public PiecePosition() { }
    public PiecePosition(int row, int col)
    {
        Row = row; Col = col;
    }

    // Ընդունում է "e4" կամ "E4". սխալ տեքստի դեպքում նետում է exception,
    // որպեսզի լուռ (0,0) դիրք չստեղծվի։
    public PiecePosition(string coordinate)
    {
        if (!TryParseCoordinates(coordinate, out int row, out int col))
            throw new ArgumentException(
                $"Invalid square coordinate: '{coordinate}'.", nameof(coordinate));

        Row = row;
        Col = col;
    }

    public static PiecePosition Parse(string coordinate)
        => new PiecePosition(coordinate);

    public static bool TryParse(string? coordinate, out PiecePosition? position)
    {
        if (TryParseCoordinates(coordinate, out int row, out int col))
        {
            position = new PiecePosition(row, col);
            return true;
        }

        position = null;
        return false;
    }

    private static bool TryParseCoordinates(string? coordinate, out int row, out int col)
    {
        row = col = -1;

        if (coordinate is null || coordinate.Length != 2)
            return false;

        int file = char.ToLowerInvariant(coordinate[0]) - 'a'; // a - h → 0 - 7
        int rank = coordinate[1] - '1';                        // 1 - 8 → 0 - 7

        if (file is < 0 or > 7 || rank is < 0 or > 7)
            return false;

        col = file;
        row = 7 - rank;
        return true;
    }

    public override string ToString()
    {
        char file = (char)('A' + Col);
        int rank = 8 - Row;

        return $"{file}{rank}";
    }

    public bool Equals(PiecePosition? other)
        => other is not null &&
            this.Row == other.Row && this.Col == other.Col;

    public override bool Equals(object? obj)
        => Equals(obj as PiecePosition);

    public override int GetHashCode()
        => HashCode.Combine(Row, Col);
}
