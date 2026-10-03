using Icod.DCurses;
using Icod.LiteRogue.Model.Presentation;
namespace Icod.LiteRogue.View;

public sealed class DCursesGameView(CursesSession session) : IGameView {
	public bool HelpVisible { get; set; }
	public void Render(GameSnapshot snapshot, IReadOnlyList<string> messages) {
		var screen = session.StandardScreen; screen.WrapMode = CursesWrapMode.Clip; screen.Clear();
		var frame = FrameBuilder.Build(snapshot, screen.Columns, screen.Rows, messages, HelpVisible);
		for (int row = 0; row < frame.Count; row++) { screen.Move(row, 0); screen.Write(frame[row]); }
	}
	public async ValueTask RefreshAsync(CancellationToken cancellationToken) => await session.RefreshAsync(cancellationToken);
}
