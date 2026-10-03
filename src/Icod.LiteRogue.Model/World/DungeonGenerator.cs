/*
	Icod.LiteRogue.Model
	Domain model for a turn-based Rogue-style dungeon game.
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

namespace Icod.LiteRogue.Model.World;

public static class DungeonGenerator {
	public static DungeonLevel Generate(int depth, int seed, GameRules rules) {
		ArgumentNullException.ThrowIfNull(rules);
		if (depth is < 1 or > 10) throw new ArgumentOutOfRangeException(nameof(depth));
		if (rules.Width < 18 || rules.Height < 18 || rules.Width % 3 != 0 || rules.Height % 3 != 0)
			throw new ArgumentException("Dungeon dimensions must be multiples of three and at least 18.", nameof(rules));
		if (!double.IsFinite(rules.RoomChance) || rules.RoomChance < 0 || rules.RoomChance > 1)
			throw new ArgumentException("Room chance must be between zero and one.", nameof(rules));
		int generationSeed = unchecked(seed * 397 ^ depth * 7919);
		for (int attempt = 0; attempt < 128; attempt++) {
			var random = new Random(unchecked(generationSeed ^ attempt * 104729));
			if (TryGenerate(depth, rules, random, out var level)) return level;
		}
		throw new InvalidOperationException("Unable to generate a dungeon satisfying the doorway and corridor constraints.");
	}
	private static bool TryGenerate(int depth, GameRules rules, Random random, out DungeonLevel level) {
		int width = rules.Width, height = rules.Height, sw = width / 3, sh = height / 3;
		var tiles = new TerrainType[width * height];
		var rooms = new List<Room>();
		for (int sector = 0; sector < 9; sector++) if (random.NextDouble() < rules.RoomChance) AddRoom(sector);
		if (rooms.Count == 0) AddRoom(random.Next(9));
		var connected = new List<Room> { rooms[random.Next(rooms.Count)] };
		var unconnected = rooms.Where(room => room != connected[0]).ToList();
		while (unconnected.Count > 0) {
			bool foundConnection = false;
			foreach (var candidate in unconnected.Select(room => new {
				Room = room,
				Distance = connected.Min(other => ManhattanDistance(room.Center, other.Center)),
				TieBreaker = random.Next()
			}).OrderBy(candidate => candidate.Distance).ThenBy(candidate => candidate.TieBreaker)) {
				foreach (var target in connected.OrderBy(target => ManhattanDistance(candidate.Room.Center, target.Center)).ThenBy(_ => random.Next())) {
					if (!TryConnect(candidate.Room, target)) continue;
					connected.Add(candidate.Room);
					unconnected.Remove(candidate.Room);
					foundConnection = true;
					break;
				}
				if (foundConnection) break;
			}
			if (!foundConnection) {
				level = null!;
				return false;
			}
		}
		var entry = connected[0].Center;
		var distances = new Dictionary<GridPosition, int> { { entry, 0 } };
		var queue = new Queue<GridPosition>(); queue.Enqueue(entry);
		while (queue.TryDequeue(out var p)) foreach (var (dx, dy) in new[] { (0, -1), (1, 0), (0, 1), (-1, 0) }) {
			var next = new GridPosition(p.X + dx, p.Y + dy);
			if (next.X >= 0 && next.Y >= 0 && next.X < width && next.Y < height && tiles[next.Y * width + next.X] != TerrainType.Wall && distances.TryAdd(next, distances[p] + 1)) queue.Enqueue(next);
		}
		var objective = distances.Where(pair => pair.Key != entry && rooms.Any(r => r.ContainsInterior(pair.Key)))
			.OrderByDescending(pair => pair.Value).ThenBy(pair => pair.Key.Y).ThenBy(pair => pair.Key.X).First().Key;
		tiles[objective.Y * width + objective.X] = depth == 10 ? TerrainType.Goal : TerrainType.Staircase;
		level = new DungeonLevel(width, height, tiles, rooms, entry, objective);
		return true;

		void AddRoom(int sector) {
			int rw = random.Next(4, Math.Min(24, sw - 2) + 1), rh = random.Next(4, Math.Min(5, sh - 2) + 1);
			int x = sector % 3 * sw + random.Next(1, sw - rw), y = sector / 3 * sh + random.Next(1, sh - rh);
			var room = new Room(sector, x, y, rw, rh); rooms.Add(room);
			for (int yy = y + 1; yy < y + rh - 1; yy++) for (int xx = x + 1; xx < x + rw - 1; xx++) tiles[yy * width + xx] = TerrainType.Floor;
		}
		bool TryConnect(Room firstRoom, Room secondRoom) {
			foreach (var first in Doorways(firstRoom).OrderBy(_ => random.Next())) foreach (var second in Doorways(secondRoom).OrderBy(_ => random.Next())) {
				if (DoorsTouch(first.Position, second.Position) || HasAdjacentDoor(first.Position) || HasAdjacentDoor(second.Position)) continue;
				if (!TryFindRoute(first.Outside, second.Outside, out var corridor)) continue;
				var hallway = corridor.Append(first.Position).Append(second.Position).ToHashSet();
				if (CreatesThickHallway(hallway)) continue;
				tiles[first.Position.Y * width + first.Position.X] = TerrainType.Door;
				tiles[second.Position.Y * width + second.Position.X] = TerrainType.Door;
				foreach (var position in corridor) tiles[position.Y * width + position.X] = TerrainType.Corridor;
				return true;
			}
			return false;
		}
		IEnumerable<Doorway> Doorways(Room room) {
			for (int x = room.X + 1; x < room.X + room.Width - 1; x++) {
				yield return new(new(x, room.Y), new(x, room.Y - 1));
				yield return new(new(x, room.Y + room.Height - 1), new(x, room.Y + room.Height));
			}
			for (int y = room.Y + 1; y < room.Y + room.Height - 1; y++) {
				yield return new(new(room.X, y), new(room.X - 1, y));
				yield return new(new(room.X + room.Width - 1, y), new(room.X + room.Width, y));
			}
		}
		bool HasAdjacentDoor(GridPosition position) {
			for (int y = position.Y - 1; y <= position.Y + 1; y++) for (int x = position.X - 1; x <= position.X + 1; x++)
				if (x >= 0 && y >= 0 && x < width && y < height && tiles[y * width + x] == TerrainType.Door) return true;
			return false;
		}
		bool TryFindRoute(GridPosition start, GridPosition end, out List<GridPosition> route) {
			route = [];
			if (tiles[start.Y * width + start.X] != TerrainType.Wall || tiles[end.Y * width + end.X] != TerrainType.Wall) return false;
			var parents = new Dictionary<GridPosition, GridPosition> { [start] = start };
			var pending = new Queue<GridPosition>(); pending.Enqueue(start);
			var directions = random.Next(2) == 0
				? new[] { new GridPosition(1, 0), new GridPosition(0, 1), new GridPosition(-1, 0), new GridPosition(0, -1) }
				: new[] { new GridPosition(0, 1), new GridPosition(1, 0), new GridPosition(0, -1), new GridPosition(-1, 0) };
			while (pending.TryDequeue(out var current)) {
				if (current == end) {
					for (var position = end; position != start; position = parents[position]) route.Add(position);
					route.Add(start);
					route.Reverse();
					return true;
				}
				foreach (var direction in directions) {
					var next = new GridPosition(current.X + direction.X, current.Y + direction.Y);
					if (next.X < 0 || next.Y < 0 || next.X >= width || next.Y >= height || tiles[next.Y * width + next.X] != TerrainType.Wall || parents.ContainsKey(next)) continue;
					if (next != start && next != end && HasAdjacentDoor(next)) continue;
					parents.Add(next, current);
					pending.Enqueue(next);
				}
			}
			return false;
		}
		bool CreatesThickHallway(IReadOnlySet<GridPosition> proposed) {
			foreach (var position in proposed) for (int originY = position.Y - 1; originY <= position.Y; originY++) for (int originX = position.X - 1; originX <= position.X; originX++) {
				if (originX < 0 || originY < 0 || originX + 1 >= width || originY + 1 >= height) continue;
				bool IsHallway(GridPosition point) => proposed.Contains(point) || tiles[point.Y * width + point.X] is TerrainType.Corridor or TerrainType.Door;
				if (IsHallway(new(originX, originY)) && IsHallway(new(originX + 1, originY)) && IsHallway(new(originX, originY + 1)) && IsHallway(new(originX + 1, originY + 1))) return true;
			}
			return false;
		}
	}
	private static int ManhattanDistance(GridPosition first, GridPosition second) => Math.Abs(first.X - second.X) + Math.Abs(first.Y - second.Y);
	private static bool DoorsTouch(GridPosition first, GridPosition second) => Math.Abs(first.X - second.X) <= 1 && Math.Abs(first.Y - second.Y) <= 1;
	private readonly record struct Doorway(GridPosition Position, GridPosition Outside);
}
