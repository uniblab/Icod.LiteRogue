# Icod.LiteRogue

[![Build](https://github.com/uniblab/Icod.LiteRogue/actions/workflows/main.yaml/badge.svg?branch=main)](https://github.com/uniblab/Icod.LiteRogue/actions/workflows/main.yaml)
[![NuGet](https://img.shields.io/nuget/vpre/Icod.LiteRogue.svg)](https://www.nuget.org/packages/Icod.LiteRogue)
[![NuGet downloads](https://img.shields.io/nuget/dt/Icod.LiteRogue.svg)](https://www.nuget.org/packages/Icod.LiteRogue)
[![License: GPL v3](https://img.shields.io/badge/license-GPLv3-blue.svg)](LICENSE)

Icod.LiteRogue is a small, turn-based Rogue-style game rendered with [Icod.DCurses](https://github.com/uniblab/Icod.DCurses) and organized using Model-View-Controller.

Explore ten procedurally generated dungeon levels. Each level uses a 3x3 arrangement of possible rooms joined by corridors. Fight increasingly dangerous foes, collect better weapons and armor, and manage healing potions. Reach the fountain on level 10 to win. Death ends the run, and descent is permanent.

## Features

- Ten reproducible dungeon floors generated from a displayed run seed.
- Up to nine rooms per floor, with connected corridors through occupied and empty sectors.
- Eight-direction movement and bump-to-attack melee combat.
- Positioning, armor, and corridor control that matter during fights.
- Automatic weapon, armor, and healing-potion collection without an inventory menu.
- Room and corridor exploration with persistent terrain memory and currently visible creatures.
- Increasingly dangerous rats, goblins, orcs, and trolls.
- Permanent death, permanent descent, and immediate victory at the final fountain.
- A terminal-independent Model with an Icod.DCurses View and application Controller.
- One cross-platform .NET tool plus framework-dependent archives for six operating-system and CPU targets.

## What it looks like

A representative viewport combines the run status, explored dungeon, visible creatures, and nearby loot:

```text
HP 23/30  Floor 6/10  Potions 2  Seed 73
Axe (8) / Chain (2)  Turn 418

                 ######+################
                 #......................#
                 #.......o..............#
        :::::::::+..........@....!......#
                 #...................]..#
                 ############+###########
                             :
                             :

@ you # wall . floor : corridor + door > stairs & goal
Move hjklyubn / keypad / arrows | p potion | > down | ? help | q quit
```

The dungeon is always 78x21. Large terminals show the entire floor; smaller terminals show a viewport centered on the player. Very small terminals display a resize prompt without changing the dungeon or consuming a turn.

## Install and play

### NuGet tool

Installing the global tool requires the .NET 10 SDK. Specify the prerelease version explicitly:

```sh
dotnet tool install --global Icod.LiteRogue --version 1.0.0-alpha.3
literogue
```

Start a reproducible run by supplying a signed 32-bit integer seed:

```sh
literogue --seed 73
```

Update or remove the global tool with:

```sh
dotnet tool update --global Icod.LiteRogue --version <version>
dotnet tool uninstall --global Icod.LiteRogue
```

### Downloadable archive

GitHub Releases provide framework-dependent archives for `win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-x64`, and `osx-arm64`. Install the .NET 10 runtime, download and extract the archive for your platform, then run:

```text
Icod.LiteRogue.exe    Windows
./Icod.LiteRogue     Linux and macOS
```

Gameplay requires an interactive terminal. Redirected input and output are suitable for `--help` and `--version`, but not for a run.

## Command line

| Command | Result |
| --- | --- |
| `literogue` | Starts a new run with a randomly selected seed. |
| `literogue --seed 73` | Starts a reproducible run with seed 73. |
| `literogue --help` | Prints usage and a compact control reference without opening the terminal UI. |
| `literogue --version` | Prints the installed game version. |

Invalid arguments print usage to standard error and exit before opening the terminal UI.

## Essential controls

| Action | Keys |
| --- | --- |
| Cardinal movement | Arrow keys or `h j k l` |
| Diagonal movement | `y u b n`, keypad diagonals, or Home/End/Page Up/Page Down |
| Drink a healing potion | `p` |
| Descend while standing on stairs | `>` |
| Toggle help | `?` |
| Quit | `q` |
| Start a fresh run after death or victory | `r` |

Bump a foe to attack it and walk over loot to collect it. Better equipment replaces current gear automatically. Maximum health remains 30, and each potion restores up to 10 health. See the [player guide](docs/Playing.md) for complete controls, glyphs, combat rules, equipment tables, exploration behavior, and troubleshooting.

## Development status

The complete game is implemented at **1.0.0-alpha.3**. Balance feedback and live Windows/macOS terminal acceptance remain ahead of alpha.4, RC.1, and stable 1.0.0.

- [Game design](docs/Icod.LiteRogue-Design.md)
- [Main development roadmap](Icod.LiteRogue-Development-Roadmap.md)
- [1.0.0 development plan](docs/superpowers/plans/2026-10-03-icod-literogue-1.0.0.md)
- [Terminal and release acceptance](docs/Terminal-Testing.md)
- [Changelog](CHANGELOG.md)

The private Model library owns generation, visibility, combat, progression, and every gameplay transition. The console application supplies the Controller and Icod.DCurses View. Rendering, help, and resizing consume immutable snapshots and do not advance the game.

Build locally with the .NET 10 SDK and PowerShell:

```sh
./build.sh       # Unix-like hosts
build.cmd        # Windows
```

The shared procedure cleans, restores, builds, tests, packs, and validates exactly one NuGet tool package. Run from source with:

```sh
dotnet run --project src/Icod.LiteRogue -- --seed 73
```

See the [packaging procedures](packaging/README.md) for distribution and Trusted Publishing details.

## Reporting problems

[Open an issue](https://github.com/uniblab/Icod.LiteRogue/issues/new) with the game version, operating system and CPU, terminal name and version, run seed, and the commands or steps that reproduce the problem. For display or input problems, also include the terminal dimensions and whether arrow, keypad, or vi-style movement keys were used.

## License

Icod.LiteRogue is licensed under the GNU General Public License, version 3. See [LICENSE](LICENSE), copied from [uniblab/.github](https://github.com/uniblab/.github/blob/main/GPL3.LICENSE).

Runtime dependencies retain their own licenses; see [dependency notices](THIRD-PARTY-NOTICES.md).
