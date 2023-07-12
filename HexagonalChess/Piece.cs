﻿using Presets = HexagonalChess.MovementDirectionPresets;

namespace HexagonalChess;

public class Piece
{
    public static Piece Pawn(int id, bool isWhite) => new(id, nameof(Pawn), isWhite, Presets.Pawn, Presets.PawnAttack);
    public static Piece Bishop(int id, bool isWhite) => new(id, nameof(Bishop), isWhite, Presets.Bishop);
    public static Piece Knight(int id, bool isWhite) => new(id, nameof(Knight), isWhite, Presets.Knight);
    public static Piece Rook(int id, bool isWhite) => new(id, nameof(Rook), isWhite, Presets.Rook);
    public static Piece Queen(int id, bool isWhite) => new(id, nameof(Queen), isWhite, Presets.Queen);
    public static Piece King(int id, bool isWhite) => new(id, nameof(King), isWhite, Presets.King, null);

    public int Id { get; }
    public bool IsWhite { get; }
    public string Name { get; private set; }
    public MovementDirection Direction { get; private set; }
    public MovementDirection AttackDir { get; private set; }

    public Piece(int id, string name, bool isWhite, MovementDirection direction, MovementDirection? attackDir = null)
    {
        Id = id;
        Name = name;
        Direction = direction;
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