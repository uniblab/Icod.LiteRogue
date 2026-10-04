# Changelog

## 1.0.0 — Unreleased

- Complete ten-floor Rogue game with seeded 3x3 room generation, connected corridors, exploration memory, and current entity visibility.
- Corridors stay one cell wide, enter rooms through separated doors, and preserve room walls and a solid outer map border. Routing favors nearby facing doorways and searches all destination doors together instead of repeating a search for each pair.
- Combined map and population validation retains 10,000 seed/depth cases and adds dense, sparse, and minimum-size layouts. Seeded generation remains repeatable, but this generator update changes the layouts produced by existing seeds.
- Eight-direction movement, turn-based melee, increasingly strong enemies, automatic equipment upgrades, healing potions, permanent death, and immediate victory at the final fountain.
- MVC implementation with a terminal-independent Model and Icod.DCurses 2.2.0 View.
- Shared uniblab build procedures, GPL3 license, one NuGet tool (`literogue`), and six framework-dependent executable archives.
- Trusted Publishing uses `release.yaml`, environment `Release`, and the repository owner as the default NuGet username.
- Maintainer accepted the existing game for stable 1.0.0 on October 4, 2026. This release uses Icod.DCurses 2.2.0; further dependency development is deferred.

Alpha.1 exploration, alpha.2 combat/loot and alpha.3 complete gameplay were internal development checkpoints. The maintainer accepted promotion directly to 1.0.0 without separate alpha.4 or RC.1 publications. Additional human balance feedback and live platform testing remain follow-up work. Publication is pending merge and the matching `v1.0.0` tag.
