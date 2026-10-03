using Icod.LiteRogue.Model;
using Icod.LiteRogue.Terminal;
using Icod.LiteRogue.View;
namespace Icod.LiteRogue.Controller;
public sealed class GameController {
    private GameSession game;
    private readonly ITerminalInput input;
    private readonly IGameView view;
    public GameController(GameSession game,ITerminalInput input,IGameView view) { this.game=game; this.input=input; this.view=view; }
    public async Task RunAsync(CancellationToken cancellationToken=default) {
        IReadOnlyList<string> messages=Array.Empty<string>();
        while(true) {
            cancellationToken.ThrowIfCancellationRequested();
            view.Render(game.Snapshot(),messages); await view.RefreshAsync(cancellationToken);
            var current=await input.ReadAsync(cancellationToken);
            if(current.Kind==TerminalInputEventKind.End) return;
            if(current.Kind!=TerminalInputEventKind.Key) continue;
            if(current.Key.Character=='q') return;
            if(current.Key.Character=='?' || current.Key.Navigation==NavigationKey.Escape) { view.HelpVisible=current.Key.Character=='?'&&!view.HelpVisible; continue; }
            if(view.HelpVisible) continue;
            if(current.Key.Character=='r' && game.Snapshot().Status!=RunStatus.Playing) {
                game=GameSession.NewRun(System.Security.Cryptography.RandomNumberGenerator.GetInt32(int.MaxValue),GameRules.Default); messages=Array.Empty<string>(); continue;
            }
            if(KeyBindings.TryTranslate(current.Key,out var command)) messages=game.Apply(command).Messages;
        }
    }
}
