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
    // Ընդունում է.
    //   "<դիրք> <հերթ>"                                   — կարճ ձև (հին puzzle-ների համար)
    //   "<դիրք> <հերթ> <փոխատեղում> <en passant> [halfmove] [fullmove]" — ստանդարտ FEN
    public static FenPosition ParsePosition(string positionText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(positionText);

        string[] fields = positionText.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries);

        if (fields.Length is not (2 or 4 or 5 or 6))
        {
            throw new ArgumentException(
                "Position text must be '<board> <turn>' or a standard FEN " +
                "'<board> <turn> <castling> <en-passant> [halfmove] [fullmove]'.",
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

        // Կարճ ձևում փոխատեղման իրավունքների մասին տեղեկություն չկա.
        // պահպանում ենք հին վարքը (տնային վանդակների արքան ու նավակները համարվում են չշարժված)։
        if (fields.Length == 2)
            return new FenPosition(board, activeTurn);

        ApplyCastlingRights(board, fields[2]);

        MoveInfo? previousMove = ParseEnPassant(board, fields[3], activeTurn);

        int halfmoveClock = fields.Length > 4 ? ParseCounter(fields[4], minimum: 0) : 0;
        int fullmoveNumber = fields.Length > 5 ? ParseCounter(fields[5], minimum: 1) : 1;

        return new FenPosition(board, activeTurn, previousMove, halfmoveClock, fullmoveNumber);
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

    // Ստանդարտ 6 դաշտանոց FEN։
    public static string ToFen(
        ChessBoard board,
        PieceColor activeTurn,
        MoveInfo? previousMove = null,
        int halfmoveClock = 0,
        int fullmoveNumber = 1)
    {
        ArgumentNullException.ThrowIfNull(board);

        return string.Join(' ',
            ToPosition(board, activeTurn),
            GetCastlingRights(board),
            GetEnPassantSquare(board, previousMove),
            halfmoveClock,
            fullmoveNumber);
    }

    // ------------------------------------------------------------------
    // Փոխատեղում
    // ------------------------------------------------------------------

    // FEN-ի "KQkq" դաշտը թարգմանում ենք HasMoved-ի. եթե իրավունքը բացակայում է,
    // համապատասխան նավակը (կամ երկու իրավունքն էլ չկան՝ արքան) նշվում է որպես շարժված։
    private static void ApplyCastlingRights(ChessBoard board, string castlingField)
    {
        if (castlingField != "-" &&
            (castlingField.Length is 0 or > 4 || castlingField.Any(c => "KQkq".IndexOf(c) < 0)))
        {
            throw new ArgumentException($"Invalid FEN castling field: '{castlingField}'.");
        }

        ApplySideCastlingRights(board, PieceColor.White, homeRow: 7,
            kingSide: castlingField.Contains('K'), queenSide: castlingField.Contains('Q'));

        ApplySideCastlingRights(board, PieceColor.Black, homeRow: 0,
            kingSide: castlingField.Contains('k'), queenSide: castlingField.Contains('q'));
    }

    private static void ApplySideCastlingRights(
        ChessBoard board, PieceColor color, int homeRow, bool kingSide, bool queenSide)
    {
        if (IsPiece(board[homeRow, 4], PieceType.King, color))
            board[homeRow, 4]!.HasMoved = !kingSide && !queenSide;

        if (IsPiece(board[homeRow, 7], PieceType.Rook, color))
            board[homeRow, 7]!.HasMoved = !kingSide;

        if (IsPiece(board[homeRow, 0], PieceType.Rook, color))
            board[homeRow, 0]!.HasMoved = !queenSide;
    }

    private static string GetCastlingRights(ChessBoard board)
    {
        var rights = new StringBuilder();

        AppendSideRights(board, PieceColor.White, homeRow: 7, rights, 'K', 'Q');
        AppendSideRights(board, PieceColor.Black, homeRow: 0, rights, 'k', 'q');

        return rights.Length == 0 ? "-" : rights.ToString();
    }

    private static void AppendSideRights(
        ChessBoard board, PieceColor color, int homeRow,
        StringBuilder rights, char kingSideSymbol, char queenSideSymbol)
    {
        Piece? king = board[homeRow, 4];

        if (!IsPiece(king, PieceType.King, color) || king!.HasMoved)
            return;

        if (IsUnmovedRook(board[homeRow, 7], color))
            rights.Append(kingSideSymbol);

        if (IsUnmovedRook(board[homeRow, 0], color))
            rights.Append(queenSideSymbol);
    }

    private static bool IsUnmovedRook(Piece? piece, PieceColor color)
        => IsPiece(piece, PieceType.Rook, color) && !piece!.HasMoved;

    private static bool IsPiece(Piece? piece, PieceType type, PieceColor color)
        => piece is not null && piece.Type == type && piece.Color == color;

    // ------------------------------------------------------------------
    // En passant
    // ------------------------------------------------------------------

    // En passant վանդակը (օր. "e3") այն վանդակն է, որի վրայով զինվորը նոր է անցել։
    // Այստեղից վերականգնում ենք նախորդ քայլը (e2→e4), որ ChessRules.TryEnPassant-ն աշխատի։
    private static MoveInfo? ParseEnPassant(ChessBoard board, string field, PieceColor activeTurn)
    {
        if (field == "-")
            return null;

        if (!PiecePosition.TryParse(field, out PiecePosition? square))
            throw new ArgumentException($"Invalid FEN en passant square: '{field}'.");

        // Եթե սպիտակի հերթն է, սևն է նոր քայլել (7-րդ → 5-րդ շարք, վանդակը 6-րդ շարքում)։
        bool blackJustMoved = activeTurn == PieceColor.White;
        int expectedRow = blackJustMoved ? 2 : 5;

        if (square!.Row != expectedRow)
            throw new ArgumentException($"En passant square '{field}' does not match the side to move.");

        int fromRow = blackJustMoved ? 1 : 6;
        int toRow = blackJustMoved ? 3 : 4;
        PieceColor pawnColor = blackJustMoved ? PieceColor.Black : PieceColor.White;

        if (!IsPiece(board[toRow, square.Col], PieceType.Pawn, pawnColor))
            throw new ArgumentException($"En passant square '{field}' has no pawn that just moved past it.");

        return new MoveInfo(
            new PiecePosition(fromRow, square.Col),
            new PiecePosition(toRow, square.Col));
    }

    private static string GetEnPassantSquare(ChessBoard board, MoveInfo? previousMove)
    {
        if (previousMove?.Start is null || previousMove.Target is null)
            return "-";

        PiecePosition start = previousMove.Start;
        PiecePosition target = previousMove.Target;

        if (start.Col != target.Col ||
            Math.Abs(start.Row - target.Row) != 2 ||
            board[target]?.Type != PieceType.Pawn)
        {
            return "-";
        }

        var passedSquare = new PiecePosition((start.Row + target.Row) / 2, start.Col);
        return passedSquare.ToString().ToLowerInvariant();
    }

    private static int ParseCounter(string field, int minimum)
    {
        if (!int.TryParse(field, out int value) || value < minimum)
            throw new ArgumentException($"Invalid FEN move counter: '{field}'.");

        return value;
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