using Icod.LiteRogue.Model.Presentation;
using Icod.LiteRogue.Model.Items;
using Icod.LiteRogue.Model.World;
using Icod.LiteRogue.Model.Actors;

namespace Icod.LiteRogue.Model;

public sealed partial class GameSession {
	private readonly int seed;
	private readonly GameRules rules;
	private DungeonLevel level;
	private Visibility visibility;
	private GridPosition player;
	private long turn;
	private readonly Random random;
	private List<Enemy> enemies;
	private int health;
	private Weapon weapon;
	private Armor armor;
	private int potions;
	private List<Loot> loot;
	private int depth;
	private readonly Func<int, LevelContent> floorSource;
	private RunStatus status = RunStatus.Playing;
	internal GameSession(int seed, GameRules rules, DungeonLevel level, IEnumerable<Actors.Enemy>? enemies = null, int? health = null, IEnumerable<Loot>? loot = null, int potions = 0, Weapon? weapon = null, Armor? armor = null, int depth = 1, Func<int, LevelContent>? floorSource = null) {
		this.seed = seed; this.rules = rules;
		this.level = level; player = level.Entry;
		this.enemies = (enemies ?? []).OrderBy(e => e.Id).ToList(); this.health = health ?? rules.MaximumHealth; random = new Random(unchecked(seed * 31 + 17));
		this.weapon = weapon ?? new("Knife", 4); this.armor = armor ?? new("Clothes", 0); this.potions = potions; this.loot = (loot ?? []).ToList();
		this.depth = depth; this.floorSource = floorSource ?? (next => CreateLevel(next, seed, rules));
		visibility = new Visibility(level.Width, level.Height); visibility.Update(level, player);
	}
	public static GameSession NewRun(int seed, GameRules rules) {
		ArgumentNullException.ThrowIfNull(rules);
		if (rules.MaximumHealth <= 0 || rules.PotionHealing <= 0) throw new ArgumentOutOfRangeException(nameof(rules), "Health and potion healing must be positive.");
		var level = DungeonGenerator.Generate(1, seed, rules);
		var enemies = LevelPopulation.Enemies(level, 1, seed);
		return new GameSession(seed, rules, level, enemies, loot: LevelPopulation.Loot(level, 1, seed, enemies));
	}
	private static LevelContent CreateLevel(int depth, int seed, GameRules rules) {
		var level = DungeonGenerator.Generate(depth, seed, rules); var enemies = LevelPopulation.Enemies(level, depth, seed);
		return new(level, enemies, LevelPopulation.Loot(level, depth, seed, enemies));
	}
	public ActionResult Apply(GameCommand command) {
		if (status != RunStatus.Playing) return Reject("This run is over.");
		if (command.Kind == CommandKind.Descend) {
			if (depth >= 10 || level.At(player) != TerrainType.Staircase) return Reject("Stand on a staircase to descend.");
			var content = floorSource(depth + 1); depth++; level = content.Level; enemies = content.Enemies.OrderBy(e => e.Id).ToList(); loot = content.Loot.ToList();
			player = level.Entry; visibility = new Visibility(level.Width, level.Height); visibility.Update(level, player); turn++;
			return new(true, true, Array.AsReadOnly(new[] { $"You descend to floor {depth}." }), Snapshot());
		}
		if (command.Kind == CommandKind.DrinkPotion) {
			if (potions == 0) return Reject("You have no healing potions.");
			if (health == rules.MaximumHealth) return Reject("You are already at full health.");
			potions--; health = Math.Min(rules.MaximumHealth, health + rules.PotionHealing); turn++;
			var healingMessages = new List<string> { "You drink a healing potion." }; EnemyPhase(healingMessages);
			return new(true, true, healingMessages.AsReadOnly(), Snapshot());
		}
		if (command.Kind != CommandKind.Move || !Enum.IsDefined(command.Direction)) return Reject("That action is unavailable.");
		var delta = command.Direction switch {
			Direction.North => new GridPosition(0, -1),
			Direction.NorthEast => new(1, -1),
			Direction.East => new(1, 0),
			Direction.SouthEast => new(1, 1),
			Direction.South => new(0, 1),
			Direction.SouthWest => new(-1, 1),
			Direction.West => new(-1, 0),
			Direction.NorthWest => new(-1, -1),
			_ => default
		};
		var target = new GridPosition(player.X + delta.X, player.Y + delta.Y);
		if (!level.CanStep(player, target)) return Reject("The way is blocked.");
		var messages = new List<string>();
		var enemy = enemies.FirstOrDefault(e => e.Position == target);
		if (enemy is not null) {
			int damage = Combat.Damage(weapon.Damage, 0); enemy = enemy with { Health = enemy.Health - damage };
			enemies.RemoveAll(e => e.Id == enemy.Id);
			if (enemy.Health > 0) enemies.Add(enemy);
			messages.Add(enemy.Health > 0 ? $"You hit {enemy.Name} for {damage}." : $"You defeat {enemy.Name}.");
		}
		else { player = target; CollectLoot(messages); }
		turn++; visibility.Update(level, player);
		if (depth == 10 && level.At(player) == TerrainType.Goal) { status = RunStatus.Won; messages.Add("You reach the fountain. You win!"); }
		else EnemyPhase(messages);
		return new(true, true, messages.AsReadOnly(), Snapshot());
	}
	private ActionResult Reject(string message) => new(false, false, Array.AsReadOnly(new[] { message }), Snapshot());
	public GameSnapshot Snapshot() {
		var tiles = new TileSnapshot[level.Width * level.Height];
		for (int y = 0; y < level.Height; y++) for (int x = 0; x < level.Width; x++) {
			var p = new GridPosition(x, y); bool explored = visibility.IsExplored(p);
			tiles[y * level.Width + x] = new(p, explored ? level.At(p) : TerrainType.Wall, explored, visibility.IsVisible(p));
		}
		return new(seed, depth, status, health, rules.MaximumHealth, weapon, armor, potions, player, level.Width, level.Height, Array.AsReadOnly(tiles), turn, Array.AsReadOnly(enemies.Where(e => visibility.IsVisible(e.Position)).OrderBy(e => e.Id).ToArray()), Array.AsReadOnly(loot.Where(item => visibility.IsVisible(item.Position)).ToArray()));
	}
	private void EnemyPhase(List<string> messages) {
		foreach (var original in enemies.OrderBy(e => e.Id).ToArray()) {
			if (status != RunStatus.Playing) break;
			var enemy = enemies.FirstOrDefault(e => e.Id == original.Id); if (enemy is null) continue;
			if (level.CanStep(enemy.Position, player)) {
				int damage = Combat.Damage(enemy.Attack, armor.Reduction); health = Math.Max(0, health - damage); messages.Add($"{enemy.Name} hits you for {damage}.");
				if (health == 0) { status = RunStatus.Dead; messages.Add("You died. The run is over."); }
				continue;
			}
			var sight = new Visibility(level.Width, level.Height); sight.Update(level, enemy.Position);
			var choices = Neighbors(enemy.Position).Where(p => p != player && enemies.All(e => e.Id == enemy.Id || e.Position != p)).ToArray();
			GridPosition destination = enemy.Position;
			if (sight.IsVisible(player)) destination = Pursuit(enemy.Position) ?? enemy.Position;
			else if (choices.Length > 0) { int choice = random.Next(choices.Length + 1); if (choice < choices.Length) destination = choices[choice]; }
			int index = enemies.FindIndex(e => e.Id == enemy.Id); enemies[index] = enemy with { Position = destination };
		}
	}
	private IEnumerable<GridPosition> Neighbors(GridPosition position) {
		for (int dy = -1; dy <= 1; dy++) for (int dx = -1; dx <= 1; dx++) {
			var next = new GridPosition(position.X + dx, position.Y + dy); if (level.CanStep(position, next)) yield return next;
		}
	}
	private GridPosition? Pursuit(GridPosition from) {
		var visited = new HashSet<GridPosition> { from }; var queue = new Queue<(GridPosition Position, GridPosition First)>();
		foreach (var next in Neighbors(from).Where(p => enemies.All(e => e.Position != p))) { visited.Add(next); queue.Enqueue((next, next)); }
		while (queue.TryDequeue(out var item)) {
			if (item.Position == player) return item.First;
			foreach (var next in Neighbors(item.Position)) if (visited.Add(next) && enemies.All(e => e.Position != next)) queue.Enqueue((next, item.First));
		}
		return null;
	}
	private void CollectLoot(List<string> messages) {
		foreach (var item in loot.Where(item => item.Position == player).ToArray()) {
			loot.Remove(item);
			switch (item.Kind) {
				case LootKind.Potion: potions++; messages.Add("You collect a healing potion."); break;
				case LootKind.Weapon when item.Strength > weapon.Damage: weapon = new(item.Name, item.Strength); messages.Add($"You equip {item.Name}."); break;
				case LootKind.Armor when item.Strength > armor.Reduction: armor = new(item.Name, item.Strength); messages.Add($"You equip {item.Name}."); break;
				default: messages.Add($"You leave the weaker {item.Name} behind."); break;
			}
		}
	}
}
