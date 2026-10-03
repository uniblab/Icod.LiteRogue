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

using Icod.LiteRogue.Model.Actors;
namespace Icod.LiteRogue.Model.World;

public static class LevelPopulation {
	public static IReadOnlyList<Items.Loot> Loot(DungeonLevel level, int depth, int seed, IReadOnlyList<Enemy> enemies) {
		var random = new Random(unchecked(seed * 151 + depth * 797));
		var available = level.WalkablePositions().Where(p => p != level.Entry && p != level.Objective && enemies.All(e => e.Position != p)).OrderBy(_ => random.Next()).ToArray();
		int tier = Math.Min(3, depth / 3); var weapon = GameRules.Weapons[tier]; var armor = GameRules.Armors[tier];
		var contents = new List<(Items.LootKind Kind, string Name, int Strength)> { (Items.LootKind.Potion, "Healing potion", 0), (Items.LootKind.Weapon, weapon.Name, weapon.Damage), (Items.LootKind.Armor, armor.Name, armor.Reduction) };
		contents.AddRange(Enumerable.Repeat((Items.LootKind.Potion, "Healing potion", 0), 1 + depth / 3));
		return Array.AsReadOnly(contents.Take(available.Length).Select((item, index) => new Items.Loot(available[index], item.Kind, item.Name, item.Strength)).ToArray());
	}
	public static IReadOnlyList<Enemy> Enemies(DungeonLevel level, int depth, int seed) {
		var random = new Random(unchecked(seed * 127 + depth * 733));
		var available = level.WalkablePositions().Where(p => p != level.Entry && p != level.Objective && Math.Max(Math.Abs(p.X - level.Entry.X), Math.Abs(p.Y - level.Entry.Y)) >= 4).OrderBy(_ => random.Next()).ToArray();
		var tier = GameRules.EnemyTier(depth);
		return Array.AsReadOnly(available.Take(2 + (depth - 1) / 2).Select((p, id) => new Enemy(id, tier.Name, tier.Glyph, p, tier.Health, tier.Attack)).ToArray());
	}
}
