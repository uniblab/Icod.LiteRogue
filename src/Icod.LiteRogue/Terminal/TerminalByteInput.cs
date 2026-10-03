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

namespace Icod.LiteRogue.Terminal;

// Console.OpenStandardInput emulates line input on Unix. Read the descriptor
// through FileStream so DCurses receives individual bytes in cbreak mode.
public sealed class TerminalByteInput(Stream stream) : Icod.Terminal.ITerminalInput {
	public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => stream.ReadAsync(buffer, cancellationToken);
}
internal sealed class TerminalByteOutput(Stream stream) : Icod.Terminal.ITerminalOutput {
	public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => stream.WriteAsync(buffer, cancellationToken);
	public async ValueTask FlushAsync(CancellationToken cancellationToken = default) => await stream.FlushAsync(cancellationToken);
}
