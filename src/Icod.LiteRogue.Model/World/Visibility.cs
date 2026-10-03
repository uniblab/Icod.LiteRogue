namespace Icod.LiteRogue.Model.World;

public sealed class Visibility {
	private readonly int width, height;
	private readonly HashSet<GridPosition> explored = [];
	private readonly HashSet<GridPosition> visible = [];
	public Visibility(int width, int height) { this.width = width; this.height = height; }
	public void Update(DungeonLevel level, GridPosition player) {
		visible.Clear();
		foreach (var room in level.Rooms.Where(r => r.ContainsInterior(player)))
			for (int y = room.Y; y < room.Y + room.Height; y++) for (int x = room.X; x < room.X + room.Width; x++) See(new(x, y));
		for (int dy = -1; dy <= 1; dy++) for (int dx = -1; dx <= 1; dx++) {
			if (dx != 0 && dy != 0 && !level.IsWalkable(new(player.X + dx, player.Y)) && !level.IsWalkable(new(player.X, player.Y + dy))) continue;
			See(new(player.X + dx, player.Y + dy));
		}
		explored.UnionWith(visible);
	}
	public bool IsVisible(GridPosition position) => visible.Contains(position);
	public bool IsExplored(GridPosition position) => explored.Contains(position);
	private void See(GridPosition p) { if (p.X >= 0 && p.Y >= 0 && p.X < width && p.Y < height) visible.Add(p); }
}
