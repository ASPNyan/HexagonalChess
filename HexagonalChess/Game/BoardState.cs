namespace HexagonalChess.Game;

public class BoardState
{
    // ew, this looks horrible like this.
    private static readonly string[] BoardLayout =
    {
                      "F11", // 1
                   "E10", "G10", // 2
                 "D9", "F10", "H9", // 3
               "C8", "E9", "G9", "I8", // 4
           "B7", "D8", "F9", "H8", "K7", // 5
        "A6", "C7", "E8", "G8", "I7", "L6", // 6
           "B6", "D7", "F8", "H7", "K6", // 5
        "A5", "C6", "E7", "G7", "I6", "L5", // 6
           "B5", "D6", "F7", "H6", "K5", // 5
        "A4", "C5", "E6", "G6", "I5", "L4", // 6
           "B4", "D5", "F6", "H5", "K4", // 5
        "A3", "C4", "E5", "G5", "I4", "L3", // 6
           "B3", "D4", "F5", "H4", "K3", // 5
        "A2", "C3", "E4", "G4", "I3", "L2", // 6
           "B2", "D3", "F4", "H3", "K2", // 5
        "A1", "C2", "E3", "G3", "I2", "L1", // 6
           "B1", "D2", "F3", "H2", "K1", // 5
               "C1", "E2", "G2", "I1", // 4
                 "D1", "F2", "H1",  // 3
                   "E1", "G1", // 2
                      "F1", // 1
   };

    private static readonly int[] RowLength =
    {
        1,
        2,
        3,
        4,
        5,
        6,
        5,
        6,
        5,
        6,
        5,
        6,
        5,
        6,
        5,
        6,
        5,
        4,
        3,
        2,
        1
    };

    private static readonly Dictionary<string, int> CellIds = BoardLayout.Select(
        (cell, index) => new KeyValuePair<string,int>(cell.ToUpper(), index))
        .ToDictionary(pair => pair.Key, pair => pair.Value);

    public readonly Dictionary<int, CellState> Cells;

    public List<CellState> GetAllCells() => Cells.Values.ToList();

    public CellState this[string cell]
    {
        get
        {
            try
            {
                return Cells[CellIds[cell.ToUpper()]];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentOutOfRangeException(nameof(cell),
                    "Invalid cell notation. Valid cells are: A(1-6), B(1-7), C(1-8), D(1-9), " +
                    "E(1-10), F(1-11), G(1-10), H(1-9), I(1-8), K(1-7), and L(1-6). " +
                    $"{cell.ToUpper()} was provided instead.");
            }
        }
    }

    public List<CellState> File(char file)
    {
        // Convert the char to uppercase to ensure case insensitivity
        file = char.ToUpper(file);
        
        // Check if the character is valid
        if (file is < 'A' or > 'L' or 'J')
            throw new ArgumentOutOfRangeException(nameof(file), "Invalid file character. It should be between A and L, and not J.");
        
        return Cells.Values.Where(cell => cell.Coord[0] == file).ToList();
    }

    public List<CellState> Rank(int rank)
    {
        // Check if the number is valid
        if (rank is < 1 or > 11)
            throw new ArgumentOutOfRangeException(nameof(rank), "Invalid file number. It should be between 1 and 11.");
        
        // Convert rank to string for easy comparison
        string rankStr = rank.ToString();
        return Cells.Values.Where(
            cell => cell.Coord.Length == 3 ? cell.Coord[1..] == rankStr : cell.Coord[1] == rankStr[0])
            .ToList();
    }

    public List<CellState> Color(CellColor color)
    {
        List<string> coords = new();

        for (int i = (int)color; i < RowLength.Length; i++)
        {
            int position = RowLength[..i].Sum();
            var length = RowLength[i];

            int moduloResult = i % 3;
            CellColor currentColor = moduloResult switch
            {
                0 => CellColor.Dark,
                1 => CellColor.Medium,
                2 => CellColor.Light,
                _ => CellColor.Light  // This case should never be reached
            };
            
            if (currentColor == color)
                coords.AddRange(BoardLayout.Skip(position).Take(length));
        }

        List<CellState> result = GetAllCells().Where(cell => coords.Contains(cell.Coord)).ToList();

        return result;
    }

    public BoardState()
    {
        Cells = BoardLayout.Select((cell, index) =>
            new KeyValuePair<int, CellState>(index, new CellState(cell, index)))
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        foreach (var cell in Color(CellColor.Dark))
            cell.Color = CellColor.Dark;
        
        foreach (var cell in Color(CellColor.Medium))
            cell.Color = CellColor.Medium;
        
        foreach (var cell in Color(CellColor.Light))
            cell.Color = CellColor.Light;
        
        /* Black */

        foreach (var cell in Rank(7))
            this[cell.Coord].ActivePiece = PieceState.Pawn(cell.Id, false);

        for (int i = 9; i <= 11; i++)
            this[$"F{i}"].ActivePiece = PieceState.Bishop(this[$"F{i}"].Id, false);

        this["E10"].ActivePiece = PieceState.Queen(this["E10"].Id, false);
        this["G10"].ActivePiece = PieceState.King(this["G10"].Id, false);
        this["D9"].ActivePiece = PieceState.Knight(this["D9"].Id, false);
        this["H9"].ActivePiece = PieceState.Knight(this["H9"].Id, false);
        this["C8"].ActivePiece = PieceState.Rook(this["C8"].Id, false);
        this["I8"].ActivePiece = PieceState.Rook(this["C8"].Id, false);
        
        /* White */
        
        for (int i = 1; i <= 5; i++)
        {
            char file = (char)('G' - i);
            string cell = $"{file}{i}";
            this[cell].ActivePiece = PieceState.Pawn(this[cell].Id, true);
        }

        this["K1"].ActivePiece = PieceState.Pawn(this["K1"].Id, true);
        this["I2"].ActivePiece = PieceState.Pawn(this["I2"].Id, true);
        this["H3"].ActivePiece = PieceState.Pawn(this["H3"].Id, true);
        this["G4"].ActivePiece = PieceState.Pawn(this["G4"].Id, true);
        
        for (int i = 1; i <= 3; i++)
            this[$"F{i}"].ActivePiece = PieceState.Bishop(this[$"F{i}"].Id, false);
        
        this["E1"].ActivePiece = PieceState.Queen(this["E1"].Id, true);
        this["G1"].ActivePiece = PieceState.King(this["G1"].Id, true);
        this["D1"].ActivePiece = PieceState.Knight(this["D1"].Id, true);
        this["H1"].ActivePiece = PieceState.Knight(this["H1"].Id, true);
        this["C1"].ActivePiece = PieceState.Rook(this["C1"].Id, true);
        this["I1"].ActivePiece = PieceState.Rook(this["I1"].Id, true);
    }

    public static readonly char[] Files = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L' };
    public static readonly int[] Ranks = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
}