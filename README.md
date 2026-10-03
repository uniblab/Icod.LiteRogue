# Icod.LiteRogue

A small, turn-based Rogue-style game rendered with Icod.DCurses and organized using Model-View-Controller.

Explore ten procedurally generated dungeon levels. Each level uses a 3x3 arrangement of possible rooms joined by corridors. Fight increasingly dangerous foes, collect better weapons and armor, and manage healing potions. Reach the designated destination on level 10 to win. Death ends the run, and descent is permanent.

## Project status

The gameplay and MVC foundation is approved. Development toward 1.0.0 is planned through playable alpha releases and a release candidate.

- [Game design](docs/Icod.LiteRogue-Design.md)
- [Main development roadmap](Icod.LiteRogue-Development-Roadmap.md)
- [1.0.0 development plan](docs/superpowers/plans/2026-10-03-icod-literogue-1.0.0.md)

The planned project structure consists of a game-model library and a console application containing the Controller and Icod.DCurses View. The release plan proposes a .NET 10 console application distributed as a .NET tool and downloadable executable archives.

## License

GNU Lesser General Public License, version 3. See [LICENSE](LICENSE), copied from [uniblab/.github](https://github.com/uniblab/.github/blob/main/LGPL3.LICENSE).
