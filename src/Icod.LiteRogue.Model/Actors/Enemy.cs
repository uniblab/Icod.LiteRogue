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

using Icod.LiteRogue.Model.World;
namespace Icod.LiteRogue.Model.Actors;

public sealed record Enemy(int Id, string Name, char Glyph, GridPosition Position, int Health, int Attack);
public static class Combat {
	public static int Damage(int attack, int reduction) => Math.Max(1, attack - reduction);
}
