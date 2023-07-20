namespace HexagonalChess.Game;

public class GameState
{
    public (Player White, Player Black) Players { get; private set; }
    
    public BoardState BoardState { get; } = new();

    internal List<Move> MoveHistory = new();
    
    public bool IsWhite { get; private set; }

    protected internal void PlayTurn(CellState init, CellState end)
    {
        if (init.ActivePiece is null) 
            throw new ArgumentException("Initial cell does not contain a piece.", nameof(init));
        
        if (end.ActivePiece?.IsWhite == init.ActivePiece.IsWhite)
            throw new ArgumentException("Pieces on the initial and end cell are of the same side");

        
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
    
    public List<CellState> CalculatePawnMoves(PieceState piece)
    {
        var coord = piece.Coord;
        var moveList = new List<CellState>();

        /*if (piece.IsWhite)
        {

            (string left, string right) attackingCells 
                = (MoveCoord(coord, -1, 2), MoveCoord(coord, 1, 2));

            moveList.AddRange(CheckAttackingPawnMoves(coord, attackingCells, true, lastMove));
            
            
        }*/

        bool startingSquare;
        
        if (piece.IsWhite)
            startingSquare = coord is "B1" or "C2" or "D3" or "E4" or "F5" or "G4" or "H3" or "I2" or "K1";
        else
            startingSquare = coord[1..] == "7";

        var getAttackingCells = FindMovesAlongVertex(coord, MovementDirection.PawnAttack, true, piece.IsWhite, 1).ToList();

        Move? lastMove;
        try
        {
            lastMove = MoveHistory.Last();
        }
        catch (Exception)
        {
            lastMove = null;
        }
        
        moveList.AddRange(CheckAttackingPawnMoves(coord, (getAttackingCells[0].Coord, getAttackingCells[1].Coord), piece.IsWhite, lastMove));

        int moveDistance = startingSquare ? 2 : 1;
        
        moveList.AddRange(FindMovesAlongSide(coord, MovementDirection.Forward, false, piece.IsWhite, moveDistance));
        
        return moveList;
    }

    private IEnumerable<CellState> CheckAttackingPawnMoves(string coord, (string left, string right) attackingCells, bool isWhite, Move? lastMove = null)
    {
        var moveList = new List<CellState>();

        // en passant checking for white
        bool leftEnPassant = lastMove?.PiecePostMove.Name is "Pawn" &&
                             lastMove.PiecePostMove.Coord == MoveCoord(coord, -1, 0) && 
                             lastMove.PiecePreMove.Coord == MoveCoord(coord, -1, 2);

        bool rightEnPassant = lastMove?.PiecePostMove.Name is "Pawn" &&
                              lastMove.PiecePostMove.Coord == MoveCoord(coord, 1, 0) &&
                              lastMove.PiecePreMove.Coord == MoveCoord(coord, 1, 2);

        if (!isWhite)
        {
            // en passant checking for black
            leftEnPassant = lastMove?.PiecePostMove.Name is "Pawn" &&
                                 lastMove.PiecePostMove.Coord == MoveCoord(coord, 1, -1) && 
                                 lastMove.PiecePreMove.Coord == MoveCoord(coord, 1, -3);

            rightEnPassant = lastMove?.PiecePostMove.Name is "Pawn" &&
                                  lastMove.PiecePostMove.Coord == MoveCoord(coord, -1, -1) &&
                                  lastMove.PiecePreMove.Coord == MoveCoord(coord, -1, -3);
        }
        
        if (BoardState[attackingCells.left].ActivePiece is not null || leftEnPassant)
            moveList.Add(BoardState[attackingCells.left]);
            
        if (BoardState[attackingCells.right].ActivePiece is not null || rightEnPassant)
            moveList.Add(BoardState[attackingCells.right]);
        
        return moveList;
    }

    
    private IEnumerable<CellState> FindMovesInPieceDirection(PieceState piece, bool takes, int? moveDistance = null)
    {
        string coord = piece.Coord;

        var moveList = new List<CellState>();
        var movementDirectionsSide = new[] { 
            MovementDirection.Forward,
            MovementDirection.ForwardRight, 
            MovementDirection.ForwardLeft,
            MovementDirection.Backward,
            MovementDirection.BackwardRight,
            MovementDirection.BackwardLeft 
        };
                        
        var movementDirectionsVertex = new[] { 
            MovementDirection.DiagonalForwardRight,
            MovementDirection.DiagonalForwardLeft, 
            MovementDirection.DiagonalRight,
            MovementDirection.DiagonalLeft,
            MovementDirection.DiagonalBackwardRight,
            MovementDirection.DiagonalBackwardLeft 
        };
        foreach (var direction in movementDirectionsSide)
        {
            if (piece.Direction.HasFlag(direction))
            {
                moveList.AddRange(FindMovesAlongSide(coord, direction, takes, piece.IsWhite, moveDistance));
            }
        }

        foreach (var direction in movementDirectionsVertex)
        {
            if (piece.Direction.HasFlag(direction))
            {
                moveList.AddRange(FindMovesAlongVertex(coord, direction, takes, piece.IsWhite, moveDistance));
            }
        }
    
        return moveList;
    }
    

    private IEnumerable<CellState> FindMovesAlongSide(string coord, MovementDirection movementDirection, bool takes,
        bool isWhite, int? moveDistance = null)
    {
        (int fileDiff, int rankDiff) moveDiffPattern = movementDirection switch
        {
            MovementDirection.Forward => isWhite ? (0, 1) : (0, -1),
            MovementDirection.ForwardRight => isWhite ? (1, 0) : (-1, -1),
            MovementDirection.ForwardLeft => isWhite ? (-1, 0) : (1, -1),
            MovementDirection.Backward => isWhite ? (0, -1) : (0, 1),
            MovementDirection.BackwardRight => isWhite ? (1, -1) : (-1, 0),
            MovementDirection.BackwardLeft => isWhite ? (-1, -1) : (1, 0),
            _ => (0, 0)
        };

        var moveList = moveDiffPattern is { fileDiff: 0, rankDiff: 0 }
            ? new List<CellState>()
            : FindMovesInFileRankPattern(coord, moveDiffPattern.fileDiff, moveDiffPattern.rankDiff, takes,
                moveDistance);

        return moveList;
    }

    private IEnumerable<CellState> FindMovesAlongVertex(string coord, MovementDirection movementDirection, bool takes,
        bool isWhite, int? moveDistance = null)
    {
        (int fileDiff, int rankDiff) moveDiffPattern = movementDirection switch
        {
            MovementDirection.DiagonalForwardRight => isWhite ? (1, 1) : (-1, -2),
            MovementDirection.DiagonalForwardLeft => isWhite ? (-1, 1) : (1, -2),
            MovementDirection.DiagonalRight => isWhite ? (2, -1) : (-2, -1),
            MovementDirection.DiagonalLeft => isWhite ? (-2, -1) : (2, -1),
            MovementDirection.DiagonalBackwardRight => isWhite ? (1, -2) : (-1, 1),
            MovementDirection.DiagonalBackwardLeft => isWhite ? (-1, -2) : (1, 1),
            _ => (0, 0)
        };

        var moveList = moveDiffPattern is { fileDiff: 0, rankDiff: 0 }
            ? new List<CellState>()
            : FindMovesInFileRankPattern(coord, moveDiffPattern.fileDiff, moveDiffPattern.rankDiff, takes,
                moveDistance);

        return moveList;
    }

    private IEnumerable<CellState> FindMovesInFileRankPattern(string coord, int fileDiff, int rankDiff, bool takes, 
        int? moveDistance = null)
    {
        var moveList = new List<CellState>();

        int iterations = 1;
        while (moveDistance is null || iterations <= moveDistance)
        {
            string newCoord = MoveCoord(coord, fileDiff * iterations, rankDiff * iterations);
            
            if (!BoardState.BoardLayout.Contains(newCoord))
                return moveList;

            var cell = BoardState[newCoord];

            if (cell.ActivePiece is not null)
            {
                if (takes)
                    moveList.Add(cell);
                return moveList;
            }
            
            moveList.Add(cell);
            
            iterations++;
        }

        return moveList;
    }

    public static string MoveCoord(string coord, int fileDiff, int rankDiff)
        => $"{coord[0]+fileDiff}{int.Parse(coord[1..])+rankDiff}";
}