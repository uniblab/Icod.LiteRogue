using Icod.LiteRogue.Model.World;
namespace Icod.LiteRogue.Model.Actors;

public sealed record Enemy(int Id, string Name, char Glyph, GridPosition Position, int Health, int Attack);
public static class Combat {
	public static int Damage(int attack, int reduction) => Math.Max(1, attack - reduction);
}
