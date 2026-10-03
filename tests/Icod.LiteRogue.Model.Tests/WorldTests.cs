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
	[Fact]
	public void RoomsStayWithinSectors() {
		for (int seed = 0; seed < 100; seed++) {
			var level = DungeonGenerator.Generate(1, seed, GameRules.Default);
			Assert.NotEmpty(level.Rooms);
			Assert.Equal(level.Rooms.Count, level.Rooms.Select(r => r.Sector).Distinct().Count());
			foreach (var room in level.Rooms) {
				int sx = room.Sector % 3 * 26, sy = room.Sector / 3 * 7;
				Assert.InRange(room.X, sx + 1, sx + 25 - room.Width);
				Assert.InRange(room.Y, sy + 1, sy + 6 - room.Height);
			}
		}
	}
	[Fact]
	public void AllWalkableTilesConnected() {
		for (int seed = 0; seed < 1000; seed++) for (int depth = 1; depth <= 10; depth++) {
			var level = DungeonGenerator.Generate(depth, seed, GameRules.Default);
			var reached = Reach(level, level.Entry);
			Assert.True(reached.SetEquals(level.WalkablePositions()), $"Disconnected seed {seed}, depth {depth}");
			Assert.Contains(level.Objective, reached);
			Assert.NotEqual(level.Entry, level.Objective);
		}
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
		Assert.Equal(first.WalkablePositions().Select(first.At), second.WalkablePositions().Select(second.At));
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
