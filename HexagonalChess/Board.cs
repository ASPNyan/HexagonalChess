namespace HexagonalChess;

public class Board
{
    // ew, this looks horrible like this.
    private static readonly string[] BoardLayout =
    {
                      "F11",
                   "E10", "G10",
                 "D9", "F10", "H9", 
               "C8", "E9", "G9", "I8",
           "B7", "D8", "F9", "H8", "K7",
        "A6", "C7", "E8", "G8", "I7", "L6",
           "B6", "D7", "F8", "H7", "K6",
        "A5", "C6", "E7", "G7", "I6", "L5",
           "B5", "D6", "F7", "H6", "K5",
        "A4", "C5", "E6", "G6", "I5", "L4",
           "B4", "D5", "F6", "H5", "K4",
        "A3", "C4", "E5", "G5", "I4", "L3",
           "B3", "D4", "F5", "H4", "K3",
        "A2", "C3", "E4", "G4", "I3", "L2",
           "B2", "D3", "F4", "H3", "K2",
        "A1", "C2", "E3", "G3", "I2", "L1",
           "B1", "D2", "F3", "H2", "K1",
               "C1", "E2", "G2", "I1",
                 "D1", "F2", "H1", 
                   "E1", "G1",
                      "F1",
   };

    private static readonly Dictionary<string, int> CellIds = (Dictionary<string, int>)BoardLayout.Select(
        (cell, index) => new KeyValuePair<string,int>(cell.ToUpper(), index));
    
    public readonly Dictionary<int, Cell> Cells = (Dictionary<int, Cell>)BoardLayout.Select(
        (cell, index) => new KeyValuePair<int, Cell>(index, new Cell(cell, index)));

    public Cell this[string cell]
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

    public List<Cell> File(char file)
    {
        // Convert the char to uppercase to ensure case insensitivity
        file = char.ToUpper(file);
        
        // Check if the character is valid
        if (file is < 'A' or > 'L' or 'J')
            throw new ArgumentOutOfRangeException(nameof(file), "Invalid file character. It should be between A and L, and not J.");
        
        return Cells.Values.Where(cell => cell.Coord[0] == file).ToList();
    }

    public List<Cell> Rank(int rank)
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

    public static Board Default()
    {
        Board board = new();
        
        /* Black */

        foreach (var cell in board.Rank(7))
            board[cell.Coord].ActivePiece = Piece.Pawn(cell.Id, false);

        for (int i = 9; i <= 11; i++)
            board[$"F{i}"].ActivePiece = Piece.Bishop(board[$"F{i}"].Id, false);

        board["E10"].ActivePiece = Piece.Queen(board["E10"].Id, false);
        board["G10"].ActivePiece = Piece.King(board["G10"].Id, false);
        board["D9"].ActivePiece = Piece.Knight(board["D9"].Id, false);
        board["H9"].ActivePiece = Piece.Knight(board["H9"].Id, false);
        board["C8"].ActivePiece = Piece.Rook(board["C8"].Id, false);
        board["I8"].ActivePiece = Piece.Rook(board["C8"].Id, false);
        
        /* White */
        
        for (int i = 1; i <= 5; i++)
        {
            char file = (char)('G' - i);
            string cell = $"{file}{i}";
            board[cell].ActivePiece = Piece.Pawn(board[cell].Id, true);
        }

        board["K1"].ActivePiece = Piece.Pawn(board["K1"].Id, true);
        board["I2"].ActivePiece = Piece.Pawn(board["I2"].Id, true);
        board["H3"].ActivePiece = Piece.Pawn(board["H3"].Id, true);
        board["G4"].ActivePiece = Piece.Pawn(board["G4"].Id, true);
        
        for (int i = 1; i <= 3; i++)
            board[$"F{i}"].ActivePiece = Piece.Bishop(board[$"F{i}"].Id, false);
        
        board["E1"].ActivePiece = Piece.Queen(board["E1"].Id, true);
        board["G1"].ActivePiece = Piece.King(board["G1"].Id, true);
        board["D1"].ActivePiece = Piece.Knight(board["D1"].Id, true);
        board["H1"].ActivePiece = Piece.Knight(board["H1"].Id, true);
        board["C1"].ActivePiece = Piece.Rook(board["C1"].Id, true);
        board["I1"].ActivePiece = Piece.Rook(board["I1"].Id, true);

        return board;
    }
}