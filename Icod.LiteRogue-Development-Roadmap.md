# Icod.LiteRogue Development Roadmap

**Target:** 1.0.0  
**Status:** Complete gameplay implemented at unreleased alpha.3; balance/platform acceptance and stable release pending
**Updated:** October 3, 2026  
**Repository:** [uniblab/Icod.LiteRogue](https://github.com/uniblab/Icod.LiteRogue)

## Product direction

Deliver a small, complete Rogue-style game: one adventurer, ten procedural dungeon levels, rooms in nine sectors, connected corridors, eight-direction movement, turn-based melee combat, equipment upgrades, healing potions, permanent descent, and death ending the run. Reaching the designated destination on level 10 wins.

The [game design](docs/Icod.LiteRogue-Design.md) defines the approved gameplay and MVC boundaries. The [1.0.0 development plan](docs/superpowers/plans/2026-10-03-icod-literogue-1.0.0.md) supplies implementation tasks, proposed defaults, verification, packaging, and release gates.

## Release sequence

| Milestone | Work | Player-visible result | Gate |
| --- | --- | --- | --- |
| Foundation | T1000 | Model/application boundaries and automated checks | Model operates without a terminal or DCurses dependency |
| 1.0.0-alpha.1 | T1001–T1002 | Explore a generated floor through DCurses | Connected generation, legal movement, fog, HUD, input, and terminal restoration work |
| 1.0.0-alpha.2 | T1003–T1004 | Fight, collect upgrades, drink potions, and die | Combat, enemy turns, equipment, healing, and death are integrated |
| 1.0.0-alpha.3 | T1005 | Play the complete ten-level descent and win | All approved gameplay is present; runs can end in victory or death |
| 1.0.0-alpha.4 | T1006 | A balanced, dependable complete game | Seed sweeps, terminal hardening, and documented playtests pass |
| 1.0.0-rc.1 | T1007 | Installable candidate with final controls and documentation | Clean installation, package/archive checks, and platform acceptance pass |
| 1.0.0 | T1008 | Stable release | Final checklist passes on the tagged source commit |

These are acceptance milestones, not dates. Additional alpha or release-candidate revisions may fix defects without expanding the 1.0.0 scope. A milestone may remain an internal checkpoint if publishing it would add no useful testing opportunity.

## Tranches

T1000–T1005 are implemented. Automated terrain and population checks each cover 10,000 seed/depth cases. Twenty balance simulations cover ten seeds, with both wins and deaths. T1006–T1007 automation and documentation are present; human balance and live platform acceptance remain pending. T1008 is intentionally unstarted until the candidate is accepted. See [acceptance evidence](docs/Terminal-Testing.md).

| Tranche | Deliverable | Depends on |
| --- | --- | --- |
| T1000 | Solution, Model contract, application seam, tests, and build/CI foundation | Approved design and reviewed plan |
| T1001 | Seeded 3x3 dungeon generation and visibility | T1000 |
| T1002 | Eight-direction movement, DCurses rendering, keyboard input, and application lifecycle | T1001 |
| T1003 | Melee combat, enemies, turn resolution, and death | T1002 |
| T1004 | Automatic equipment upgrades and potion healing | T1003 |
| T1005 | Permanent descent through ten levels and immediate victory at the final destination | T1004 |
| T1006 | Balance, reproducibility, malformed-input handling, resize behavior, and stress checks | T1005 |
| T1007 | Release packaging, player documentation, license delivery, and release candidate | T1006 |
| T1008 | Stable release verification and publication readiness | T1007 |

## Scope discipline

1.0.0 implements the approved simple game. It does not require the worlds, puzzles, companions, politics, or quest systems of the separate Zelda Roguelike proposal. Experience levels, food, spellcasting, save/resume, and a general-purpose rules runtime are outside this release.

The Model owns rules and state. Icod.DCurses remains confined to the application View and its terminal adapters. Display refreshes and resizing do not advance turns or regenerate the dungeon.

## Release practice

Use package versions such as `1.0.0-alpha.1`, `1.0.0-rc.1`, and `1.0.0`, with matching tags `v1.0.0-alpha.1`, `v1.0.0-rc.1`, and `v1.0.0`. Mark alpha and candidate GitHub Releases as prereleases.

Validate pull requests before merge, validate Release distributions on main, and publish from version tags contained in main. Record each accepted tranche and release in this roadmap and the changelog. Release creation and publication occur when requested by the maintainer.

## Stable release definition

A new installation must launch the game, explain its controls, and support a complete run to victory or death. Every generated floor must be reachable and usable; turns and visibility must follow the design; equipment and potions must work; the terminal must recover after exit. The shipped package and archives must include their documentation and license, identify the exact version, and pass the documented platform checks.
