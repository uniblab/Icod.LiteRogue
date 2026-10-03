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

using Icod.LiteRogue.Terminal;
namespace Icod.LiteRogue;

public static class Application {
	public const string Usage = "Usage: literogue [--seed <integer>] [--help] [--version]";
	public static string Version => typeof(Application).Assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
		.Cast<System.Reflection.AssemblyInformationalVersionAttribute>().First().InformationalVersion.Split('+')[0];
	public static async Task<int> RunAsync(string[] args, TextWriter output, TextWriter error, Func<CancellationToken, ValueTask<ITerminalHost>> open, CancellationToken cancellationToken = default) {
		int? seed = null; bool help = false, version = false;
		for (int i = 0; i < args.Length; i++) {
			switch (args[i]) {
				case "--help": help = true; break;
				case "--version": version = true; break;
				case "--seed" when i + 1 < args.Length && int.TryParse(args[++i], out int value): seed = value; break;
				default: await error.WriteLineAsync(Usage); return 2;
			}
		}
		if (help) { await output.WriteLineAsync(Usage + "\nReach the fountain on floor 10. Move: arrows, hjklyubn, keypad. p: potion; >: descend; ?: help; q: quit."); return 0; }
		if (version) { await output.WriteLineAsync(Version); return 0; }
		try {
			await using var host = await open(cancellationToken);
			var game = Model.GameSession.NewRun(seed ?? System.Security.Cryptography.RandomNumberGenerator.GetInt32(int.MaxValue), Model.GameRules.Default);
			await new Controller.GameController(game, host.Input, host.View).RunAsync(cancellationToken);
			return 0;
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { return 0; }
		catch (Exception ex) { await error.WriteLineAsync($"Icod.LiteRogue: {ex.Message}"); return 1; }
	}
}
