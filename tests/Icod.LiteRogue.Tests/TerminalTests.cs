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

using Icod.LiteRogue.Terminal;
namespace Icod.LiteRogue.Tests;

public sealed class TerminalTests {
	[Fact]
	public async Task QuitDisposesHost() {
		var host = new TestHost(new ScriptedInput('q'));
		int exit = await Application.RunAsync(["--seed", "7"], new StringWriter(), new StringWriter(), _ => ValueTask.FromResult<ITerminalHost>(host));
		Assert.Equal(0, exit); Assert.True(host.Disposed);
	}
	[Fact]
	public async Task InputFailureDisposesHost() {
		var host = new TestHost(new FailingInput()); var error = new StringWriter();
		int exit = await Application.RunAsync(["--seed", "7"], new StringWriter(), error, _ => ValueTask.FromResult<ITerminalHost>(host));
		Assert.Equal(1, exit); Assert.True(host.Disposed); Assert.Contains("Disconnected", error.ToString());
	}
	[Fact]
	public async Task CancellationDisposesHost() {
		using var cancel = new CancellationTokenSource(); cancel.Cancel();
		var host = new TestHost(new ScriptedInput('q'));
		int exit = await Application.RunAsync(["--seed", "7"], new StringWriter(), new StringWriter(), _ => ValueTask.FromResult<ITerminalHost>(host), cancel.Token);
		Assert.Equal(0, exit); Assert.True(host.Disposed);
	}
}
