# Playing Icod.LiteRogue

Reach the fountain (`&`) on dungeon floor 10 to win. Each run has ten generated floors; stairs (`>`) on floors 1–9 lead downward permanently. There is no ascent. Death ends the run; press `r` to start a fresh run or `q` to quit. No save files, experience levels, food, spells, or inventory menu are needed.

## Requirements and launch

Install the .NET 10 runtime and use an interactive terminal. The tool command is `literogue`; downloaded archives use `Icod.LiteRogue` (`Icod.LiteRogue.exe` on Windows). `--help` and `--version` work without a terminal. `--seed 7` starts a reproducible run; the seed is displayed in the HUD. Reproduction requires the same game version and commands.

The dungeon always measures 78x21 regardless of terminal size. A terminal of 78x25 or larger displays the full map; smaller windows show a viewport centered on the player. Below 30x6, enlarge the terminal when prompted. Resizing does not take a turn or regenerate terrain.

## Controls

| Action | Keys |
| --- | --- |
| Cardinal movement | Arrow keys; `h` left, `j` down, `k` up, `l` right |
| Diagonal movement | `y` northwest, `u` northeast, `b` southwest, `n` southeast |
| Eight directions on keypad | `7 8 9 / 4 6 / 1 2 3`; Home/End/Page Up/Page Down also work |
| Drink healing potion | `p` |
| Descend while on stairs | `>` |
| Help | `?` (toggle), Escape (close) |
| Quit | `q` |
| New run after death or victory | `r` |

Bump an adjacent foe to attack it. Every legal attack hits; weapon damage minus armor reduction is at least one. Moving, attacking, and drinking cost one turn, after which each foe can act once. Blocked moves, invalid drinks, help, and resize do not cost turns. Diagonals are blocked when both adjacent orthogonal tiles are walls. Enemies follow the same rule. Descent takes a turn but gives no arrival attack; victory takes effect immediately on reaching the fountain.

## Glyphs and exploration

| Glyph | Meaning |
| --- | --- |
| `@` | You |
| `#` / `.` / `:` / `+` | Wall / room floor / corridor / door |
| `>` / `&` | Down stairs / final fountain |
| `)` / `]` / `!` | Weapon / armor / healing potion |
| `r` / `g` / `o` / `T` | Rat / goblin / orc / troll |

Entering a room reveals it. Corridors reveal nearby connected tiles; previously explored terrain stays mapped. Enemies and loot appear only while currently visible. Foes pursue when they see you, otherwise wander. Use passages to limit how many foes can reach you at once.

## Equipment, health and loot

Maximum health is always 30. Start with a Knife (4 damage), Clothes (0 reduction), and no potions. Walking over loot collects it automatically: stronger gear replaces equipped gear, while equal or weaker gear is discarded. Each potion restores 10 health, capped at 30, and is consumed before enemies respond. At full health or with no potions, drinking is rejected.

| Floors | Foe | Health / attack | Gear available | Damage / reduction |
| --- | --- | --- | --- | --- |
| 1–2 | Rat | 4 / 1 | Knife / Clothes | 4 / 0 |
| 3–5 | Goblin | 8 / 2 | Sword / Leather | 6 / 1 |
| 6–8 | Orc | 12 / 3 | Axe / Chain | 8 / 2 |
| 9–10 | Troll | 16 / 4 | Runeblade / Plate | 10 / 3 |

Each floor requests `2 + floor((depth-1)/2)` foes, one weapon, one armor, and `2 + floor(depth/3)` potions. Placement is capped by available tiles, with entry and destination reserved and no overlaps at generation. Foes begin at least four tiles from the entry. All rooms and traversable terrain are connected; corridors can pass through empty sectors. Exploration helps find gear before entering deeper floors.

## Troubleshooting

If the display is clipped, enlarge the terminal; `?` shows the controls. If keypad digits are not reported by the terminal, use vi keys or navigation keys. Use a real terminal rather than redirected standard input/output for gameplay. Report the game version, OS, terminal name, seed, and steps that led to a problem. The application restores its owned terminal session on quit, end of input, cancellation, or an input failure.
