namespace Icod.LiteRogue.Terminal;

// Console.OpenStandardInput emulates line input on Unix. Read the descriptor
// through FileStream so DCurses receives individual bytes in cbreak mode.
public sealed class TerminalByteInput(Stream stream) : Icod.Terminal.ITerminalInput {
	public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => stream.ReadAsync(buffer, cancellationToken);
}
internal sealed class TerminalByteOutput(Stream stream) : Icod.Terminal.ITerminalOutput {
	public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => stream.WriteAsync(buffer, cancellationToken);
	public async ValueTask FlushAsync(CancellationToken cancellationToken = default) => await stream.FlushAsync(cancellationToken);
}
