/*
	Icod.LiteRogue.Model.Tests
	Automated test suite for the Icod.LiteRogue domain model.
	Copyright (C) 2026  Timothy J. Bruce <uniblab@hotmail.com>
*/

/*
	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Icod.LiteRogue.Model.World;
namespace Icod.LiteRogue.Model.Tests;

public sealed class PopulationTests {
	internal static void AssertPopulationRespectsReservedTiles(DungeonLevel level, int seed, int depth) {
		var enemies = LevelPopulation.Enemies(level, depth, seed);
		var loot = LevelPopulation.Loot(level, depth, seed, enemies);
		var positions = enemies.Select(e => e.Position).Concat(loot.Select(l => l.Position)).ToArray();
		Assert.Equal(positions.Length, positions.Distinct().Count());
		Assert.All(positions, p => { Assert.True(level.IsWalkable(p), $"seed {seed}, depth {depth}"); Assert.NotEqual(level.Entry, p); Assert.NotEqual(level.Objective, p); });
		Assert.Contains(loot, l => l.Kind == Items.LootKind.Potion);
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
