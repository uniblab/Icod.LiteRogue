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

internal static class Fixtures {
	internal static GameSession Session(string[] rows, GridPosition? entry = null, IEnumerable<Actors.Enemy>? enemies = null, int? health = null) {
		var level = Level(rows);
		level = new DungeonLevel(level.Width, level.Height, rows.SelectMany(row => row.Select(c => c switch { '#' => TerrainType.Wall, '>' => TerrainType.Staircase, '&' => TerrainType.Goal, _ => TerrainType.Floor })).ToArray(), [], entry ?? new(1, 1), level.Objective);
		return new GameSession(7, GameRules.Default, level, enemies, health);
	}
	internal static DungeonLevel Level(params string[] rows) {
		int width = rows[0].Length;
		var tiles = rows.SelectMany(row => row.Select(c => c switch { '#' => TerrainType.Wall, '>' => TerrainType.Staircase, '&' => TerrainType.Goal, _ => TerrainType.Floor })).ToArray();
		return new DungeonLevel(width, rows.Length, tiles, [], new(1, 1), new(width - 2, rows.Length - 2));
	}
}
