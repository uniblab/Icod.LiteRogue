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

using Icod.DCurses;
using Icod.LiteRogue.View;
namespace Icod.LiteRogue.Terminal;

public sealed class DCursesTerminalHost : ITerminalHost, ITerminalInput {
	private readonly CursesSession session;
	private readonly Stream? ownedInput;
	private bool disposed;
	private (int Rows, int Columns) dimensions;
	private DCursesTerminalHost(CursesSession session, Stream? ownedInput = null) { this.session = session; this.ownedInput = ownedInput; View = new DCursesGameView(session); dimensions = (session.StandardScreen.Rows, session.StandardScreen.Columns); }
	public static async ValueTask<ITerminalHost> OpenAsync(CancellationToken cancellationToken) {
		if (OperatingSystem.IsWindows()) return new DCursesTerminalHost(await CursesSession.OpenAsync(cancellationToken: cancellationToken));
		var input = new FileStream("/dev/stdin", FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 1, useAsync: false);
		Icod.Terminal.TerminalSession? terminal = null;
		try {
			terminal = await Icod.Terminal.TerminalSession.OpenAsync(Icod.Terminal.SystemTerminalControlProvider.Instance, Icod.Terminal.TerminalEndpoint.StandardInput, Icod.Terminal.TerminalEndpoint.StandardOutput,
				new TerminalByteInput(input), new TerminalByteOutput(Console.OpenStandardOutput()), new Icod.Terminal.TerminalSessionOptions { InputMode = Icod.Terminal.TerminalInputMode.CBreak, EchoInput = false, RequireInteractiveOutput = true }, cancellationToken);
			return new DCursesTerminalHost(await CursesSession.OpenAsync(terminal, cancellationToken: cancellationToken), input);
		}
		catch {
			try { if (terminal is not null) await terminal.DisposeAsync(); } finally { input.Dispose(); }
			throw;
		}
	}
	public ITerminalInput Input => this;
	public IGameView View { get; }
	public async ValueTask<TerminalInputEvent> ReadAsync(CancellationToken cancellationToken) {
		while (true) {
			var current = await session.ReadEventAsync(TimeSpan.FromMilliseconds(100), cancellationToken);
			session.SynchronizeDimensions(); var fresh = (session.StandardScreen.Rows, session.StandardScreen.Columns);
			if (current.Kind == CursesEventKind.Lifecycle && current.Lifecycle?.Kind is CursesLifecycleEventKind.Interrupt or CursesLifecycleEventKind.Termination) return new(TerminalInputEventKind.End);
			if (fresh != dimensions || current.RequiresRepaint) { dimensions = fresh; session.Invalidate(); return new(TerminalInputEventKind.Resize); }
			if (current.Kind != CursesEventKind.Input || current.Input is not { } input) continue;
			if (input.Kind == CursesInputEventKind.EndOfInput) return new(TerminalInputEventKind.End);
			if (input.KeyPhase == CursesKeyEventPhase.Release || (input.Modifiers & (CursesKeyModifiers.Control | CursesKeyModifiers.Alt | CursesKeyModifiers.Super | CursesKeyModifiers.Meta)) != 0) continue;
			if (input.Kind is not (CursesInputEventKind.Key or CursesInputEventKind.Text)) continue;
			var navigation = input.Key switch {
				CursesKey.Up or CursesKey.KeypadUp => NavigationKey.Up,
				CursesKey.Down or CursesKey.KeypadDown => NavigationKey.Down,
				CursesKey.Left or CursesKey.KeypadLeft => NavigationKey.Left,
				CursesKey.Right or CursesKey.KeypadRight => NavigationKey.Right,
				CursesKey.Home or CursesKey.KeypadHome => NavigationKey.Home,
				CursesKey.End or CursesKey.KeypadEnd => NavigationKey.End,
				CursesKey.PageUp or CursesKey.KeypadPageUp => NavigationKey.PageUp,
				CursesKey.PageDown or CursesKey.KeypadPageDown => NavigationKey.PageDown,
				CursesKey.Escape => NavigationKey.Escape,
				_ => NavigationKey.None
			};
			char character = input.Character is { } rune && rune.Value <= char.MaxValue ? (char)rune.Value : input.Key is >= CursesKey.Keypad0 and <= CursesKey.Keypad9 ? (char)('0' + (input.Key - CursesKey.Keypad0)) : default;
			return new(TerminalInputEventKind.Key, new(character, navigation));
		}
	}
	public async ValueTask DisposeAsync() {
		if (disposed) return; disposed = true;
		try { await session.DisposeAsync(); } finally { ownedInput?.Dispose(); }
	}
}
