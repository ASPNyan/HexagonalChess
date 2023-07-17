namespace HexagonalChess;

public class Player
{
    public Guid? Guid { get; }

    public string? Username { get; }

    public Player()
    {
        Guid = null;
        Username = null;
    }
    
    internal Player(Guid guid, string username)
    {
        Guid = guid;
        Username = username;
    }

    public static bool operator ==(Player player1, Player player2) => player1.Guid == player2.Guid;

    public static bool operator !=(Player player1, Player player2) => !(player1 == player2);
}