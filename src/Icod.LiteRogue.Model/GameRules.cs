namespace Icod.LiteRogue.Model;

public sealed record GameRules {
    public static GameRules Default { get; } = new();
    public int Width { get; init; } = 78;
    public int Height { get; init; } = 21;
    public int MaximumHealth { get; init; } = 30;
    public int PotionHealing { get; init; } = 10;
    public double RoomChance { get; init; } = 0.7;
}
