using Icod.LiteRogue.Model.Actors;
using Icod.LiteRogue.Model.Items;
namespace Icod.LiteRogue.Model.World;
internal sealed record LevelContent(DungeonLevel Level,IReadOnlyList<Enemy> Enemies,IReadOnlyList<Loot> Loot);
