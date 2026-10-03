using Icod.LiteRogue.Model.World;
using Icod.LiteRogue.Model.Items;
namespace Icod.LiteRogue.Model.Tests;

public sealed class DescentTests {
	[Fact] public void DescentRequiresStaircase() => Assert.False(Fixtures.Session(["#####", "#...#", "#####"]).Apply(GameCommand.Descend).Accepted);
	[Fact]
	public void DescentPreservesResources() {
		var next = Fixtures.Level("#####", "#...#", "#####");
		var game = new GameSession(7, GameRules.Default, Fixtures.Level("#####", "#>..#", "#####"), health: 25, potions: 2, weapon: new("Sword", 6), armor: new("Leather", 1), floorSource: _ => new(next, [new(1, "Rat", 'r', new(2, 1), 8, 5)], []));
		var result = game.Apply(GameCommand.Descend); Assert.True(result.Accepted); Assert.Equal(2, result.Snapshot.Depth); Assert.Equal(25, result.Snapshot.Health); Assert.Equal(2, result.Snapshot.Potions); Assert.Equal(6, result.Snapshot.Weapon.Damage); Assert.Equal(1, result.Snapshot.Armor.Reduction); Assert.Equal(1, result.Snapshot.Turn);
	}
	[Fact]
	public void NewFloorStartsUnexplored() {
		var next = DungeonGenerator.Generate(2, 7, GameRules.Default);
		var game = new GameSession(7, GameRules.Default, Fixtures.Level("#####", "#>..#", "#####"), floorSource: _ => new(next, [], []));
		var snapshot = game.Apply(GameCommand.Descend).Snapshot; Assert.Equal(next.Entry, snapshot.Player); Assert.True(snapshot.Tiles.Count(t => t.Explored) < snapshot.Tiles.Count / 2);
	}
	[Fact] public void NoAscentCommandExists() => Assert.Equal(new[] { "Move", "DrinkPotion", "Descend" }, Enum.GetNames<CommandKind>());
}
public sealed class VictoryTests {
	[Fact] public void TenthLevelHasGoalAndNoDownStaircase() { var level = DungeonGenerator.Generate(10, 7, GameRules.Default); Assert.Equal(TerrainType.Goal, level.At(level.Objective)); Assert.DoesNotContain(level.WalkablePositions(), p => level.At(p) == TerrainType.Staircase); }
	[Fact]
	public void GoalWinsBeforeEnemyPhase() {
		var game = new GameSession(7, GameRules.Default, Fixtures.Level("#####", "#.&.#", "#####"), [new(1, "Troll", 'T', new(3, 1), 16, 30)], health: 1, depth: 10);
		var result = game.Apply(GameCommand.Move(Direction.East)); Assert.Equal(RunStatus.Won, result.Snapshot.Status); Assert.Equal(1, result.Snapshot.Health);
	}
	[Fact]
	public void CommandsAfterVictoryDoNothing() {
		var game = new GameSession(7, GameRules.Default, Fixtures.Level("#####", "#.&.#", "#####"), depth: 10); game.Apply(GameCommand.Move(Direction.East));
		var before = System.Text.Json.JsonSerializer.Serialize(game.Snapshot()); Assert.False(game.Apply(GameCommand.Descend).TurnConsumed); Assert.False(game.Apply(GameCommand.Move(Direction.West)).Accepted); Assert.Equal(before, System.Text.Json.JsonSerializer.Serialize(game.Snapshot()));
	}
}
public sealed class CompleteRunTests {
	[Fact]
	public void EnemyFreeRunCompletesAllTenFloors() {
		var levels = Enumerable.Range(1, 10).ToDictionary(depth => depth, depth => DungeonGenerator.Generate(depth, 73, GameRules.Default));
		var game = new GameSession(73, GameRules.Default, levels[1], floorSource: depth => new(levels[depth], [], []));
		for (int depth = 1; depth <= 10; depth++) {
			Assert.Equal(depth, game.Snapshot().Depth);
			foreach (var command in Path(levels[depth], game.Snapshot().Player, levels[depth].Objective)) Assert.True(game.Apply(command).Accepted);
			if (depth < 10) Assert.True(game.Apply(GameCommand.Descend).Accepted);
		}
		Assert.Equal(RunStatus.Won, game.Snapshot().Status);
	}
	internal static IEnumerable<GameCommand> Path(DungeonLevel level, GridPosition start, GridPosition goal) {
		var queue = new Queue<GridPosition>(); queue.Enqueue(start); var parents = new Dictionary<GridPosition, (GridPosition From, Direction Direction)>(); var seen = new HashSet<GridPosition> { start };
		(Direction Direction, int X, int Y)[] deltas = [(Direction.North, 0, -1), (Direction.East, 1, 0), (Direction.South, 0, 1), (Direction.West, -1, 0)];
		while (queue.TryDequeue(out var current) && current != goal) foreach (var delta in deltas) { var next = new GridPosition(current.X + delta.X, current.Y + delta.Y); if (level.CanStep(current, next) && seen.Add(next)) { parents[next] = (current, delta.Direction); queue.Enqueue(next); } }
		Assert.Contains(goal, seen); var moves = new List<GameCommand>();
		for (var current = goal; current != start; current = parents[current].From) moves.Add(GameCommand.Move(parents[current].Direction));
		moves.Reverse(); return moves;
	}
}
