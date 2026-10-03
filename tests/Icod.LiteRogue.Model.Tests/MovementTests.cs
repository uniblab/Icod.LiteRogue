using Icod.LiteRogue.Model.World;
namespace Icod.LiteRogue.Model.Tests;
public sealed class MovementTests {
    [Theory]
    [InlineData(Direction.North,3,2)][InlineData(Direction.NorthEast,4,2)][InlineData(Direction.East,4,3)][InlineData(Direction.SouthEast,4,4)]
    [InlineData(Direction.South,3,4)][InlineData(Direction.SouthWest,2,4)][InlineData(Direction.West,2,3)][InlineData(Direction.NorthWest,2,2)]
    public void MovesInEightDirections(Direction direction,int x,int y) {
        var game=Fixtures.Session(["#######","#.....#","#.....#","#.....#","#.....#","#.....#","#######"],new(3,3));
        var result=game.Apply(GameCommand.Move(direction));
        Assert.True(result.TurnConsumed); Assert.Equal(new GridPosition(x,y),result.Snapshot.Player); Assert.Equal(1,result.Snapshot.Turn);
    }
    [Fact] public void ClosedCornerBlocksMove() {
        var game=Fixtures.Session(["#####","#.#.#","##..#","#...#","#####"]);
        Assert.False(game.Apply(GameCommand.Move(Direction.SouthEast)).Accepted);
        Assert.Equal(new GridPosition(1,1),game.Snapshot().Player);
    }
    [Fact] public void OneOpenSideAllowsDiagonal() {
        var game=Fixtures.Session(["#####","#...#","##..#","#...#","#####"]);
        Assert.Equal(new GridPosition(2,2),game.Apply(GameCommand.Move(Direction.SouthEast)).Snapshot.Player);
    }
    [Fact] public void InvalidMoveDoesNotConsumeTurn() {
        var game=Fixtures.Session(["###","#.#","###"]);
        foreach(var direction in Enum.GetValues<Direction>()) Assert.False(game.Apply(GameCommand.Move(direction)).TurnConsumed);
        Assert.Equal(0,game.Snapshot().Turn);
    }
    [Fact] public void MapEdgesBlockMovement() {
        var game=Fixtures.Session(["..",".."],new(0,0));
        Assert.False(game.Apply(GameCommand.Move(Direction.NorthWest)).Accepted);
        Assert.False(game.Apply(GameCommand.Move(Direction.West)).Accepted);
    }
}
