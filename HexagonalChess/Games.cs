using System.Collections.Concurrent;
using HexagonalChess.Game;

namespace HexagonalChess;

public class Games : ConcurrentDictionary<Guid, GameState>
{
    public Guid CreateNew()
    {
        var id = Guid.NewGuid();
        var newGame = new GameState { OnFinish = () => TryRemove(id, out _) };
        TryAdd(id, newGame);
        return id;
    }

    private new GameState this[Guid guid] => base[guid];

    public (GameState Game, bool? PlayerIsWhite) GetGameState(Player player, Guid gameId)
    {
        var gameState = this[gameId];
        var users = gameState.Players;

        bool? playerIsWhite;

        if (player == users.White)
            playerIsWhite = true;
        else if (player == users.Black)
            playerIsWhite = false;
        else
            playerIsWhite = null;

        return (gameState, playerIsWhite);
    }
}