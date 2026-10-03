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
