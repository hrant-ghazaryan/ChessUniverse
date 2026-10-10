using ChessUniverse.Library.Enums;
using ChessUniverse.Library.Pieces;
using System.Text;

namespace ChessUniverse.Library;

public static class FenNotation
{
    public static ChessBoard ParseBoardPlacement(string boardPlacement)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(boardPlacement);

        string[] ranks = boardPlacement.Split('/');

        if (ranks.Length != 8)
            throw new ArgumentException(
                "FEN board placement must contain 8 ranks.",
                nameof(boardPlacement));

        Piece?[,] pieces = new Piece?[8, 8];

        for (int row = 0; row < 8; row++)
        {
            int col = 0;

            foreach (char symbol in ranks[row])
            {
                if (char.IsDigit(symbol))
                {
                    int emptySquares = symbol - '0';

                    if (emptySquares is < 1 or > 8)
                    {
                        throw new ArgumentException(
                            "FEN contains an invalid empty-square count.",
                            nameof(boardPlacement));
                    }

                    col += emptySquares;
                }
                else
                {
                    if (col >= 8)
                    {
                        throw new ArgumentException(
                            "FEN rank contains too many squares.",
                            nameof(boardPlacement));
                    }

                    pieces[row, col] = CreatePiece(
                        symbol,
                        new PiecePosition(row, col));

                    col++;
                }
            }

            if (col != 8)
            {
                throw new ArgumentException(
                    "Every FEN rank must contain exactly 8 squares.",
                    nameof(boardPlacement));
            }
        }

        return new ChessBoard(pieces);
    }

    public static string ToBoardPlacement(ChessBoard board)
    {
        ArgumentNullException.ThrowIfNull(board);

        var ranks = new List<string>();

        for (int row = 0; row < 8; row++)
        {
            var rank = new StringBuilder();
            int emptySquares = 0;

            for (int col = 0; col < 8; col++)
            {
                Piece? piece = board[row, col];

                if (piece is null)
                {
                    emptySquares++;
                    continue;
                }

                if (emptySquares > 0)
                {
                    rank.Append(emptySquares);
                    emptySquares = 0;
                }

                rank.Append(GetFenSymbol(piece));
            }

            if (emptySquares > 0)
                rank.Append(emptySquares);

            ranks.Add(rank.ToString());
        }

        return string.Join("/", ranks);
    }
    public static FenPosition ParsePosition(string positionText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(positionText);

        string[] fields = positionText.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries);

        if (fields.Length != 2)
        {
            throw new ArgumentException(
                "Position text must contain board placement and active turn.",
                nameof(positionText));
        }

        PieceColor activeTurn = fields[1] switch
        {
            "w" => PieceColor.White,
            "b" => PieceColor.Black,
            _ => throw new ArgumentException(
                "Active turn must be 'w' or 'b'.",
                nameof(positionText))
        };

        ChessBoard board = ParseBoardPlacement(fields[0]);

        return new FenPosition(board, activeTurn);
    }

    public static string ToPosition(
        ChessBoard board,
        PieceColor activeTurn)
    {
        string activeTurnSymbol = activeTurn == PieceColor.White
            ? "w"
            : "b";

        return $"{ToBoardPlacement(board)} {activeTurnSymbol}";
    }

    private static Piece CreatePiece(char symbol, PiecePosition position)
    {
        PieceColor color = char.IsUpper(symbol)
            ? PieceColor.White
            : PieceColor.Black;

        Piece piece = char.ToLowerInvariant(symbol) switch
        {
            'p' => new Pawn(color),
            'r' => new Rook(color),
            'n' => new Knight(color),
            'b' => new Bishop(color),
            'q' => new Queen(color),
            'k' => new King(color),
            _ => throw new ArgumentException(
                $"Invalid FEN piece symbol: {symbol}")
        };

        piece.Position = position;

        return piece;
    }

    private static char GetFenSymbol(Piece piece)
    {
        char symbol = piece.Type switch
        {
            PieceType.Pawn => 'p',
            PieceType.Rook => 'r',
            PieceType.Knight => 'n',
            PieceType.Bishop => 'b',
            PieceType.Queen => 'q',
            PieceType.King => 'k',
            _ => throw new InvalidOperationException(
                $"Unknown piece type: {piece.Type}")
        };

        return piece.Color == PieceColor.White
            ? char.ToUpperInvariant(symbol)
            : symbol;
    }
}