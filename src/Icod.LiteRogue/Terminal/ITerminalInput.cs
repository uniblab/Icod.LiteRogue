namespace Icod.LiteRogue.Terminal;

public enum NavigationKey { None, Up, Down, Left, Right, Home, End, PageUp, PageDown, Escape }
public readonly record struct TerminalKey(char Character = default, NavigationKey Navigation = NavigationKey.None);
public enum TerminalInputEventKind { Key, Resize, End, Ignored }
public readonly record struct TerminalInputEvent(TerminalInputEventKind Kind, TerminalKey Key = default);
public interface ITerminalInput { ValueTask<TerminalInputEvent> ReadAsync(CancellationToken cancellationToken); }
public interface ITerminalHost : IAsyncDisposable { ITerminalInput Input { get; } View.IGameView View { get; } }
