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

using Icod.LiteRogue.Model.Items;
using Icod.LiteRogue.Model.World;

namespace Icod.LiteRogue.Model.Presentation;

public readonly record struct TileSnapshot(GridPosition Position, TerrainType Terrain, bool Explored, bool Visible);
public sealed record GameSnapshot(int Seed, int Depth, RunStatus Status, int Health, int MaximumHealth, Weapon Weapon, Armor Armor, int Potions, GridPosition Player, int Width, int Height, IReadOnlyList<TileSnapshot> Tiles, long Turn = 0, IReadOnlyList<Actors.Enemy>? Enemies = null, IReadOnlyList<Loot>? Loot = null);
