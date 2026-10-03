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

using Icod.LiteRogue.Model.Actors;
namespace Icod.LiteRogue.Model.Tests;

public sealed class CombatTests {
	[Fact]
	public void BumpAttackKeepsPlayerPosition() {
		var game = Fixtures.Session(["#####", "#...#", "#...#", "#####"], enemies: [new(1, "Rat", 'r', new(2, 1), 8, 2)]);
		var result = game.Apply(GameCommand.Move(Direction.East));
		Assert.True(result.Accepted); Assert.Equal(new(1, 1), result.Snapshot.Player);
		Assert.Equal(4, Assert.Single(result.Snapshot.Enemies!).Health); Assert.Equal(28, result.Snapshot.Health);
	}
	[Theory]
	[InlineData(4, 0, 4)]
	[InlineData(4, 2, 2)]
	[InlineData(2, 5, 1)]
	public void ArmorMitigatesWithMinimumOneDamage(int attack, int armor, int expected) => Assert.Equal(expected, Combat.Damage(attack, armor));
	[Fact]
	public void ClosedCornerBlocksAttack() {
		var game = Fixtures.Session(["#####", "#.#.#", "##..#", "#####"], enemies: [new(1, "Rat", 'r', new(2, 2), 8, 2)]);
		Assert.False(game.Apply(GameCommand.Move(Direction.SouthEast)).Accepted); Assert.Equal(30, game.Snapshot().Health);
	}
	[Fact]
	public void HiddenEntitiesAreAbsent() {
		var game = Fixtures.Session(["#######", "#.....#", "#######"], enemies: [new(1, "Rat", 'r', new(5, 1), 8, 2)]);
		Assert.Empty(game.Snapshot().Enemies!);
	}
	[Fact]
	public void EnemyUsesSameMovementRules() {
		var game = Fixtures.Session(["#####", "#.#.#", "##..#", "#####"], entry: new(2, 2), enemies: [new(1, "Rat", 'r', new(1, 1), 8, 2)]);
		game.Apply(GameCommand.Move(Direction.East)); Assert.Equal(30, game.Snapshot().Health);
	}
}
public sealed class TurnsTests {
	[Fact]
	public void EachEnemyActsAtMostOnce() {
		var game = Fixtures.Session(["#####", "#...#", "#...#", "#####"], enemies: [new(2, "Rat", 'r', new(2, 1), 20, 2), new(1, "Rat", 'r', new(1, 2), 20, 3)]);
		game.Apply(GameCommand.Move(Direction.East)); Assert.Equal(25, game.Snapshot().Health);
	}
	[Fact]
	public void EnemiesNeverOverlap() {
		var game = Fixtures.Session(["#######", "#.....#", "#.....#", "#######"], entry: new(3, 1), enemies: [new(1, "Rat", 'r', new(2, 2), 20, 1), new(2, "Rat", 'r', new(4, 2), 20, 1)]);
		for (int i = 0; i < 3; i++) { var snapshot = game.Apply(GameCommand.Move(Direction.South)).Snapshot; Assert.Equal(snapshot.Enemies!.Count, snapshot.Enemies.Select(e => e.Position).Distinct().Count()); }
	}
	[Fact]
	public void RejectedCommandSkipsEnemyPhase() {
		var game = Fixtures.Session(["#####", "#...#", "#####"], enemies: [new(1, "Rat", 'r', new(2, 1), 8, 2)]);
		Assert.False(game.Apply(GameCommand.Move(Direction.North)).TurnConsumed); Assert.Equal(30, game.Snapshot().Health);
	}
}
public sealed class DeathTests {
	[Fact]
	public void DeathStopsRemainingEnemyActions() {
		var game = Fixtures.Session(["#####", "#...#", "#...#", "#####"], enemies: [new(1, "Rat", 'r', new(2, 1), 20, 30), new(2, "Rat", 'r', new(1, 2), 20, 30)], health: 1);
		var result = game.Apply(GameCommand.Move(Direction.East)); Assert.Equal(RunStatus.Dead, result.Snapshot.Status); Assert.Equal(0, result.Snapshot.Health);
		Assert.Single(result.Messages, m => m.Contains("hits you"));
	}
	[Fact]
	public void CommandsAfterDeathDoNothing() {
		var game = Fixtures.Session(["#####", "#...#", "#####"], enemies: [new(1, "Rat", 'r', new(2, 1), 20, 30)], health: 1);
		game.Apply(GameCommand.Move(Direction.East)); var before = System.Text.Json.JsonSerializer.Serialize(game.Snapshot());
		Assert.False(game.Apply(GameCommand.Move(Direction.East)).Accepted); Assert.Equal(before, System.Text.Json.JsonSerializer.Serialize(game.Snapshot()));
	}
}
