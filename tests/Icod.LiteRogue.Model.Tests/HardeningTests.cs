using System.Text.Json;
namespace Icod.LiteRogue.Model.Tests;

public sealed class HardeningTests {
	[Fact]
	public void SameSeedAndCommandsRepeatRun() {
		var a = GameSession.NewRun(7, GameRules.Default); var b = GameSession.NewRun(7, GameRules.Default);
		foreach (var direction in Enumerable.Range(0, 100).Select(i => (Direction)(i % 8))) { a.Apply(GameCommand.Move(direction)); b.Apply(GameCommand.Move(direction)); }
		Assert.Equal(JsonSerializer.Serialize(a.Snapshot()), JsonSerializer.Serialize(b.Snapshot()));
	}
	[Fact]
	public void RedrawDoesNotAdvanceRandomness() {
		var a = GameSession.NewRun(7, GameRules.Default); var b = GameSession.NewRun(7, GameRules.Default);
		for (int i = 0; i < 50; i++) a.Snapshot();
		foreach (var direction in Enum.GetValues<Direction>()) { a.Apply(GameCommand.Move(direction)); b.Apply(GameCommand.Move(direction)); }
		Assert.Equal(JsonSerializer.Serialize(a.Snapshot()), JsonSerializer.Serialize(b.Snapshot()));
	}
	[Fact]
	public void SnapshotsCannotLeakHiddenEntities() {
		var a = GameSession.NewRun(7, GameRules.Default).Snapshot();
		Assert.All(a.Enemies!, e => Assert.True(a.Tiles.Single(t => t.Position == e.Position).Visible));
		Assert.All(a.Loot!, l => Assert.True(a.Tiles.Single(t => t.Position == l.Position).Visible));
		Assert.Throws<NotSupportedException>(() => ((IList<Actors.Enemy>)a.Enemies!).Clear());
		Assert.Throws<NotSupportedException>(() => ((IList<Items.Loot>)a.Loot!).Clear());
	}
	[Theory]
	[InlineData(0, 10)]
	[InlineData(-1, 10)]
	[InlineData(30, 0)]
	[InlineData(30, -1)]
	public void NonpositiveHealthRulesAreRejected(int health, int healing) => Assert.Throws<ArgumentOutOfRangeException>(() => GameSession.NewRun(7, GameRules.Default with { MaximumHealth = health, PotionHealing = healing }));
}
