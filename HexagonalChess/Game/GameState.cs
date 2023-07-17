namespace HexagonalChess.Game;

public class GameState
{
    public (Player White, Player Black) Players { get; private set; }
    
    public BoardState BoardState { get; } = new();
    
    public bool IsWhite { get; private set; }

    protected internal void PlayTurn(CellState init, CellState end)
    {
        if (init.ActivePiece is null) 
            throw new ArgumentException("Initial cell does not contain a piece.", nameof(init));
        
        if (end.ActivePiece?.IsWhite == init.ActivePiece.IsWhite)
            throw new ArgumentException("Pieces on the initial and end cell are of the same side");

        MovementDirection? movedDirection = init.DirectionToCell(end, IsWhite);

        if (movedDirection is null || !init.ActivePiece.Direction.HasFlag(movedDirection.Value))
        {
            throw new ArgumentException(
                "Illegal move detected: " +
                "The attempted move does not match the movement rules of the active piece. " +
                $"The attempted move was {GenerateAlgebraicMoveNotationFromCells(init, end)}. " +
                $"If you cannot read chess algebraic notation, read more at: " +
                $"https://en.wikipedia.org/wiki/Algebraic_notation_(chess)");
        }

        IsWhite = !IsWhite;
    }

    /// <summary>
    /// Generates the algebraic move notation for a particular move.
    /// </summary>
    /// <param name="init">The initial <see cref="CellState"/> where the move originates.</param>
    /// <param name="end">The ending <see cref="CellState"/> where the move terminates.</param>
    /// <param name="promotedPiece">Optional parameter for the <see cref="PieceState"/> to which a pawn has promoted. The default value is null.</param>
    /// <returns>Returns a string representing the algebraic move notation of the given move.</returns>
    /// <remarks>
    /// This method generates the algebraic move notation for hexagonal chess, specifying the piece that moved,
    /// if a capture was made, the destination square, and any special conditions such as pawn promotion.
    /// The method handles disambiguation when two pieces of the same type can move to the same square. In this case,
    /// additional notation specifying the file (column) or rank (row) of the moving piece is added to the notation.
    /// The disambiguation check is performed by the <see cref="CheckMoveDisambiguation"/> method,
    /// which is called twice: once for files and once for ranks.
    /// </remarks>
    public string GenerateAlgebraicMoveNotationFromCells(CellState init, CellState end, PieceState? promotedPiece = null)
    {
        var initPiece = init.ActivePiece!;
        var endPiece = end.ActivePiece;

        string notation = initPiece.Name switch
        {
            "King" => "K",
            "Queen" => "Q",
            "Rook" => "R",
            "Bishop" => "B",
            "Knight" => "N",
            _ => string.Empty
        };
        

        CheckMoveDisambiguation(ref notation, init, end, BoardState.File(init.Coord[0]));
        
        CheckMoveDisambiguation(ref notation, init, end, BoardState.Rank(int.Parse(init.Coord[1..])));

        if (endPiece is not null)
            notation += 'x';

        notation += end.Coord;

        if (initPiece.Name is "Pawn" && promotedPiece is not null)
        {
            notation += promotedPiece.Name switch
            {
                "Queen" => "Q",
                "Rook" => "R",
                "Bishop" => "B",
                "Knight" => "N",
                _ => string.Empty
            };
        }

        return notation;
    }

    private static void CheckMoveDisambiguation(ref string notation,
        CellState init,
        CellState end,
        IEnumerable<CellState> cells) =>
        notation = (from cell in cells
            where cell.Coord != init.Coord && cell.ActivePiece is not null
            where cell.ActivePiece.Name == init.ActivePiece!.Name
            let moveDir = cell.DirectionToCell(end,
                init.ActivePiece!.IsWhite)
            where moveDir is not null && cell.ActivePiece.Direction.HasFlag(moveDir.Value)
            select cell).Aggregate(notation,
            (current,
                _) => current + init.Coord.ToLower()[0]);

    public void AddPlayers(Player white, Player black)
    {
        Players = (white, black);
    }

    public Action? OnFinish { get; init; }
}