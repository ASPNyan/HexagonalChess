namespace HexagonalChess.Game;

public class CellState
{
    protected bool Equals(CellState other)
    {
        return Equals(ActivePiece, other.ActivePiece);
    }

    internal readonly string Coord;
    internal readonly int Id;

    public char File => Coord[0];
    public int Rank => int.Parse(Coord[1..]);
    public CellColor Color { get; set; }

    public PieceState? ActivePiece;
    
    internal CellState(string coord, int id, PieceState? startingPiece = null)
    {
        Coord = coord;
        Id = id;
        ActivePiece = startingPiece;
    }

    public MovementDirection? DirectionToCell(CellState cellState, bool isWhite)
    {
        int currentFile = Coord[0];
        int currentRank = int.Parse(Coord[1..]);

        int newFile = cellState.Coord[0];
        int newRank = int.Parse(cellState.Coord[1..]);

        if (currentFile > 'J')
            currentFile -= 1;

        if (newFile > 'J')
            newFile -= 1;

        int diffFile = newFile - currentFile;
        int diffRank = newRank - currentRank;
        diffRank = isWhite ? -diffRank : diffRank;
        
        if (ActivePiece?.Direction.HasFlag(MovementDirection.Knight) is true)
        {
            // Validate the knight's movement (L shape: 2 steps in one direction and 1 step in perpendicular direction)
            if ((Math.Abs(diffFile) == 2 && Math.Abs(diffRank) == 1) || (Math.Abs(diffFile) == 1 && Math.Abs(diffRank) == 2))
                return MovementDirection.Knight;

            return null; // Invalid move for Knight
        }
        
        if (diffFile == diffRank)
        {
            return diffFile > 0
                ? MovementDirection.DiagonalForwardRight
                : MovementDirection.DiagonalBackwardLeft;
        }

        if (diffFile == -diffRank)
        {
            return diffFile > 0
                ? MovementDirection.DiagonalForwardLeft
                : MovementDirection.DiagonalBackwardRight;
        }

        if (diffFile == 0) return diffRank > 0 ? MovementDirection.Forward : MovementDirection.Backward;
        if (diffRank == 0) return diffFile > 0 ? MovementDirection.ForwardRight : MovementDirection.BackwardLeft;

        // Not a valid move: return null
        return null;
    }
}