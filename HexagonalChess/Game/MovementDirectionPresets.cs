namespace HexagonalChess.Game;

public static class MovementDirectionPresets
{
    public const MovementDirection Pawn = MovementDirection.Forward;
    public const MovementDirection PawnAttack = MovementDirection.PawnAttack;

    public const MovementDirection Bishop = MovementDirection.DiagonalForwardLeft |
                                            MovementDirection.DiagonalForwardRight | MovementDirection.DiagonalLeft |
                                            MovementDirection.DiagonalRight | MovementDirection.DiagonalBackwardLeft |
                                            MovementDirection.DiagonalBackwardRight;

    public const MovementDirection Knight = MovementDirection.Knight;

    public const MovementDirection Rook = MovementDirection.ForwardLeft | MovementDirection.Forward |
                                          MovementDirection.ForwardRight | MovementDirection.BackwardLeft |
                                          MovementDirection.Backward | MovementDirection.BackwardRight;

    public const MovementDirection Queen = Bishop | Rook;

    public const MovementDirection King = Queen;
}