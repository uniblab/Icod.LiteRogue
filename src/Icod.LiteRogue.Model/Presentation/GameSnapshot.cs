using Icod.LiteRogue.Model.Items;
using Icod.LiteRogue.Model.World;

namespace Icod.LiteRogue.Model.Presentation;

public readonly record struct TileSnapshot(GridPosition Position, TerrainType Terrain, bool Explored, bool Visible);
public sealed record GameSnapshot(int Seed, int Depth, RunStatus Status, int Health, int MaximumHealth, Weapon Weapon, Armor Armor, int Potions, GridPosition Player, int Width, int Height, IReadOnlyList<TileSnapshot> Tiles, long Turn=0, IReadOnlyList<Actors.Enemy>? Enemies=null,IReadOnlyList<Loot>? Loot=null);
