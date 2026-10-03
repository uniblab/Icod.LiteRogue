using Icod.LiteRogue.Model.Presentation;
using Icod.LiteRogue.Terminal;
using Icod.LiteRogue.View;
namespace Icod.LiteRogue.Tests;
internal sealed class ScriptedInput : ITerminalInput {
    private readonly Queue<TerminalInputEvent> events;
    public ScriptedInput(params char[] keys):this(keys.Select(c=>new TerminalInputEvent(TerminalInputEventKind.Key,new(c))).ToArray()) { }
    public ScriptedInput(params TerminalInputEvent[] events) { this.events=new(events); }
    public ValueTask<TerminalInputEvent> ReadAsync(CancellationToken token) {
        token.ThrowIfCancellationRequested(); return ValueTask.FromResult(events.TryDequeue(out var item)?item:new(TerminalInputEventKind.End));
    }
}
internal sealed class CapturingView : IGameView {
    public bool HelpVisible { get; set; }
    public List<GameSnapshot> Frames { get; }=[];
    public void Render(GameSnapshot snapshot,IReadOnlyList<string> messages) => Frames.Add(snapshot);
    public ValueTask RefreshAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
}
internal sealed class TestHost(ITerminalInput input) : ITerminalHost {
    public bool Disposed { get; private set; }
    public ITerminalInput Input { get; }=input;
    public IGameView View { get; }=new CapturingView();
    public ValueTask DisposeAsync() { Disposed=true; return ValueTask.CompletedTask; }
}
internal sealed class FailingInput : ITerminalInput {
    public ValueTask<TerminalInputEvent> ReadAsync(CancellationToken token) => throw new IOException("Disconnected");
}
