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
    private long turn;
    internal GameSession(int seed, GameRules rules, DungeonLevel level) {
        this.seed=seed; this.rules=rules;
        this.level=level; player=level.Entry;
        visibility=new Visibility(level.Width,level.Height); visibility.Update(level,player);
    }
    public static GameSession NewRun(int seed, GameRules rules) {
        ArgumentNullException.ThrowIfNull(rules);
        return new GameSession(seed, rules, DungeonGenerator.Generate(1,seed,rules));
    }
    public ActionResult Apply(GameCommand command) {
        if(command.Kind!=CommandKind.Move || !Enum.IsDefined(command.Direction)) return Reject("That action is unavailable.");
        var delta=command.Direction switch {
            Direction.North=>new GridPosition(0,-1),Direction.NorthEast=>new(1,-1),Direction.East=>new(1,0),Direction.SouthEast=>new(1,1),
            Direction.South=>new(0,1),Direction.SouthWest=>new(-1,1),Direction.West=>new(-1,0),Direction.NorthWest=>new(-1,-1),
            _=>default
        };
        var target=new GridPosition(player.X+delta.X,player.Y+delta.Y);
        if(!level.CanStep(player,target)) return Reject("The way is blocked.");
        player=target; turn++; visibility.Update(level,player);
        return new(true,true,Array.Empty<string>(),Snapshot());
    }
    private ActionResult Reject(string message) => new(false,false,Array.AsReadOnly(new[]{message}),Snapshot());
    public GameSnapshot Snapshot() {
        var tiles=new TileSnapshot[level.Width*level.Height];
        for(int y=0;y<level.Height;y++) for(int x=0;x<level.Width;x++) {
            var p=new GridPosition(x,y); bool explored=visibility.IsExplored(p);
            tiles[y*level.Width+x]=new(p,explored?level.At(p):TerrainType.Wall,explored,visibility.IsVisible(p));
        }
        return new(seed,1,RunStatus.Playing,rules.MaximumHealth,rules.MaximumHealth,new Weapon("Knife",4),new Armor("Clothes",0),0,player,level.Width,level.Height,Array.AsReadOnly(tiles),turn);
    }
}
