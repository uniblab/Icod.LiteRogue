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
using Icod.LiteRogue.View;
namespace Icod.LiteRogue.Controller;

public sealed class GameController {
	private GameSession game;
	private readonly ITerminalInput input;
	private readonly IGameView view;
	public GameController(GameSession game, ITerminalInput input, IGameView view) { this.game = game; this.input = input; this.view = view; }
	public async Task RunAsync(CancellationToken cancellationToken = default) {
		IReadOnlyList<string> messages = Array.Empty<string>();
		while (true) {
			cancellationToken.ThrowIfCancellationRequested();
			view.Render(game.Snapshot(), messages); await view.RefreshAsync(cancellationToken);
			var current = await input.ReadAsync(cancellationToken);
			if (current.Kind == TerminalInputEventKind.End) return;
			if (current.Kind != TerminalInputEventKind.Key) continue;
			if (current.Key.Character == 'q') return;
			if (current.Key.Character == '?' || current.Key.Navigation == NavigationKey.Escape) { view.HelpVisible = current.Key.Character == '?' && !view.HelpVisible; continue; }
			if (view.HelpVisible) continue;
			if (current.Key.Character == 'r' && game.Snapshot().Status != RunStatus.Playing) {
				game = GameSession.NewRun(System.Security.Cryptography.RandomNumberGenerator.GetInt32(int.MaxValue), GameRules.Default); messages = Array.Empty<string>(); continue;
			}
			if (KeyBindings.TryTranslate(current.Key, out var command)) messages = game.Apply(command).Messages;
		}
	}
}
