namespace Icod.LiteRogue.Model.World;

public sealed record Room(int Sector, int X, int Y, int Width, int Height) {
    public GridPosition Center => new(X+Width/2,Y+Height/2);
    public bool Contains(GridPosition p) => p.X>=X && p.Y>=Y && p.X<X+Width && p.Y<Y+Height;
    public bool ContainsInterior(GridPosition p) => p.X>X && p.Y>Y && p.X<X+Width-1 && p.Y<Y+Height-1;
}
public sealed class DungeonLevel {
    private readonly TerrainType[] tiles;
    internal DungeonLevel(int width,int height,TerrainType[] tiles,IReadOnlyList<Room> rooms,GridPosition entry,GridPosition objective) {
        Width=width; Height=height; this.tiles=(TerrainType[])tiles.Clone(); Rooms=Array.AsReadOnly(rooms.ToArray()); Entry=entry; Objective=objective;
    }
    public int Width { get; }
    public int Height { get; }
    public IReadOnlyList<Room> Rooms { get; }
    public GridPosition Entry { get; }
    public GridPosition Objective { get; }
    public bool InBounds(GridPosition p) => p.X>=0 && p.Y>=0 && p.X<Width && p.Y<Height;
    public TerrainType At(GridPosition p) => InBounds(p)?tiles[p.Y*Width+p.X]:TerrainType.Wall;
    public bool IsWalkable(GridPosition p) => InBounds(p) && At(p)!=TerrainType.Wall;
    public bool CanStep(GridPosition from,GridPosition to) {
        if(!IsWalkable(from) || !IsWalkable(to)) return false;
        int dx=to.X-from.X,dy=to.Y-from.Y;
        if(Math.Abs(dx)>1 || Math.Abs(dy)>1 || (dx==0 && dy==0)) return false;
        return dx==0 || dy==0 || IsWalkable(new(from.X+dx,from.Y)) || IsWalkable(new(from.X,from.Y+dy));
    }
    public IEnumerable<GridPosition> WalkablePositions() {
        for(int y=0;y<Height;y++) for(int x=0;x<Width;x++) { var p=new GridPosition(x,y); if(IsWalkable(p)) yield return p; }
    }
}
