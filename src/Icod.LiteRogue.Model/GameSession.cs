using Icod.LiteRogue.Model.Presentation;
using Icod.LiteRogue.Model.Items;
using Icod.LiteRogue.Model.World;

namespace Icod.LiteRogue.Model;

public sealed partial class GameSession {
    private readonly int seed;
    private readonly GameRules rules;
    private DungeonLevel level;
    private Visibility visibility;
    private GridPosition player;
    private GameSession(int seed, GameRules rules) {
        this.seed=seed; this.rules=rules;
        level=DungeonGenerator.Generate(1,seed,rules); player=level.Entry;
        visibility=new Visibility(level.Width,level.Height); visibility.Update(level,player);
    }
    public static GameSession NewRun(int seed, GameRules rules) {
        ArgumentNullException.ThrowIfNull(rules);
        return new GameSession(seed, rules);
    }
    public GameSnapshot Snapshot() {
        var tiles=new TileSnapshot[level.Width*level.Height];
        for(int y=0;y<level.Height;y++) for(int x=0;x<level.Width;x++) {
            var p=new GridPosition(x,y); bool explored=visibility.IsExplored(p);
            tiles[y*level.Width+x]=new(p,explored?level.At(p):TerrainType.Wall,explored,visibility.IsVisible(p));
        }
        return new(seed,1,RunStatus.Playing,rules.MaximumHealth,rules.MaximumHealth,new Weapon("Knife",4),new Armor("Clothes",0),0,player,level.Width,level.Height,Array.AsReadOnly(tiles));
    }
}
