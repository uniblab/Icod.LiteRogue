namespace Icod.LiteRogue.Model.Presentation;

public sealed record ActionResult(bool Accepted, bool TurnConsumed, IReadOnlyList<string> Messages, GameSnapshot Snapshot);
