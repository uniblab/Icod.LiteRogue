using Icod.DCurses;
using Icod.LiteRogue.View;
namespace Icod.LiteRogue.Terminal;
public sealed class DCursesTerminalHost : ITerminalHost,ITerminalInput {
    private readonly CursesSession session;
    private bool disposed;
    private (int Rows,int Columns) dimensions;
    private DCursesTerminalHost(CursesSession session) { this.session=session; View=new DCursesGameView(session); dimensions=(session.StandardScreen.Rows,session.StandardScreen.Columns); }
    public static async ValueTask<ITerminalHost> OpenAsync(CancellationToken cancellationToken) => new DCursesTerminalHost(await CursesSession.OpenAsync(cancellationToken:cancellationToken));
    public ITerminalInput Input=>this;
    public IGameView View { get; }
    public async ValueTask<TerminalInputEvent> ReadAsync(CancellationToken cancellationToken) {
        while(true) {
            var current=await session.ReadEventAsync(TimeSpan.FromMilliseconds(100),cancellationToken);
            session.SynchronizeDimensions(); var fresh=(session.StandardScreen.Rows,session.StandardScreen.Columns);
            if(current.Kind==CursesEventKind.Lifecycle && current.Lifecycle?.Kind is CursesLifecycleEventKind.Interrupt or CursesLifecycleEventKind.Termination) return new(TerminalInputEventKind.End);
            if(fresh!=dimensions || current.RequiresRepaint) { dimensions=fresh; session.Invalidate(); return new(TerminalInputEventKind.Resize); }
            if(current.Kind!=CursesEventKind.Input || current.Input is not {} input) continue;
            if(input.Kind==CursesInputEventKind.EndOfInput) return new(TerminalInputEventKind.End);
            if(input.KeyPhase==CursesKeyEventPhase.Release || (input.Modifiers & (CursesKeyModifiers.Control|CursesKeyModifiers.Alt|CursesKeyModifiers.Super|CursesKeyModifiers.Meta))!=0) continue;
            if(input.Kind is not (CursesInputEventKind.Key or CursesInputEventKind.Text)) continue;
            var navigation=input.Key switch {
                CursesKey.Up or CursesKey.KeypadUp=>NavigationKey.Up,CursesKey.Down or CursesKey.KeypadDown=>NavigationKey.Down,
                CursesKey.Left or CursesKey.KeypadLeft=>NavigationKey.Left,CursesKey.Right or CursesKey.KeypadRight=>NavigationKey.Right,
                CursesKey.Home or CursesKey.KeypadHome=>NavigationKey.Home,CursesKey.End or CursesKey.KeypadEnd=>NavigationKey.End,
                CursesKey.PageUp or CursesKey.KeypadPageUp=>NavigationKey.PageUp,CursesKey.PageDown or CursesKey.KeypadPageDown=>NavigationKey.PageDown,CursesKey.Escape=>NavigationKey.Escape,_=>NavigationKey.None
            };
            char character=input.Character is {} rune && rune.Value<=char.MaxValue?(char)rune.Value:input.Key is >=CursesKey.Keypad0 and <=CursesKey.Keypad9?(char)('0'+(input.Key-CursesKey.Keypad0)):default;
            return new(TerminalInputEventKind.Key,new(character,navigation));
        }
    }
    public async ValueTask DisposeAsync() {
        if(disposed) return; disposed=true;
        await session.DisposeAsync();
    }
}
