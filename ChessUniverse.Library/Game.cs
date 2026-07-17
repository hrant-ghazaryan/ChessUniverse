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
    /*public static ChessBoard? CastlingLeft(ChessBoard chessBoard, MoveInfo moveInfo)
    {
        if (!CastlingRules.ValidateCastlingParameters(chessBoard, moveInfo))
            return chessBoard;

        var startPosition = moveInfo.Start!;
        var targetPosition = moveInfo.Target!;
        var color = chessBoard[startPosition]!.Color;

        CastlingRookMove rookPositions = CastlingRules.GetCastlingRookMove(moveInfo);
        var rookStartPosition = rookPositions.StartPosition!;
        var rookTargetPosition = rookPositions.TargetPosition!;

        //Թագավորի դիրքի փոփոխություն – 2 դիրք ձախ.
        chessBoard[targetPosition] = chessBoard[startPosition];
        chessBoard[targetPosition]?.Position = targetPosition;
        chessBoard[startPosition] = null;

        //Նավակի դիրքի փոփոփոխություն – 3 դիրք աջ.
        chessBoard[rookTargetPosition] = chessBoard[rookStartPosition];
        chessBoard[rookTargetPosition]?.Position = rookTargetPosition;
        chessBoard[rookStartPosition] = null;

        chessBoard[targetPosition.Row, 3] = chessBoard[targetPosition.Row, 0];
        chessBoard[targetPosition.Row, 3]?.Position = new PiecePosition { Row = targetPosition.Row, Col = 3 };
        chessBoard[targetPosition.Row, 0] = null;

        //Castling property ի փոփոխություն․
        moveInfo.Castling = (true, color);
        return chessBoard;
    }*/
    public static ChessBoard? Castling(ChessBoard chessBoard, MoveInfo moveInfo)
    {
        if (!CastlingRules.ValidateCastlingParameters(chessBoard, moveInfo))
            return chessBoard;

        var startPosition = moveInfo.Start!;
        var targetPosition = moveInfo.Target!;

        var color = chessBoard[startPosition]!.Color;

        var rookMove = CastlingRules.GetCastlingRookMove(moveInfo);

        MoveKingForCastling(chessBoard, startPosition, targetPosition);
        MoveRookForCastling(chessBoard, rookMove);

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
    /*public static ChessBoard? CastlingRight(ChessBoard chessBoard, MoveInfo moveInfo)
    {
        if (!CastlingRules.ValidateCastlingParameters(chessBoard, moveInfo))
            return chessBoard;

        var startPosition = moveInfo.Start!;
        var targetPosition = moveInfo.Target!;
        var color = chessBoard[startPosition]!.Color;


        //Թագավորի դիրքի փոփոխություն – 2 դիրք աջ.
        chessBoard[targetPosition] = chessBoard[startPosition];
        chessBoard[targetPosition]?.Position = targetPosition;
        chessBoard[startPosition] = null;

        //Նավակի դիրքի փոփոփոխություն – 2 դիրք ձախ.
        chessBoard[targetPosition.Row, 5] = chessBoard[targetPosition.Row, 7];
        chessBoard[targetPosition.Row, 5]?.Position = new PiecePosition { Row = targetPosition.Row, Col = 5 };
        chessBoard[targetPosition.Row, 7] = null;

        //Castling property ի փոփոխություն․
        moveInfo.Castling = (true, color);
        return chessBoard;
    }*/
    public static void MoveKingForCastling(ChessBoard chessBoard, PiecePosition startPosition, PiecePosition targetPosition)
    {
        chessBoard[targetPosition] = chessBoard[startPosition];
        chessBoard[targetPosition]?.Position = targetPosition;
        chessBoard[targetPosition]?.HasMoved = true;
        chessBoard[startPosition] = null;
    }
    public static void MoveRookForCastling(ChessBoard chessBoard, CastlingRookMove rookPositions)
    {
        var rookStartPosition = rookPositions.StartPosition!;
        var rookTargetPosition = rookPositions.TargetPosition!;

        chessBoard[rookTargetPosition] = chessBoard[rookStartPosition];
        chessBoard[rookTargetPosition]?.Position = rookTargetPosition;
        chessBoard[rookTargetPosition]?.HasMoved = true;
        chessBoard[rookStartPosition] = null;
    }
}