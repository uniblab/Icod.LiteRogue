using Icod.LiteRogue.Model.World;
namespace Icod.LiteRogue.Model.Items;

public enum LootKind { Weapon, Armor, Potion }
public sealed record Loot(GridPosition Position, LootKind Kind, string Name, int Strength = 0);
