using System.Text.Json;
using Icod.LiteRogue.Controller;
using Icod.LiteRogue.Model;
using Icod.LiteRogue.Terminal;
namespace Icod.LiteRogue.Tests;

public sealed class ControllerTests {
	[Theory]
	[InlineData('h', Direction.West)]
	[InlineData('j', Direction.South)]
	[InlineData('k', Direction.North)]
	[InlineData('l', Direction.East)]
	[InlineData('y', Direction.NorthWest)]
	[InlineData('u', Direction.NorthEast)]
	[InlineData('b', Direction.SouthWest)]
	[InlineData('n', Direction.SouthEast)]
	[InlineData('4', Direction.West)]
	[InlineData('2', Direction.South)]
	[InlineData('8', Direction.North)]
	[InlineData('6', Direction.East)]
	[InlineData('7', Direction.NorthWest)]
	[InlineData('9', Direction.NorthEast)]
	[InlineData('1', Direction.SouthWest)]
	[InlineData('3', Direction.SouthEast)]
	public void KeyMappingMatchesDirections(char key, Direction direction) {
		Assert.True(KeyBindings.TryTranslate(new(key), out var command));
		Assert.Equal(GameCommand.Move(direction), command);
	}
	[Fact]
	public async Task HelpDoesNotAdvanceTurn() {
		var game = GameSession.NewRun(7, GameRules.Default); var before = JsonSerializer.Serialize(game.Snapshot());
		await new GameController(game, new ScriptedInput('?', '?', 'q'), new CapturingView()).RunAsync();
		Assert.Equal(before, JsonSerializer.Serialize(game.Snapshot()));
	}
	[Fact]
	public async Task ResizeDoesNotChangeSnapshot() {
		var game = GameSession.NewRun(7, GameRules.Default); var before = JsonSerializer.Serialize(game.Snapshot());
		var input = new ScriptedInput(new TerminalInputEvent(TerminalInputEventKind.Resize), new TerminalInputEvent(TerminalInputEventKind.Key, new('q')));
		await new GameController(game, input, new CapturingView()).RunAsync();
		Assert.Equal(before, JsonSerializer.Serialize(game.Snapshot()));
	}
	[Theory]
	[InlineData("--help", 0)]
	[InlineData("--version", 0)]
	[InlineData("--unknown", 2)]
	[InlineData("--seed", 2)]
	public async Task NoninteractiveArgumentsDoNotOpenTerminal(string argument, int expected) {
		var output = new StringWriter(); var error = new StringWriter();
		int result = await Application.RunAsync([argument], output, error, _ => throw new InvalidOperationException("Terminal must stay closed"));
		Assert.Equal(expected, result);
		Assert.NotEmpty(expected == 0 ? output.ToString() : error.ToString());
	}
}
