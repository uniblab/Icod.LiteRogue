/*
	Icod.LiteRogue
	Text-mode Rogue-style dungeon game built with Icod.DCurses.
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

using Icod.LiteRogue.Model.Presentation;
namespace Icod.LiteRogue.View;

public static class FrameBuilder {
	public static IReadOnlyList<string> Build(GameSnapshot snapshot, int columns, int rows, IReadOnlyList<string> messages, bool help) {
		if (columns <= 0 || rows <= 0) return Array.Empty<string>();
		string Clip(string line) => line[..Math.Min(line.Length, columns)];
		if (columns < 30 || rows < 6) return Array.AsReadOnly(new[] { Clip("Resize terminal to at least 30x6.") });
		if (help) return Array.AsReadOnly(new[] { "Icod.LiteRogue - reach floor 10's fountain", "Move: arrows, hjklyubn, keypad 1-9", "Bump foes to attack. Walk over loot.", "p: healing potion   >: descend on stairs", "?: close help   q: quit   r: new run after end", "@ you # wall . floor : corridor + door", "> stairs & fountain ) weapon ] armor ! potion", "Terrain remains mapped; foes need current sight." }.Take(rows).Select(Clip).ToArray());
		var lines = new List<string> { Clip($"HP {snapshot.Health}/{snapshot.MaximumHealth}  Floor {snapshot.Depth}/10  Potions {snapshot.Potions}  Seed {snapshot.Seed}"), Clip($"{snapshot.Weapon.Name} ({snapshot.Weapon.Damage}) / {snapshot.Armor.Name} ({snapshot.Armor.Reduction})  Turn {snapshot.Turn}") };
		int width = Math.Min(snapshot.Width, columns), height = Math.Min(snapshot.Height, rows - 4);
		int left = Math.Clamp(snapshot.Player.X - width / 2, 0, snapshot.Width - width), top = Math.Clamp(snapshot.Player.Y - height / 2, 0, snapshot.Height - height);
		var tiles = snapshot.Tiles.ToDictionary(t => t.Position);
		for (int y = top; y < top + height; y++) {
			var line = new char[width];
			for (int x = left; x < left + width; x++) {
				var position = new Model.World.GridPosition(x, y); var tile = tiles[position];
				line[x - left] = position == snapshot.Player ? '@' : !tile.Explored ? ' ' : tile.Terrain switch {
					Model.World.TerrainType.Wall => '#',
					Model.World.TerrainType.Floor => '.',
					Model.World.TerrainType.Corridor => ':',
					Model.World.TerrainType.Door => '+',
					Model.World.TerrainType.Staircase => '>',
					Model.World.TerrainType.Goal => '&',
					_ => ' '
				};
				if (position != snapshot.Player && snapshot.Loot?.FirstOrDefault(e => e.Position == position) is { } item) line[x - left] = item.Kind switch { Model.Items.LootKind.Weapon => ')', Model.Items.LootKind.Armor => ']', _ => '!' };
				if (position != snapshot.Player && snapshot.Enemies?.FirstOrDefault(e => e.Position == position) is { } enemy) line[x - left] = enemy.Glyph;
			}
			lines.Add(new string(line));
		}
		lines.Add(Clip(snapshot.Status switch { Model.RunStatus.Won => "You reached the fountain. YOU WIN! r: new run; q: quit", Model.RunStatus.Dead => "You died. The run is over. r: new run; q: quit", _ => messages.LastOrDefault() ?? "@ you # wall . floor : corridor + door > stairs & goal" }));
		lines.Add(Clip("Move hjklyubn / keypad / arrows | p potion | > down | ? help | q quit"));
		return lines.AsReadOnly();
	}
}
