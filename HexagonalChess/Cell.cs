namespace HexagonalChess;

public class Cell
{
    protected bool Equals(Cell other)
    {
        return Equals(ActivePiece, other.ActivePiece);
    }

    internal readonly string Coord;
    internal readonly int Id;

    public Piece? ActivePiece;
    
    internal Cell(string coord, int id, Piece? startingPiece = null)
    {
        Coord = coord;
        Id = id;
        ActivePiece = startingPiece;
    }

    public bool ContainsPiece() => ActivePiece is not null;
}