namespace Icod.LiteRogue.Model;

public sealed record GameRules {
	public static IReadOnlyList<Items.Weapon> Weapons { get; } = Array.AsReadOnly(new Items.Weapon[] { new("Knife", 4), new("Sword", 6), new("Axe", 8), new("Runeblade", 10) });
	public static IReadOnlyList<Items.Armor> Armors { get; } = Array.AsReadOnly(new Items.Armor[] { new("Clothes", 0), new("Leather", 1), new("Chain", 2), new("Plate", 3) });
	public static (string Name, char Glyph, int Health, int Attack) EnemyTier(int depth) => depth switch { <= 2 => ("Rat", 'r', 4, 1), <= 5 => ("Goblin", 'g', 8, 2), <= 8 => ("Orc", 'o', 12, 3), _ => ("Troll", 'T', 16, 4) };
	public static GameRules Default { get; } = new();
	public int Width { get; init; } = 78;
	public int Height { get; init; } = 21;
	public int MaximumHealth { get; init; } = 30;
	public int PotionHealing { get; init; } = 10;
	public double RoomChance { get; init; } = 0.7;
}
