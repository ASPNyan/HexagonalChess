namespace HexagonalChess.Game;

/// <summary>
/// Movement direction relative to player position. Diagonal means along a vertex.
/// <br/><br/>
/// Directions can also be written out as numbers on a clock. If you use this as an input method,
/// you can bitshift 1 by the hour on the clock.
/// </summary>
[Flags]
public enum MovementDirection
{
    /// <summary>
    /// Due to the knights unusual movement, it receives its own direction separate from the others.
    /// </summary>
    Knight = -1<<31,
    PawnAttack = DiagonalRight | DiagonalLeft,
    
    Forward = 1<<12,
    DiagonalForwardRight = 1<<1,
    ForwardRight = 1<<2,
    DiagonalRight = 1<<3,
    BackwardRight = 1<<4,
    DiagonalBackwardRight = 1<<5,
    Backward = 1<<6,
    DiagonalBackwardLeft = 1<<7,
    BackwardLeft = 1<<8,
    DiagonalLeft = 1<<9,
    ForwardLeft = 1<<10,
    DiagonalForwardLeft = 1<<11
}