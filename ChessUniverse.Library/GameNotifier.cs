namespace ChessUniverse.Library;

public class GameNotifier
{
    public MoveInfo MoveMassage(MoveInfo obj)
    {
        Console.WriteLine("Good Move:");
        return obj;
    }
    public MoveInfo InvalidMoveMassage(MoveInfo obj)
    {
        Console.WriteLine("Invalid Move!");
        return obj;
    }
    public MoveInfo CheckMassage(MoveInfo obj)
    {
        Console.WriteLine("CHECK!");
        return obj;
    }
    public MoveInfo CheckMateMassage(MoveInfo    obj)
    {
        Console.WriteLine("CHECKMATE!");
        return obj;
    }
}
