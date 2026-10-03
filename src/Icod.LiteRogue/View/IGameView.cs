using Icod.LiteRogue.Model.Presentation;
namespace Icod.LiteRogue.View;
public interface IGameView {
    bool HelpVisible { get; set; }
    void Render(GameSnapshot snapshot,IReadOnlyList<string> messages);
    ValueTask RefreshAsync(CancellationToken cancellationToken);
}
