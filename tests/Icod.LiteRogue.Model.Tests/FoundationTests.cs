using Icod.LiteRogue.Model;
using Icod.LiteRogue.Model.Presentation;

namespace Icod.LiteRogue.Model.Tests;

public sealed class FoundationTests {
    [Fact]
    public void NewRunStartsFresh() {
        var run = GameSession.NewRun(123, GameRules.Default);
        var view = run.Snapshot();
        Assert.Equal(1, view.Depth);
        Assert.Equal(RunStatus.Playing, view.Status);
        Assert.Equal(view.MaximumHealth, view.Health);
        Assert.Equal(0, view.Potions);
        Assert.Equal(4, view.Weapon.Damage);
        Assert.Equal(0, view.Armor.Reduction);
        Assert.Equal(123, view.Seed);
    }

    [Fact]
    public void SnapshotCannotMutateModel() {
        var run = GameSession.NewRun(123, GameRules.Default);
        var view = run.Snapshot();
        Assert.Throws<NotSupportedException>(() => ((IList<TileSnapshot>)view.Tiles)[0] = default);
        Assert.Equal(view.Tiles, run.Snapshot().Tiles);
    }

    [Fact]
    public void ModelDoesNotDependOnDCurses() {
        Assert.DoesNotContain(typeof(GameSession).Assembly.GetReferencedAssemblies(), a => a.Name!.StartsWith("Icod.DCurses", StringComparison.Ordinal));
    }
}
