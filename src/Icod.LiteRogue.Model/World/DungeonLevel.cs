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

public sealed record Room(int Sector, int X, int Y, int Width, int Height) {
	public GridPosition Center => new(X + Width / 2, Y + Height / 2);
	public bool Contains(GridPosition p) => p.X >= X && p.Y >= Y && p.X < X + Width && p.Y < Y + Height;
	public bool ContainsInterior(GridPosition p) => p.X > X && p.Y > Y && p.X < X + Width - 1 && p.Y < Y + Height - 1;
}
public sealed class DungeonLevel {
	private readonly TerrainType[] tiles;
	internal DungeonLevel(int width, int height, TerrainType[] tiles, IReadOnlyList<Room> rooms, GridPosition entry, GridPosition objective) {
		Width = width; Height = height; this.tiles = (TerrainType[])tiles.Clone(); Rooms = Array.AsReadOnly(rooms.ToArray()); Entry = entry; Objective = objective;
	}
	public int Width { get; }
	public int Height { get; }
	public IReadOnlyList<Room> Rooms { get; }
	public GridPosition Entry { get; }
	public GridPosition Objective { get; }
	public bool InBounds(GridPosition p) => p.X >= 0 && p.Y >= 0 && p.X < Width && p.Y < Height;
	public TerrainType At(GridPosition p) => InBounds(p) ? tiles[p.Y * Width + p.X] : TerrainType.Wall;
	public bool IsWalkable(GridPosition p) => InBounds(p) && At(p) != TerrainType.Wall;
	public bool CanStep(GridPosition from, GridPosition to) {
		if (!IsWalkable(from) || !IsWalkable(to)) return false;
		int dx = to.X - from.X, dy = to.Y - from.Y;
		if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1 || (dx == 0 && dy == 0)) return false;
		return dx == 0 || dy == 0 || IsWalkable(new(from.X + dx, from.Y)) || IsWalkable(new(from.X, from.Y + dy));
	}
	public IEnumerable<GridPosition> WalkablePositions() {
		for (int y = 0; y < Height; y++) for (int x = 0; x < Width; x++) { var p = new GridPosition(x, y); if (IsWalkable(p)) yield return p; }
	}
}
