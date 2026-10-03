namespace Icod.LiteRogue.Model.World;

public readonly record struct GridPosition(int X, int Y);
public enum TerrainType { Wall, Floor, Corridor, Door, Staircase, Goal }
