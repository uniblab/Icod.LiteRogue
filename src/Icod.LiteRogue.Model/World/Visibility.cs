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
