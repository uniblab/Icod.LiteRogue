using Icod.LiteRogue.Model.Items;
using Icod.LiteRogue.Model.World;
namespace Icod.LiteRogue.Model.Tests;

public sealed class ItemsTests {
	internal static GameSession Game(Loot loot, int health = 30, int potions = 0) => new(7, GameRules.Default, Fixtures.Level("#####", "#...#", "#####"), health: health, loot: [loot], potions: potions);
	[Fact]
	public void WalkingCollectsLoot() {
		var game = Game(new(new(2, 1), LootKind.Weapon, "Sword", 6)); var result = game.Apply(GameCommand.Move(Direction.East));
		Assert.Equal(new Weapon("Sword", 6), result.Snapshot.Weapon); Assert.Empty(result.Snapshot.Loot!);
	}
	[Theory]
	[InlineData(3)]
	[InlineData(4)]
	public void OnlyBetterEquipmentReplacesCurrentItem(int damage) {
		var game = Game(new(new(2, 1), LootKind.Weapon, "Rusty knife", damage));
		Assert.Equal(new Weapon("Knife", 4), game.Apply(GameCommand.Move(Direction.East)).Snapshot.Weapon);
	}
	[Fact]
	public void ArmorPickupReplacesWeakerArmor() {
		var game = Game(new(new(2, 1), LootKind.Armor, "Leather", 1)); Assert.Equal(1, game.Apply(GameCommand.Move(Direction.East)).Snapshot.Armor.Reduction);
	}
	[Fact]
	public void PotionPickupIncrementsCount() {
		var game = Game(new(new(2, 1), LootKind.Potion, "Potion")); Assert.Equal(1, game.Apply(GameCommand.Move(Direction.East)).Snapshot.Potions);
	}
}
public sealed class HealingTests {
	private static GameSession Game(int health, int potions) => new(7, GameRules.Default, Fixtures.Level("#####", "#...#", "#####"), health: health, potions: potions);
	[Fact] public void PotionClampsToMaximum() { var snapshot = Game(25, 1).Apply(GameCommand.DrinkPotion).Snapshot; Assert.Equal(30, snapshot.Health); Assert.Equal(0, snapshot.Potions); }
	[Fact]
	public void DrinkingCostsOneTurn() {
		var game = new GameSession(7, GameRules.Default, Fixtures.Level("#####", "#...#", "#####"), [new(1, "Rat", 'r', new(2, 1), 8, 2)], health: 20, potions: 1);
		var result = game.Apply(GameCommand.DrinkPotion); Assert.True(result.TurnConsumed); Assert.Equal(1, result.Snapshot.Turn); Assert.Equal(28, result.Snapshot.Health);
	}
	[Fact] public void EmptyInventoryRejectsDrink() => Assert.False(Game(20, 0).Apply(GameCommand.DrinkPotion).TurnConsumed);
	[Fact] public void FullHealthRejectsDrink() { var result = Game(30, 1).Apply(GameCommand.DrinkPotion); Assert.False(result.TurnConsumed); Assert.Equal(1, result.Snapshot.Potions); }
	[Fact]
	public void InvalidDrinkDoesNotAdvanceRandomness() {
		var a = GameSession.NewRun(7, GameRules.Default); var b = GameSession.NewRun(7, GameRules.Default);
		a.Apply(GameCommand.DrinkPotion);
		foreach (var direction in Enum.GetValues<Direction>()) { a.Apply(GameCommand.Move(direction)); b.Apply(GameCommand.Move(direction)); }
		Assert.Equal(System.Text.Json.JsonSerializer.Serialize(a.Snapshot()), System.Text.Json.JsonSerializer.Serialize(b.Snapshot()));
	}
}
