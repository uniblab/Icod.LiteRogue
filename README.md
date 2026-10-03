# Icod.LiteRogue

A small, turn-based Rogue-style game rendered with Icod.DCurses and organized using Model-View-Controller.

Explore ten procedurally generated dungeon levels. Each level uses a 3x3 arrangement of possible rooms joined by corridors. Fight increasingly dangerous foes, collect better weapons and armor, and manage healing potions. Reach the designated destination on level 10 to win. Death ends the run, and descent is permanent.

## Install and play

Requires the .NET 10 runtime and an interactive terminal. Once a prerelease is published:

```sh
dotnet tool install --global Icod.LiteRogue --version 1.0.0-alpha.3
literogue
literogue --seed 7
```

Update with `dotnet tool update --global Icod.LiteRogue --version <version>`; uninstall with `dotnet tool uninstall --global Icod.LiteRogue`. Framework-dependent archives contain `Icod.LiteRogue` (`Icod.LiteRogue.exe` on Windows) for Windows, Linux and macOS, x64 and ARM64.

Move with arrows, vi keys `h j k l y u b n`, or keypad `1 2 3 4 6 7 8 9`. Bump foes to attack, walk over loot to collect it, press `p` to drink a potion, `>` to descend, `?` for help and `q` to quit. After victory or death, `r` starts a fresh run. Maximum health stays 30; potions heal up to 10.

A terminal of 78x25 shows the whole dungeon; smaller windows use a viewport. Resizing preserves the world and takes no turn. See the [player guide](docs/Playing.md) for glyphs, rules, balance tables and troubleshooting.

## Development status

The complete game is implemented at **1.0.0-alpha.3**, currently unreleased. Balance feedback and live Windows/macOS terminal acceptance remain ahead of alpha.4, RC.1 and stable 1.0.0.

- [Game design](docs/Icod.LiteRogue-Design.md)
- [Main development roadmap](Icod.LiteRogue-Development-Roadmap.md)
- [1.0.0 development plan](docs/superpowers/plans/2026-10-03-icod-literogue-1.0.0.md)

The private Model library owns generation, visibility and gameplay state. The console application supplies the Controller and Icod.DCurses View; resizing and rendering consume immutable snapshots.

Build locally with the .NET 10 SDK and PowerShell: `./build.sh` on Unix or `build.cmd` on Windows. The shared procedure cleans, restores, builds, tests, packs and validates one NuGet tool package. Run from source with `dotnet run --project src/Icod.LiteRogue -- --seed 7`. See [packaging procedures](packaging/README.md) and [terminal acceptance](docs/Terminal-Testing.md).

## License

GNU General Public License, version 3. See [LICENSE](LICENSE), copied from [uniblab/.github](https://github.com/uniblab/.github/blob/main/GPL3.LICENSE).

Runtime dependencies retain their licenses; see [dependency notices](THIRD-PARTY-NOTICES.md).
