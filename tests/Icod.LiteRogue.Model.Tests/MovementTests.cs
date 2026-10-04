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

using Icod.LiteRogue.Model.World;
namespace Icod.LiteRogue.Model.Tests;

public sealed class MovementTests {
	[Theory]
	[InlineData(Direction.North, 3, 2)]
	[InlineData(Direction.NorthEast, 4, 2)]
	[InlineData(Direction.East, 4, 3)]
	[InlineData(Direction.SouthEast, 4, 4)]
	[InlineData(Direction.South, 3, 4)]
	[InlineData(Direction.SouthWest, 2, 4)]
	[InlineData(Direction.West, 2, 3)]
	[InlineData(Direction.NorthWest, 2, 2)]
	public void MovesInEightDirections(Direction direction, int x, int y) {
		var game = Fixtures.Session(["#######", "#.....#", "#.....#", "#.....#", "#.....#", "#.....#", "#######"], new(3, 3));
		var result = game.Apply(GameCommand.Move(direction));
		Assert.True(result.TurnConsumed); Assert.Equal(new GridPosition(x, y), result.Snapshot.Player); Assert.Equal(1, result.Snapshot.Turn);
	}
	[Fact]
	public void ClosedCornerBlocksMove() {
		var game = Fixtures.Session(["#####", "#.#.#", "##..#", "#...#", "#####"]);
		Assert.False(game.Apply(GameCommand.Move(Direction.SouthEast)).Accepted);
		Assert.Equal(new GridPosition(1, 1), game.Snapshot().Player);
	}
	[Fact]
	public void OneOpenSideAllowsDiagonal() {
		var game = Fixtures.Session(["#####", "#...#", "##..#", "#...#", "#####"]);
		Assert.Equal(new GridPosition(2, 2), game.Apply(GameCommand.Move(Direction.SouthEast)).Snapshot.Player);
	}
	[Fact]
	public void InvalidMoveDoesNotConsumeTurn() {
		var game = Fixtures.Session(["###", "#.#", "###"]);
		foreach (var direction in Enum.GetValues<Direction>()) Assert.False(game.Apply(GameCommand.Move(direction)).TurnConsumed);
		Assert.Equal(0, game.Snapshot().Turn);
	}
	[Fact]
	public void MapEdgesBlockMovement() {
		var game = Fixtures.Session(["..", ".."], new(0, 0));
		Assert.False(game.Apply(GameCommand.Move(Direction.NorthWest)).Accepted);
		Assert.False(game.Apply(GameCommand.Move(Direction.West)).Accepted);
	}
}
