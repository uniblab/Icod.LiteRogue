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

public sealed class WorldTests {
	[Theory]
	[InlineData(0)]
	[InlineData(100)]
	[InlineData(200)]
	[InlineData(300)]
	[InlineData(400)]
	[InlineData(500)]
	[InlineData(600)]
	[InlineData(700)]
	[InlineData(800)]
	[InlineData(900)]
	public void GeneratedMapsAndPopulationRespectAllConstraints(int firstSeed) {
		// Check every invariant on the same 10,000 maps instead of generating
		// those maps again for each invariant and for population placement.
		for (int seed = firstSeed; seed < firstSeed + 100; seed++) for (int depth = 1; depth <= 10; depth++) {
			var level = DungeonGenerator.Generate(depth, seed, GameRules.Default);
			AssertMapConstraints(level, seed, depth);
			PopulationTests.AssertPopulationRespectsReservedTiles(level, seed, depth);
		}
	}
	[Theory]
	[InlineData(18, 18, 1)]
	[InlineData(21, 21, 1)]
	[InlineData(78, 21, 1)]
	[InlineData(78, 21, 0)]
	[InlineData(30, 30, 0.7)]
	public void MapConstraintsHoldForDenseSparseAndMinimumSizedLayouts(int width, int height, double roomChance) {
		var rules = GameRules.Default with { Width = width, Height = height, RoomChance = roomChance };
		for (int seed = 0; seed < 100; seed++) {
			int depth = seed % 10 + 1;
			var level = DungeonGenerator.Generate(depth, seed, rules);
			AssertMapConstraints(level, seed, depth);
			if (roomChance == 1) Assert.Equal(9, level.Rooms.Count);
			if (roomChance == 0) Assert.Single(level.Rooms);
		}
	}
	private static void AssertMapConstraints(DungeonLevel level, int seed, int depth) {
		string context = $"seed {seed}, depth {depth}, {level.Width}x{level.Height}";
		for (int x = 0; x < level.Width; x++) {
			Assert.True(level.At(new(x, 0)) == TerrainType.Wall, $"Open top border for {context} at x={x}");
			Assert.True(level.At(new(x, level.Height - 1)) == TerrainType.Wall, $"Open bottom border for {context} at x={x}");
		}
		for (int y = 0; y < level.Height; y++) {
			Assert.True(level.At(new(0, y)) == TerrainType.Wall, $"Open left border for {context} at y={y}");
			Assert.True(level.At(new(level.Width - 1, y)) == TerrainType.Wall, $"Open right border for {context} at y={y}");
		}
		var walkable = level.WalkablePositions().ToArray();
		var reached = Reach(level, level.Entry);
		Assert.True(reached.SetEquals(walkable), $"Disconnected {context}");
		Assert.Contains(level.Objective, reached);
		Assert.NotEqual(level.Entry, level.Objective);
		Assert.Equal(depth == 10 ? TerrainType.Goal : TerrainType.Staircase, level.At(level.Objective));
		Assert.NotEmpty(level.Rooms);
		Assert.Equal(level.Rooms.Count, level.Rooms.Select(r => r.Sector).Distinct().Count());
		foreach (var room in level.Rooms) {
			int sw = level.Width / 3, sh = level.Height / 3;
			int sx = room.Sector % 3 * sw, sy = room.Sector / 3 * sh;
			Assert.InRange(room.X, sx + 1, sx + sw - 1 - room.Width);
			Assert.InRange(room.Y, sy + 1, sy + sh - 1 - room.Height);
			foreach (var position in walkable.Where(room.Contains)) {
				Assert.True(level.At(position) != TerrainType.Corridor, $"Corridor inside room for {context} at {position}");
				if (!room.ContainsInterior(position)) Assert.Equal(TerrainType.Door, level.At(position));
			}
		}
		var doors = walkable.Where(p => level.At(p) == TerrainType.Door).ToArray();
		foreach (var door in doors) foreach (var other in doors)
			if (door != other)
				Assert.True(Math.Abs(door.X - other.X) > 1 || Math.Abs(door.Y - other.Y) > 1, $"Adjacent doors for {context}");
		bool IsHallway(GridPosition p) => level.At(p) is TerrainType.Corridor or TerrainType.Door;
		for (int y = 0; y < level.Height - 1; y++) for (int x = 0; x < level.Width - 1; x++)
			if (IsHallway(new(x, y)) && IsHallway(new(x + 1, y)) && IsHallway(new(x, y + 1)) && IsHallway(new(x + 1, y + 1)))
				Assert.Fail($"Two-cell-thick hallway for {context} at ({x}, {y})");
	}
	[Fact]
	public void TwoRoomsWithFacingWallsUseAShortStraightPassage() {
		int checkedLayouts = 0;
		for (int seed = 0; seed < 1000; seed++) {
			var level = DungeonGenerator.Generate(1, seed, GameRules.Default with { RoomChance = 0.2 });
			if (level.Rooms.Count != 2 || level.Rooms[0].Sector / 3 != level.Rooms[1].Sector / 3) continue;
			var left = level.Rooms.OrderBy(room => room.X).First();
			var right = level.Rooms.OrderBy(room => room.X).Last();
			int gap = right.X - (left.X + left.Width);
			var corridor = level.WalkablePositions().Where(p => level.At(p) == TerrainType.Corridor).ToArray();
			Assert.True(corridor.Length == gap, $"Unnecessary detour for seed {seed}: expected {gap} corridor cells, got {corridor.Length}");
			Assert.Single(corridor.Select(p => p.Y).Distinct());
			checkedLayouts++;
		}
		Assert.True(checkedLayouts > 0, "No layouts with two horizontally facing rooms were checked.");
	}
	[Fact]
	public void SingleRoomHasDistinctEntryAndObjective() {
		var level = DungeonGenerator.Generate(1, 7, GameRules.Default with { RoomChance = 0 });
		Assert.Single(level.Rooms);
		Assert.NotEqual(level.Entry, level.Objective);
		Assert.Contains(level.Objective, Reach(level, level.Entry));
	}
	[Fact]
	public void GenerationRepeatsForSeed() {
		var first = DungeonGenerator.Generate(10, 37, GameRules.Default);
		var second = DungeonGenerator.Generate(10, 37, GameRules.Default);
		Assert.Equal(first.Rooms, second.Rooms);
		Assert.Equal(first.WalkablePositions(), second.WalkablePositions());
		Assert.Equal(first.WalkablePositions().Select(first.At), second.WalkablePositions().Select(second.At));
		Assert.Equal(first.Entry, second.Entry);
		Assert.Equal(first.Objective, second.Objective);
		Assert.Equal(TerrainType.Goal, first.At(first.Objective));
	}
	[Fact]
	public void RoomEntryRevealsInterior() {
		var level = DungeonGenerator.Generate(1, 7, GameRules.Default with { RoomChance = 0 });
		var visibility = new Visibility(level.Width, level.Height);
		visibility.Update(level, level.Entry);
		var room = Assert.Single(level.Rooms);
		for (int y = room.Y + 1; y < room.Y + room.Height - 1; y++)
			for (int x = room.X + 1; x < room.X + room.Width - 1; x++)
				Assert.True(visibility.IsVisible(new(x, y)));
	}
	[Fact]
	public void ExploredTerrainPersists() {
		var level = Fixtures.Level("#######", "#.....#", "#######");
		var visibility = new Visibility(7, 3);
		visibility.Update(level, new(1, 1));
		visibility.Update(level, new(5, 1));
		Assert.True(visibility.IsExplored(new(1, 1)));
		Assert.False(visibility.IsVisible(new(1, 1)));
	}
	[Fact]
	public void CorridorVisibilityStopsAtWalls() {
		var level = Fixtures.Level("#####", "#.#.#", "#####");
		var visibility = new Visibility(5, 3);
		visibility.Update(level, new(1, 1));
		Assert.False(visibility.IsVisible(new(3, 1)));
	}
	internal static HashSet<GridPosition> Reach(DungeonLevel level, GridPosition start) {
		var found = new HashSet<GridPosition> { start };
		var queue = new Queue<GridPosition>(); queue.Enqueue(start);
		while (queue.TryDequeue(out var p)) foreach (var (dx, dy) in new[] { (0, -1), (1, 0), (0, 1), (-1, 0) }) {
			var next = new GridPosition(p.X + dx, p.Y + dy);
			if (level.IsWalkable(next) && found.Add(next)) queue.Enqueue(next);
		}
		return found;
	}
}
