using Icod.LiteRogue.Model.World;
namespace Icod.LiteRogue.Model.Tests;

public sealed class PopulationTests {
	[Fact]
	public void PopulationRespectsReservedTiles() {
		for (int seed = 0; seed < 1000; seed++) for (int depth = 1; depth <= 10; depth++) {
			var level = DungeonGenerator.Generate(depth, seed, GameRules.Default); var enemies = LevelPopulation.Enemies(level, depth, seed); var loot = LevelPopulation.Loot(level, depth, seed, enemies);
			var positions = enemies.Select(e => e.Position).Concat(loot.Select(l => l.Position)).ToArray();
			Assert.Equal(positions.Length, positions.Distinct().Count());
			Assert.All(positions, p => { Assert.True(level.IsWalkable(p), $"seed {seed}, depth {depth}"); Assert.NotEqual(level.Entry, p); Assert.NotEqual(level.Objective, p); });
			Assert.Contains(loot, l => l.Kind == Items.LootKind.Potion);
		}
	}
	[Fact]
	public void DensePopulationRemainsValid() {
		var level = Fixtures.Level("#####", "#...#", "#####"); var enemies = LevelPopulation.Enemies(level, 10, 7); var loot = LevelPopulation.Loot(level, 10, 7, enemies);
		Assert.Equal(1, loot.Count + enemies.Count); Assert.Equal(new(2, 1), Assert.Single(loot).Position);
	}
	[Fact]
	public void PopulationContainsLoot() {
		var level = DungeonGenerator.Generate(1, 7, GameRules.Default); var expected = LevelPopulation.Loot(level, 1, 7, LevelPopulation.Enemies(level, 1, 7));
		Assert.NotEmpty(expected);
	}
}
