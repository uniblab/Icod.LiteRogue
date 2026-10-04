# literogue(6) - Icod.LiteRogue

## NAME

literogue - a turn-based, text-mode dungeon game inspired by Rogue

## SYNOPSIS

```text
literogue [--seed <integer>] [--help] [--version]
```

Downloaded archives use the same options with `Icod.LiteRogue.exe` on Windows or `./Icod.LiteRogue` on Linux and macOS.

## DESCRIPTION

Explore ten procedurally generated dungeon floors, fight increasingly dangerous foes, collect better weapons and armor, and manage healing potions. Reach the fountain on floor 10 to win. Death ends the run. Stairs lead downward permanently; there is no ascent.

Each floor divides a 78x21 map into nine sectors, with up to one room in each sector. All rooms connect through one-cell-wide corridors and separated doors. Room walls and the outer map border remain intact. Empty sectors may contain connecting corridors.

Play proceeds one turn at a time. Move in eight directions, bump a foe to attack, and walk over loot to collect it. Terrain and positioning matter: narrow passages limit how many foes can reach you at once. Progress comes from equipment, with no experience levels, food, spells, or inventory menu.

Icod.LiteRogue uses [Icod.DCurses](https://github.com/uniblab/Icod.DCurses) for terminal rendering.

## OPTIONS

| Option | Meaning |
| --- | --- |
| `--seed <integer>` | Start a run with the specified signed 32-bit integer seed. Without this option, choose a random seed. The current seed appears in the status display. |
| `--help` | Print usage and a compact control reference, then exit. |
| `--version` | Print the installed game version, then exit. |

Options are case-sensitive. Supply the seed as a separate argument, for example `--seed 73`. Reproducing a run requires the same game version, seed, and commands; layouts may change between versions. Help and version output work with redirected input and output. Gameplay requires an interactive terminal.

## CONTROLS

Movement keys:

| Direction | Letter | Keypad digit | Navigation key |
| --- | --- | --- | --- |
| North | `k` | `8` | Up arrow |
| Northeast | `u` | `9` | Page Up |
| East | `l` | `6` | Right arrow |
| Southeast | `n` | `3` | Page Down |
| South | `j` | `2` | Down arrow |
| Southwest | `b` | `1` | End |
| West | `h` | `4` | Left arrow |
| Northwest | `y` | `7` | Home |

Other commands:

| Key | Action |
| --- | --- |
| `p` | Drink a healing potion. |
| `>` | Descend while standing on stairs. |
| `?` | Toggle the help display. |
| Escape | Close the help display. |
| `q` | Quit. |
| `r` | Start a fresh run with a new random seed after death or victory. |

Commands use lowercase letters. While help is open, close it to resume play. If a terminal does not report keypad digits as expected, use the letter or navigation keys.

## DISPLAY

The status display shows health, dungeon floor, potion count, run seed, equipped weapon and armor, and elapsed turns. Messages describe recent actions.

| Glyph | Meaning |
| --- | --- |
| `@` | Player |
| `#` | Wall |
| `.` | Room floor |
| `:` | Corridor |
| `+` | Door |
| `>` | Down stairs |
| `&` | Final fountain |
| `)` | Weapon |
| `]` | Armor |
| `!` | Healing potion |
| `r` | Rat |
| `g` | Goblin |
| `o` | Orc |
| `T` | Troll |

Entering a room reveals it. Corridors reveal nearby connected tiles. Previously explored terrain stays mapped; enemies and loot appear only while currently visible. Foes pursue when they see you and otherwise wander.

A terminal of at least 78 columns by 25 rows shows the full dungeon. Smaller terminals show a viewport centered on the player. Below 30 columns by 6 rows, the game asks you to enlarge the terminal. Resizing does not consume a turn or regenerate the map.

## GAMEPLAY

### Turns and combat

Moving, attacking, and drinking a potion each cost one turn, after which each foe can act once. Blocked moves, rejected potion use, help, and resizing do not cost turns. Descending costs a turn but does not give enemies an arrival attack. Reaching the final fountain wins immediately.

Every legal melee attack hits. Damage is the attacker's weapon or attack strength minus the defender's armor reduction, with a minimum of one. A diagonal move is blocked when both neighboring orthogonal tiles are walls; enemies follow the same rule.

### Health and equipment

Start with 30 health, a Knife, Clothes, and no potions. Maximum health stays at 30. Each healing potion restores up to 10 health. Drinking at full health or with no potions is rejected.

Walking over loot collects it automatically. Stronger weapons and armor replace the equipped item; equal or weaker gear is discarded. Potions accumulate until used. There are no save files: quitting abandons the current run.

| Floors | Foe | Foe health / attack | Equipment available | Weapon damage / armor reduction |
| --- | --- | --- | --- | --- |
| 1-2 | Rat | 4 / 1 | Knife / Clothes | 4 / 0 |
| 3-5 | Goblin | 8 / 2 | Sword / Leather | 6 / 1 |
| 6-8 | Orc | 12 / 3 | Axe / Chain | 8 / 2 |
| 9-10 | Troll | 16 / 4 | Runeblade / Plate | 10 / 3 |

Explore before descending to find equipment and potions for the deeper floors.

## INSTALLATION

### NuGet tool

Install the .NET 10 SDK, then install the published tool:

```sh
dotnet tool install --global Icod.LiteRogue
literogue
```

To select a particular release, add `--version <version>`, including the full prerelease suffix when applicable.

Update or remove the global tool with:

```sh
dotnet tool update --global Icod.LiteRogue
dotnet tool uninstall --global Icod.LiteRogue
```

If `literogue` is not found after installation, check that the .NET global-tool directory is on your `PATH`.

### Downloaded archive

Install the .NET 10 runtime and extract the matching archive from [GitHub Releases](https://github.com/uniblab/Icod.LiteRogue/releases). Archives are framework-dependent and available for `win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-x64`, and `osx-arm64`.

Run the executable from the extracted directory:

```powershell
# Windows PowerShell
.\Icod.LiteRogue.exe
```

```sh
# Linux and macOS
./Icod.LiteRogue
```

## EXAMPLES

Start a random run:

```sh
literogue
```

Start a run with a known seed:

```sh
literogue --seed 73
```

Show the command reference or report the version:

```sh
literogue --help
literogue --version
```

## EXIT STATUS

| Status | Meaning |
| --- | --- |
| `0` | Normal exit, help, version output, or orderly cancellation. Includes quitting after victory or death. |
| `1` | A terminal or runtime error. A diagnostic is written to standard error. |
| `2` | Invalid command-line arguments. Usage is written to standard error. |

## FILES

The NuGet package and release archives include:

| File | Purpose |
| --- | --- |
| `README.md` | This command and gameplay reference. |
| `docs/Playing.md` | Additional player guidance. |
| `LICENSE` | GNU General Public License text. |
| `THIRD-PARTY-NOTICES.md` | Runtime dependency notices. |
| `LICENSES/` | Runtime dependency license texts. |

## BUGS

For clipped output, enlarge the terminal. For keypad problems, try letter or navigation keys. Run gameplay in a real terminal with interactive standard input and output.

[Report problems](https://github.com/uniblab/Icod.LiteRogue/issues/new) with the game version, operating system and CPU, terminal name and version, terminal dimensions, run seed, and the commands needed to reproduce the problem. State which movement keys were used for input problems.

## AUTHORS

Rogue was originally created by Michael Toy and [Glenn Wichman](https://en.wikipedia.org/wiki/Glenn_Wichman), with later contributions by [Ken Arnold](https://en.wikipedia.org/wiki/Ken_Arnold). Their work is the inspiration for Icod.LiteRogue.

This .NET implementation is by Timothy J. Bruce.

## COPYRIGHT

Copyright (C) 2026 Timothy J. Bruce \<uniblab@hotmail.com\>.

Icod.LiteRogue is free software under the GNU General Public License, version 3 or, at your option, any later version. See the included `LICENSE`. The program comes without any warranty, including merchantability or fitness for a particular purpose. Runtime dependencies retain their own licenses; see the included dependency notices and license texts.

## SEE ALSO

- [Project repository](https://github.com/uniblab/Icod.LiteRogue)
- [NuGet package](https://www.nuget.org/packages/Icod.LiteRogue)
- [Player guide](https://github.com/uniblab/Icod.LiteRogue/blob/main/docs/Playing.md)
- [Changelog](https://github.com/uniblab/Icod.LiteRogue/blob/main/CHANGELOG.md)
- [Game design](https://github.com/uniblab/Icod.LiteRogue/blob/main/docs/Icod.LiteRogue-Design.md)
- [Development roadmap](https://github.com/uniblab/Icod.LiteRogue/blob/main/Icod.LiteRogue-Development-Roadmap.md)
- [Build and packaging procedures](https://github.com/uniblab/Icod.LiteRogue/blob/main/packaging/README.md)
