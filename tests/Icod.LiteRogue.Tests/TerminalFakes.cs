/*
	Icod.LiteRogue.Tests
	Automated test suite for Icod.LiteRogue.
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

using Icod.LiteRogue.Model.Presentation;
using Icod.LiteRogue.Terminal;
using Icod.LiteRogue.View;
namespace Icod.LiteRogue.Tests;

internal sealed class ScriptedInput : ITerminalInput {
	private readonly Queue<TerminalInputEvent> events;
	public ScriptedInput(params char[] keys) : this(keys.Select(c => new TerminalInputEvent(TerminalInputEventKind.Key, new(c))).ToArray()) { }
	public ScriptedInput(params TerminalInputEvent[] events) { this.events = new(events); }
	public ValueTask<TerminalInputEvent> ReadAsync(CancellationToken token) {
		token.ThrowIfCancellationRequested(); return ValueTask.FromResult(events.TryDequeue(out var item) ? item : new(TerminalInputEventKind.End));
	}
}
internal sealed class CapturingView : IGameView {
	public bool HelpVisible { get; set; }
	public List<GameSnapshot> Frames { get; } = [];
	public void Render(GameSnapshot snapshot, IReadOnlyList<string> messages) => Frames.Add(snapshot);
	public ValueTask RefreshAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
}
internal sealed class TestHost(ITerminalInput input) : ITerminalHost {
	public bool Disposed { get; private set; }
	public ITerminalInput Input { get; } = input;
	public IGameView View { get; } = new CapturingView();
	public ValueTask DisposeAsync() { Disposed = true; return ValueTask.CompletedTask; }
}
internal sealed class FailingInput : ITerminalInput {
	public ValueTask<TerminalInputEvent> ReadAsync(CancellationToken token) => throw new IOException("Disconnected");
}
