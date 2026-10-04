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

public enum NavigationKey { None, Up, Down, Left, Right, Home, End, PageUp, PageDown, Escape }
public readonly record struct TerminalKey(char Character = default, NavigationKey Navigation = NavigationKey.None);
public enum TerminalInputEventKind { Key, Resize, End, Ignored }
public readonly record struct TerminalInputEvent(TerminalInputEventKind Kind, TerminalKey Key = default);
public interface ITerminalInput { ValueTask<TerminalInputEvent> ReadAsync(CancellationToken cancellationToken); }
public interface ITerminalHost : IAsyncDisposable { ITerminalInput Input { get; } View.IGameView View { get; } }
