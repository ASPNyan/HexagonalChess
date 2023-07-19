using Presets = HexagonalChess.Game.MovementDirectionPresets;

namespace HexagonalChess.Game;

public class PieceState
{
    public static PieceState Pawn(int id, bool isWhite, string coord) => new(id, nameof(Pawn), isWhite, coord, Presets.Pawn, Presets.PawnAttack);
    public static PieceState Bishop(int id, bool isWhite, string coord) => new(id, nameof(Bishop), isWhite, coord, Presets.Bishop);
    public static PieceState Knight(int id, bool isWhite, string coord) => new(id, nameof(Knight), isWhite, coord, Presets.Knight);
    public static PieceState Rook(int id, bool isWhite, string coord) => new(id, nameof(Rook), isWhite, coord, Presets.Rook);
    public static PieceState Queen(int id, bool isWhite, string coord) => new(id, nameof(Queen), isWhite, coord, Presets.Queen);
    public static PieceState King(int id, bool isWhite, string coord) => new(id, nameof(King), isWhite, coord, Presets.King);

    public int Id { get; }
    public bool IsWhite { get; }
    public string Name { get; private set; }
    public MovementDirection Direction { get; private set; }
    public MovementDirection AttackDir { get; private set; }
    public string Coord { get; set; }

    public PieceState(int id, string name, bool isWhite, string coord, MovementDirection direction, MovementDirection? attackDir = null)
    {
        Id = id;
        Name = name;
        Direction = direction;
        Coord = coord;
        IsWhite = isWhite;
        AttackDir = attackDir ?? direction;
    }

    public void Update(string name, MovementDirection direction, MovementDirection? attackDir = null)
    {
        Name = name;
        Direction = direction;
        AttackDir = attackDir ?? direction;
    }
}