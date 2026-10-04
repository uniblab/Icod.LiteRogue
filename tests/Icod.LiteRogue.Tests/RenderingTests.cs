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

using System.Text.Json;
using Icod.LiteRogue.Model;
using Icod.LiteRogue.View;
namespace Icod.LiteRogue.Tests;

public sealed class RenderingTests {
	[Fact]
	public void TinyTerminalRecoversWithoutChangingWorld() {
		var game = GameSession.NewRun(7, GameRules.Default); var snapshot = game.Snapshot(); var before = JsonSerializer.Serialize(snapshot);
		Assert.Contains("Resize", string.Join("\n", FrameBuilder.Build(snapshot, 20, 4, [], false)));
		var large = FrameBuilder.Build(snapshot, 80, 28, [], false);
		Assert.Contains('@', string.Join("\n", large)); Assert.Contains("HP", large[0]);
		Assert.Equal(before, JsonSerializer.Serialize(game.Snapshot()));
	}
	[Theory]
	[InlineData(30, 6)]
	[InlineData(40, 12)]
	[InlineData(80, 28)]
	public void ViewportContainsPlayerAndFitsBounds(int columns, int rows) {
		var frame = FrameBuilder.Build(GameSession.NewRun(15, GameRules.Default).Snapshot(), columns, rows, [], false);
		Assert.True(frame.Count <= rows); Assert.All(frame, line => Assert.True(line.Length <= columns));
		Assert.Contains('@', string.Join("\n", frame));
	}
}
