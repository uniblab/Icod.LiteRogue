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

using Icod.LiteRogue.Model;
using Icod.LiteRogue.Terminal;
namespace Icod.LiteRogue.Controller;

public static class KeyBindings {
	public static bool TryTranslate(TerminalKey key, out GameCommand command) {
		command = default;
		Direction? direction = key.Navigation switch {
			NavigationKey.Up => Direction.North,
			NavigationKey.Down => Direction.South,
			NavigationKey.Left => Direction.West,
			NavigationKey.Right => Direction.East,
			NavigationKey.Home => Direction.NorthWest,
			NavigationKey.End => Direction.SouthWest,
			NavigationKey.PageUp => Direction.NorthEast,
			NavigationKey.PageDown => Direction.SouthEast,
			_ => key.Character switch {
				'k' or '8' => Direction.North,
				'u' or '9' => Direction.NorthEast,
				'l' or '6' => Direction.East,
				'n' or '3' => Direction.SouthEast,
				'j' or '2' => Direction.South,
				'b' or '1' => Direction.SouthWest,
				'h' or '4' => Direction.West,
				'y' or '7' => Direction.NorthWest,
				_ => null
			}
		};
		if (direction is { } d) { command = GameCommand.Move(d); return true; }
		if (key.Character == 'p') { command = GameCommand.DrinkPotion; return true; }
		if (key.Character == '>') { command = GameCommand.Descend; return true; }
		return false;
	}
}
