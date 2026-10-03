# Changelog

## 1.0.0-alpha.3 — Unreleased

- Complete ten-floor Rogue game with seeded 3x3 room generation, connected corridors, exploration memory, and current entity visibility.
- Corridors stay one cell wide, enter rooms through separated doors, and preserve room walls and a solid outer map border. Routing favors nearby facing doorways and searches all destination doors together instead of repeating a search for each pair.
- Combined map and population validation retains 10,000 seed/depth cases and adds dense, sparse, and minimum-size layouts. Seeded generation remains repeatable, but this generator update changes the layouts produced by existing seeds.
- Eight-direction movement, turn-based melee, increasingly strong enemies, automatic equipment upgrades, healing potions, permanent death, and immediate victory at the final fountain.
- MVC implementation with a terminal-independent Model and Icod.DCurses 2.2.0 View.
- Shared uniblab build procedures, GPL3 license, one NuGet tool (`literogue`), and six framework-dependent executable archives.
- Trusted Publishing uses `release.yaml`, environment `Release`, and the repository owner as the default NuGet username.

Alpha.1 exploration and alpha.2 combat/loot were internal development checkpoints; neither was published. Alpha.4 balance acceptance, cross-platform live terminal acceptance, RC.1 and stable 1.0.0 remain pending.
