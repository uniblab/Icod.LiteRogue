/*
	Icod.LiteRogue.Model.Tests
	Automated test suite for the Icod.LiteRogue domain model.
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
namespace Icod.LiteRogue.Model.Tests;

public sealed class HardeningTests {
	[Fact]
	public void SameSeedAndCommandsRepeatRun() {
		var a = GameSession.NewRun(7, GameRules.Default); var b = GameSession.NewRun(7, GameRules.Default);
		foreach (var direction in Enumerable.Range(0, 100).Select(i => (Direction)(i % 8))) { a.Apply(GameCommand.Move(direction)); b.Apply(GameCommand.Move(direction)); }
		Assert.Equal(JsonSerializer.Serialize(a.Snapshot()), JsonSerializer.Serialize(b.Snapshot()));
	}
	[Fact]
	public void RedrawDoesNotAdvanceRandomness() {
		var a = GameSession.NewRun(7, GameRules.Default); var b = GameSession.NewRun(7, GameRules.Default);
		for (int i = 0; i < 50; i++) a.Snapshot();
		foreach (var direction in Enum.GetValues<Direction>()) { a.Apply(GameCommand.Move(direction)); b.Apply(GameCommand.Move(direction)); }
		Assert.Equal(JsonSerializer.Serialize(a.Snapshot()), JsonSerializer.Serialize(b.Snapshot()));
	}
	[Fact]
	public void SnapshotsCannotLeakHiddenEntities() {
		var a = GameSession.NewRun(7, GameRules.Default).Snapshot();
		Assert.All(a.Enemies!, e => Assert.True(a.Tiles.Single(t => t.Position == e.Position).Visible));
		Assert.All(a.Loot!, l => Assert.True(a.Tiles.Single(t => t.Position == l.Position).Visible));
		Assert.Throws<NotSupportedException>(() => ((IList<Actors.Enemy>)a.Enemies!).Clear());
		Assert.Throws<NotSupportedException>(() => ((IList<Items.Loot>)a.Loot!).Clear());
	}
	[Theory]
	[InlineData(0, 10)]
	[InlineData(-1, 10)]
	[InlineData(30, 0)]
	[InlineData(30, -1)]
	public void NonpositiveHealthRulesAreRejected(int health, int healing) => Assert.Throws<ArgumentOutOfRangeException>(() => GameSession.NewRun(7, GameRules.Default with { MaximumHealth = health, PotionHealing = healing }));
}
