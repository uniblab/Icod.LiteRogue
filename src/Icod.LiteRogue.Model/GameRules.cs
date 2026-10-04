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

namespace Icod.LiteRogue.Model;

public sealed record GameRules {
	public static IReadOnlyList<Items.Weapon> Weapons { get; } = Array.AsReadOnly(new Items.Weapon[] { new("Knife", 4), new("Sword", 6), new("Axe", 8), new("Runeblade", 10) });
	public static IReadOnlyList<Items.Armor> Armors { get; } = Array.AsReadOnly(new Items.Armor[] { new("Clothes", 0), new("Leather", 1), new("Chain", 2), new("Plate", 3) });
	public static (string Name, char Glyph, int Health, int Attack) EnemyTier(int depth) => depth switch { <= 2 => ("Rat", 'r', 4, 1), <= 5 => ("Goblin", 'g', 8, 2), <= 8 => ("Orc", 'o', 12, 3), _ => ("Troll", 'T', 16, 4) };
	public static GameRules Default { get; } = new();
	public int Width { get; init; } = 78;
	public int Height { get; init; } = 21;
	public int MaximumHealth { get; init; } = 30;
	public int PotionHealing { get; init; } = 10;
	public double RoomChance { get; init; } = 0.7;
}
