namespace Icod.LiteRogue.Model.World;

public static class DungeonGenerator {
	public static DungeonLevel Generate(int depth, int seed, GameRules rules) {
		ArgumentNullException.ThrowIfNull(rules);
		if (depth is < 1 or > 10) throw new ArgumentOutOfRangeException(nameof(depth));
		if (rules.Width < 18 || rules.Height < 18 || rules.Width % 3 != 0 || rules.Height % 3 != 0)
			throw new ArgumentException("Dungeon dimensions must be multiples of three and at least 18.", nameof(rules));
		if (!double.IsFinite(rules.RoomChance) || rules.RoomChance < 0 || rules.RoomChance > 1)
			throw new ArgumentException("Room chance must be between zero and one.", nameof(rules));
		var random = new Random(unchecked(seed * 397 ^ depth * 7919));
		int width = rules.Width, height = rules.Height, sw = width / 3, sh = height / 3;
		var tiles = new TerrainType[width * height];
		var rooms = new List<Room>();
		for (int sector = 0; sector < 9; sector++) if (random.NextDouble() < rules.RoomChance) AddRoom(sector);
		if (rooms.Count == 0) AddRoom(random.Next(9));
		var connected = rooms.OrderBy(_ => random.Next()).ToArray();
		for (int i = 1; i < connected.Length; i++) Connect(connected[i].Center, connected[random.Next(i)].Center);
		if (connected.Length > 2 && random.Next(2) == 0) Connect(connected[0].Center, connected[^1].Center);
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
		return new DungeonLevel(width, height, tiles, rooms, entry, objective);

		void AddRoom(int sector) {
			int rw = random.Next(4, Math.Min(24, sw - 2) + 1), rh = random.Next(4, Math.Min(5, sh - 2) + 1);
			int x = sector % 3 * sw + random.Next(1, sw - rw), y = sector / 3 * sh + random.Next(1, sh - rh);
			var room = new Room(sector, x, y, rw, rh); rooms.Add(room);
			for (int yy = y + 1; yy < y + rh - 1; yy++) for (int xx = x + 1; xx < x + rw - 1; xx++) tiles[yy * width + xx] = TerrainType.Floor;
		}
		void Connect(GridPosition a, GridPosition b) {
			var p = a; bool horizontalFirst = random.Next(2) == 0;
			if (horizontalFirst) { AlongX(); AlongY(); } else { AlongY(); AlongX(); }
			void AlongX() { while (p.X != b.X) { p = new(p.X + Math.Sign(b.X - p.X), p.Y); Carve(p); } }
			void AlongY() { while (p.Y != b.Y) { p = new(p.X, p.Y + Math.Sign(b.Y - p.Y)); Carve(p); } }
		}
		void Carve(GridPosition p) {
			if (tiles[p.Y * width + p.X] != TerrainType.Wall) return;
			tiles[p.Y * width + p.X] = rooms.Any(r => r.Contains(p) && !r.ContainsInterior(p)) ? TerrainType.Door : TerrainType.Corridor;
		}
	}
}
