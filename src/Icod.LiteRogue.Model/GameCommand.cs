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

public enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest }
public enum CommandKind { Move, DrinkPotion, Descend }
public enum RunStatus { Playing, Won, Dead }
public readonly record struct GameCommand(CommandKind Kind, Direction Direction = Direction.North) {
	public static GameCommand Move(Direction direction) => new(CommandKind.Move, direction);
	public static GameCommand DrinkPotion => new(CommandKind.DrinkPotion);
	public static GameCommand Descend => new(CommandKind.Descend);
}
