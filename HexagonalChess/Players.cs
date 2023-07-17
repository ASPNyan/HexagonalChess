using System.Collections.Concurrent;

namespace HexagonalChess;

public class Players : ConcurrentDictionary<Guid, string>
{
    public Guid CreateNew(string username)
    {
        var id = Guid.NewGuid();
        TryAdd(id, username);
        return id;
    }

    public void Update(Guid guid, string username)
    {
        
    }
}