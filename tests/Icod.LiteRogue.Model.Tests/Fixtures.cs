using Icod.LiteRogue.Model.World;
namespace Icod.LiteRogue.Model.Tests;

internal static class Fixtures {
    internal static GameSession Session(string[] rows, GridPosition? entry=null) {
        var level=Level(rows);
        level=new DungeonLevel(level.Width,level.Height,rows.SelectMany(row=>row.Select(c=>c=='#'?TerrainType.Wall:TerrainType.Floor)).ToArray(),[],entry??new(1,1),level.Objective);
        return new GameSession(7,GameRules.Default,level);
    }
    internal static DungeonLevel Level(params string[] rows) {
        int width=rows[0].Length;
        var tiles = rows.SelectMany(row => row.Select(c => c switch { '#' => TerrainType.Wall, '>' => TerrainType.Staircase, '&' => TerrainType.Goal, _ => TerrainType.Floor })).ToArray();
        return new DungeonLevel(width, rows.Length, tiles, [], new(1,1), new(width-2,rows.Length-2));
    }
}
