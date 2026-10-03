using System.Text.Json;
using Icod.LiteRogue.Model;
using Icod.LiteRogue.View;
namespace Icod.LiteRogue.Tests;
public sealed class RenderingTests {
    [Fact] public void TinyTerminalRecoversWithoutChangingWorld() {
        var game=GameSession.NewRun(7,GameRules.Default); var snapshot=game.Snapshot(); var before=JsonSerializer.Serialize(snapshot);
        Assert.Contains("Resize",string.Join("\n",FrameBuilder.Build(snapshot,20,4,[],false)));
        var large=FrameBuilder.Build(snapshot,80,28,[],false);
        Assert.Contains('@',string.Join("\n",large)); Assert.Contains("HP",large[0]);
        Assert.Equal(before,JsonSerializer.Serialize(game.Snapshot()));
    }
    [Theory][InlineData(30,6)][InlineData(40,12)][InlineData(80,28)]
    public void ViewportContainsPlayerAndFitsBounds(int columns,int rows) {
        var frame=FrameBuilder.Build(GameSession.NewRun(15,GameRules.Default).Snapshot(),columns,rows,[],false);
        Assert.True(frame.Count<=rows); Assert.All(frame,line=>Assert.True(line.Length<=columns));
        Assert.Contains('@',string.Join("\n",frame));
    }
}
