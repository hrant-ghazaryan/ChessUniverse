namespace ChessUniverse.Library;

public static class Game
{
    /// <summary>
    /// Կատարում է սովորական քայլ (ոչ castling կամ հատուկ քայլեր)։
    /// Տեղափոխում է ֆիգուրը սկզբնական դիրքից նպատակային դիրք,
    /// թարմացնում է ֆիգուրի դիրքը և նշում է այն որպես արդեն շարժված։
    /// </summary>
    /// <param name="board">Շախմատի խաղատախտակի ընթացիկ վիճակը։</param>
    /// <param name="moveInfo">Քայլի մասին տվյալներ՝ սկզբնական և նպատակային դիրքերով։</param>
    public static void RegularMove(ChessBoard board, MoveInfo moveInfo)
    {
        if (moveInfo.Start is not null && moveInfo.Target is not null)
        {
            Piece? piece = board[moveInfo.Start];
            board[moveInfo.Target] = piece;
            piece?.HasMoved = true;
            piece?.Position = moveInfo.Target;
            board[moveInfo.Start] = null;
        }
    }
    /// <summary>
    /// Կատարում է ձախ կողմի castling (թագուհու կողմի castling) քայլը։
    /// Թագավորը տեղափոխվում է 2 դաշտ ձախ, իսկ նավակը՝ թագավորի նոր դիրքից 3 դաշտ աջ։
    /// Թարմացվում են բոլոր ֆիգուրների դիրքերը և քայլը նշվում է որպես castling։
    /// </summary>
    /// <param name="chessBoard">Շախմատի խաղատախտակի ընթացիկ վիճակը։</param>
    /// <param name="moveInfo">Castling քայլի մասին տեղեկություններ՝ սկիզբ և նպատակային դիրքերով։</param>
    /// <returns>Թարմացված խաղատախտակ castling-ից հետո, կամ նույն board-ը եթե տվյալները սխալ են։</returns>
    public static ChessBoard? CastlingLeft(ChessBoard chessBoard, MoveInfo moveInfo)
    {
        if (chessBoard is null) return chessBoard;
        if (moveInfo is null) return chessBoard;
        if (moveInfo.Start is null) return chessBoard;
        if (moveInfo.Target is null) return chessBoard;

        var color = chessBoard[moveInfo.Start]!.Color;
        //Թագավորի դիրքի փոփոխություն – 2 դիրք ձախ.
        chessBoard[moveInfo.Target] = chessBoard[moveInfo.Start];
        chessBoard[moveInfo.Target]?.Position = moveInfo.Target;
        chessBoard[moveInfo.Start] = null;

        //Նավակի դիրքի փոփոփոխություն – 3 դիրք աջ.
        chessBoard[moveInfo.Target.Row, 3] = chessBoard[moveInfo.Target.Row, 0];
        chessBoard[moveInfo.Target.Row, 3]?.Position = new PiecePosition { Row = moveInfo.Target.Row, Col = 3 };
        chessBoard[moveInfo.Target.Row, 0] = null;

        //Castling property ի փոփոխություն․
        moveInfo.Castling = (true, color);
        return chessBoard;
    }
    /// <summary>
    /// Կատարում է աջ կողմի castling (թագավորի կողմի castling) քայլը։
    /// Թագավորը տեղափոխվում է 2 դաշտ աջ, իսկ նավակը տեղափոխվում է թագավորի նոր դիրքից 2 դաշտ ձախ։
    /// Թարմացվում են բոլոր ֆիգուրների դիրքերը և քայլը նշվում է որպես castling։
    /// </summary>
    /// <param name="chessBoard">Շախմատի խաղատախտակի ընթացիկ վիճակը։</param>
    /// <param name="moveInfo">Castling քայլի մասին տեղեկություններ՝ սկիզբ և նպատակային դիրքերով։</param>
    /// <returns>Թարմացված խաղատախտակ castling-ից հետո, կամ նույն board-ը եթե տվյալները սխալ են։</returns>
    public static ChessBoard? CastlingRight(ChessBoard chessBoard, MoveInfo moveInfo)
    {
        if (chessBoard is null) return chessBoard;
        if (moveInfo is null) return chessBoard;
        if (moveInfo.Start is null) return chessBoard;
        if (moveInfo.Target is null) return chessBoard;

        var color = chessBoard[moveInfo.Start]!.Color;

        //Թագավորի դիրքի փոփոխություն – 2 դիրք աջ.
        chessBoard[moveInfo.Target] = chessBoard[moveInfo.Start];
        chessBoard[moveInfo.Target]?.Position = moveInfo.Target;
        chessBoard[moveInfo.Start] = null;

        //Նավակի դիրքի փոփոփոխություն – 2 դիրք ձախ.
        chessBoard[moveInfo.Target.Row, 5] = chessBoard[moveInfo.Target.Row, 7];
        chessBoard[moveInfo.Target.Row, 5]?.Position = new PiecePosition { Row = moveInfo.Target.Row, Col = 5 };
        chessBoard[moveInfo.Target.Row, 7] = null;

        //Castling property ի փոփոխություն․
        moveInfo.Castling = (true, color);
        return chessBoard;
    }
}
