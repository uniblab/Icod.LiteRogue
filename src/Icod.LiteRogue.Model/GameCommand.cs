namespace Icod.LiteRogue.Model;

public enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest }
public enum CommandKind { Move, DrinkPotion, Descend }
public enum RunStatus { Playing, Won, Dead }
public readonly record struct GameCommand(CommandKind Kind, Direction Direction = Direction.North) {
	public static GameCommand Move(Direction direction) => new(CommandKind.Move, direction);
	public static GameCommand DrinkPotion => new(CommandKind.DrinkPotion);
	public static GameCommand Descend => new(CommandKind.Descend);
}
