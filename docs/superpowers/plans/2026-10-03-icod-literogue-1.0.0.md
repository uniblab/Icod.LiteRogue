# Icod.LiteRogue 1.0.0 Development Implementation Plan

> **For agentic workers:** Use superpowers:executing-plans for native task-by-task execution, or superpowers:subagent-driven-development only if the maintainer selects it. Steps use checkbox syntax for tracking.

**Goal:** Ship a complete, small Rogue-style game as Icod.LiteRogue 1.0.0, using playable alpha releases to validate it along the way.

**Architecture:** A Model library owns dungeon generation and all gameplay state and rules. A console application supplies the Controller, a DCurses View, and terminal/input lifecycle adapters. The View consumes immutable presentation snapshots; only Model commands advance gameplay.

**Tech stack proposed for this release:** C#; .NET 10; Icod.DCurses 2.2.0 as the initial stable dependency; xUnit for tests; MSBuild and the shared uniblab build/distribution conventions. Recheck package availability at T1000 and pin the accepted stable version. Text graphics need no raster or atlas functionality.

**Spec:** [Approved game design](../../Icod.LiteRogue-Design.md)  
**Main roadmap:** [Release sequence and tranche status](../../../Icod.LiteRogue-Development-Roadmap.md)  
**Status:** Approved; implementation active  
**Date:** October 3, 2026

## Global constraints

- Ten dungeon levels; sectors arranged in a 3x3 grid; zero or one room per sector.
- At least one room per floor; every room connected; the player can reach the staircase or final goal.
- Descent only; death ends the run; the destination on level 10 immediately wins.
- Player and enemies use eight-direction movement; closed wall corners block diagonal movement.
- Equipment-only advancement; fixed maximum health; healing potions are the sole healing source.
- One equipped weapon, one equipped suit of armor, and a potion count; collection and better-equipment replacement are automatic.
- Entering a room reveals its interior; explored terrain remains mapped; entities obey current visibility.
- The Model has no dependency on Icod.DCurses or terminal input. Rendering and resizing do not mutate game state.
- Ship the requested GPL3 text as root `LICENSE`, with matching license notices in delivered artifacts. The Model is private; the application is the single NuGet .NET tool package.
- C#, PowerShell, and ordinary shell/cmd scripts are sufficient for the product and its build tooling.

## Proposed implementation defaults

These fill gaps the high-level design intentionally leaves open. Review them with this plan before implementation. Balance values may change during alpha testing; document the final values.

| Area | Initial decision |
| --- | --- |
| Dungeon dimensions | Fixed 78 columns by 21 rows, independent of terminal size; nine sectors of 26 by 7 |
| Rooms | 70% occupancy chance per sector, forcing one if all are empty; room exterior 4–24 columns by 4–5 rows, inset at least one cell within its sector |
| Corridors | Connect room centers with a randomized spanning tree and orthogonal passages; optional extra connections add loops; corridor carving must leave all traversable tiles reachable |
| Goal | A distinct goal tile on level 10; exact landmark name is presentation only |
| Turn costs | A successful move, melee attack, or potion use consumes a turn; rejected commands, help, redraw, and resize consume none |
| Transition | Descending replaces the floor and preserves health, gear, and potions; the arrival floor starts without an immediate enemy response |
| Terminal outcomes | Check goal/death during action resolution; victory stops enemy responses immediately; death stops the remaining enemy phase |
| Diagonals | A diagonal is blocked when both orthogonal neighbors are walls; apply the same rule to movement, pathfinding, and melee reach |
| Enemy phase | Each living enemy gets at most one move or attack, in stable ID order; enemies never overlap |
| Enemy behavior | Currently seeing the player permits pursuit; otherwise use a simple legal wander; obey the same terrain rules as the player |
| Damage | Every legal melee attack hits; damage is at least 1 after armor mitigation; initial knife damage 4, starting armor reduction 0 |
| Health and potions | Maximum health 30; each potion restores up to 10, capped at maximum; zero potions or full health rejects drinking without consuming a turn or potion |
| Loot | Entering a loot tile collects it; stronger gear replaces the current item; equal or weaker gear leaves equipped statistics unchanged |
| Visibility | A room interior is visible while occupied; corridor vision uses adjacent traversable cells and connected openings; all visible terrain becomes explored |
| Controls | Arrow keys for cardinal movement; vi keys `h j k l y u b n` and numeric keypad `1 2 3 4 6 7 8 9` for eight directions; `p` drinks, `>` descends, `?` shows help, `q` quits |
| Randomness | One recorded seed per run; deterministic generation and enemy choices for the same seed and commands in the same build; redraw/input polling never draw random values |
| Distribution | One NuGet .NET tool package, `Icod.LiteRogue`, command `literogue`; a private Model project is included in the app distribution; downloadable archives accompany releases |

Use modest, depth-based content tables: four weapon strengths beginning at 4, four armor reductions beginning at 0, and enemy health/attack tiers that increase across levels. T1004 records exact tables; T1006 tunes them through playtests. Loot must not occupy the entry, exit, goal, or an enemy tile. The generator reserves the entry/exit/goal before population.

## Planned file structure

These are planned paths, not files created by this documentation PR.

| Path | Responsibility |
| --- | --- |
| `Icod.LiteRogue.sln`, `global.json`, `Directory.Build.props` | One root solution, selected SDK, shared build settings |
| `src/Icod.LiteRogue.Model/Icod.LiteRogue.Model.csproj` | Non-packable gameplay library |
| `src/Icod.LiteRogue.Model/GameSession.cs`, `GameCommand.cs`, `GameRules.cs` | Run state, command boundary, fixed rules/balance values |
| `src/Icod.LiteRogue.Model/World/GridPosition.cs`, `DungeonLevel.cs`, `DungeonGenerator.cs` | Coordinates, terrain/rooms, procedural generation |
| `src/Icod.LiteRogue.Model/World/Visibility.cs`, `LevelPopulation.cs` | Exploration/current visibility and enemy/loot placement |
| `src/Icod.LiteRogue.Model/Actors/Enemy.cs`, `Combat.cs` | Enemy state and melee resolution |
| `src/Icod.LiteRogue.Model/Items/Equipment.cs`, `Inventory.cs` | Equipment and potion count |
| `src/Icod.LiteRogue.Model/Presentation/GameSnapshot.cs`, `ActionResult.cs` | Immutable display data and command outcome |
| `src/Icod.LiteRogue/Icod.LiteRogue.csproj`, `Program.cs` | .NET tool application and startup |
| `src/Icod.LiteRogue/Controller/GameController.cs`, `KeyBindings.cs` | Input-to-command mapping and loop coordination |
| `src/Icod.LiteRogue/View/IGameView.cs`, `DCursesGameView.cs` | Presentation seam and DCurses renderer |
| `src/Icod.LiteRogue/Terminal/ITerminalInput.cs`, `DCursesTerminalHost.cs` | Input events and owned terminal lifecycle |
| `tests/Icod.LiteRogue.Model.Tests/` | Generation, visibility, gameplay, and run-state tests |
| `tests/Icod.LiteRogue.Tests/` | Controller, rendering projection, arguments, and lifecycle tests |
| `build.cmd`, `build.sh`, `packaging/`, `.github/workflows/` | Build, verification, package/archive validation, release automation |
| `CHANGELOG.md`, `docs/Playing.md`, `docs/Terminal-Testing.md` | Release history, player guide, and live-terminal acceptance |

## Model and application contracts

Define these names at T1000 and extend their data as the owning tranches arrive. Concrete DCurses API calls belong to the terminal adapter; inspect the pinned library's documentation and samples before coding that adapter.

- `GridPosition(int X, int Y)`: immutable grid coordinate.
- `Direction`: eight values, mapped to fixed grid deltas.
- `GameCommand`: immutable command with kind `Move`, `DrinkPotion`, or `Descend`; move includes a direction. Quit and help are application operations.
- `RunStatus`: `Playing`, `Won`, or `Dead`.
- `GameSession.NewRun(int seed, GameRules rules)`: create a fresh run.
- `GameSession.Apply(GameCommand command)`: return `ActionResult`; the Model decides legality, turn consumption, messages, and enemy responses.
- `GameSession.Snapshot()`: return `GameSnapshot` with depth, status, player resources, explored terrain, currently visible entities, and the run seed.
- `ActionResult`: accepted/rejected, turn consumed, messages, and resulting snapshot.
- `DungeonGenerator.Generate(int depth, int seed, GameRules rules)`: return a connected `DungeonLevel`.
- `IGameView.Render(GameSnapshot snapshot, IReadOnlyList<string> messages)`: render without accessing mutable Model objects.
- `KeyBindings.TryTranslate(TerminalKey key, out GameCommand command)`: translate a normalized app-owned input value; keep DCurses key types out of the Model.

`TerminalKey` carries a normalized character or navigation key. `ITerminalInput.ReadAsync(CancellationToken cancellationToken)` returns a key, resize, or end-of-input event through an app-owned `TerminalInputEvent` value. `DCursesTerminalHost` owns initialization, event translation, and disposal. The Controller handles help/quit/end-of-input directly and submits only gameplay commands to the Model.

Snapshots must defensively copy or wrap collections so a caller cannot mutate Model state by casting a returned collection. Test helpers may construct small fixed levels through internal test access; the shipped command line does not expose cheat commands.

## Review focus

1. A tiny terminal must show a useful size message or clipped viewport and recover after enlargement without changing the dungeon or taking a turn. Owned by T1002/T1006.
2. Closed diagonal corners and map edges must behave identically for player movement, enemy movement, pathfinding, and attack reach. Owned by T1002/T1003.
3. Commands after victory/death, or commands with no valid effect, must not run another enemy phase or consume resources. Owned by T1003–T1005.
4. Empty sectors, a single-room floor, and dense population must still leave a valid start and reachable objective without overlapping entities. Owned by T1001/T1004/T1005.
5. Cancellation, input failure, and repeated cleanup must restore the terminal; controller/view operations must leave Model state and randomness untouched. Owned by T1002/T1006.

## Task execution pattern

For each gameplay tranche, first add the named deterministic regression cases, run the relevant test filter to see the expected failure, implement the smallest complete behavior, and rerun that filter. Then run the repository checks and commit the accepted tranche. Tests use explicit seed/terrain fixtures and behavioral assertions rather than duplicating the implementation.

Use test class names matching the filters below: `FoundationTests`, `WorldTests`, `MovementTests`, `ControllerTests`, `TerminalTests`, `CombatTests`, `TurnsTests`, `DeathTests`, `ItemsTests`, `HealingTests`, `PopulationTests`, `DescentTests`, `VictoryTests`, and `CompleteRunTests`. Put each named case in its corresponding class/file. A filtered run discovering zero cases is a verification failure.

## T1000 — Project and Model boundary

**Depends on:** Reviewed plan.  
**Files:** Root solution/build settings; the two projects; test projects; command, coordinate, rules, session, snapshot, and result files; view/input seams; initial build/CI files.

- [ ] Create the solution and project references: app → Model and Icod.DCurses; Model → .NET only. Pin the SDK and stable packages. Make only the application packable.
- [ ] Define the contracts above and a minimal fixed-level session fixture. New runs have depth 1, full health, starting equipment, zero potions, and `Playing` status.
- [ ] Add `NewRunStartsFresh` and `SnapshotCannotMutateModel`; assert depth/status/resources and prove that attempted snapshot changes cannot alter the session.
- [ ] Add a dependency check rejecting Icod.DCurses references in the Model project.
- [ ] Adopt the shared build/distribution conventions selectively. Local checks use Debug; PR validation uses Staging; main validation uses Release. Keep exactly one root solution. Prepare the application's `1.0.0-alpha.1` tool metadata, README/license inclusion, and tagged prerelease workflow before the first published alpha; T1007 strengthens distribution acceptance.
- [ ] Run `dotnet test Icod.LiteRogue.sln --filter FullyQualifiedName~Foundation`; require all foundation cases to pass. Run the build script and commit.

**Exit:** A headless Model can be driven and inspected through a stable application seam. No terminal is required by its tests.

## T1001 — Generation and visibility

**Depends on:** T1000.  
**Files:** `World/DungeonLevel.cs`, `DungeonGenerator.cs`, `Visibility.cs`; generation/visibility tests.

- [ ] Add `RoomsStayWithinSectors`, `AllWalkableTilesConnected`, `SingleRoomHasDistinctEntryAndObjective`, and `GenerationRepeatsForSeed`. Assert one room at most per sector, valid bounds, distinct reachable entry/exit, and identical terrain for repeated inputs.
- [ ] Implement the fixed dimensions, room occupancy, spanning-tree corridors, entry/exit reservations, and level-10 goal reservation.
- [ ] Add `RoomEntryRevealsInterior`, `ExploredTerrainPersists`, `HiddenEntitiesAreAbsent`, and `CorridorVisibilityStopsAtWalls`.
- [ ] Implement explored/currently-visible sets and filtered snapshots. Returning to an explored room must not expose creatures there before it becomes currently visible.
- [ ] Run `dotnet test Icod.LiteRogue.sln --filter FullyQualifiedName~World`; sweep seeds 0–999 at depths 1–10 for bounds, connectivity, and objective reachability. Print seed/depth on any failure and commit.

**Exit:** Procedural floors are reproducible, traversable, and presented with the approved fog behavior.

## T1002 — Movement and first DCurses application

**Depends on:** T1001.  
**Files:** `GameSession.cs`; Controller, View, and Terminal files; movement/controller/lifecycle tests; initial `docs/Playing.md`.

- [ ] Add `MovesInEightDirections`, `ClosedCornerBlocksMove`, `OneOpenSideAllowsDiagonal`, and `InvalidMoveDoesNotConsumeTurn`, including every map edge.
- [ ] Implement movement commands and a DCurses map/HUD/message renderer using the Model snapshot. Add ASCII-compatible glyphs and a visible legend.
- [ ] Implement the documented key mappings, help, quit, and `--seed <int>`, `--help`, `--version`. Invalid arguments report usage and exit before opening the terminal.
- [ ] Add `KeyMappingMatchesDirections`, `ResizeDoesNotChangeSnapshot`, `HelpDoesNotAdvanceTurn`, `QuitDisposesHost`, and `InputFailureDisposesHost` through app-owned fake view/input seams.
- [ ] Preserve map geometry at every terminal size; use a viewport or a size message when needed. Dispose terminal ownership on normal exit, cancellation, and failure.
- [ ] Run `dotnet test Icod.LiteRogue.sln --filter "FullyQualifiedName~Movement|FullyQualifiedName~Controller|FullyQualifiedName~Terminal"`; perform live move/resize/quit checks and commit.

**Alpha.1 gate:** Explore a generated floor in the terminal with stable fog, HUD, eight-direction input, and clean exit. Early help/release notes identify descent/combat as not yet implemented.

## T1003 — Combat, enemy turns, and death

**Depends on:** T1002.  
**Files:** `Actors/Enemy.cs`, `Combat.cs`, `World/LevelPopulation.cs`, `GameSession.cs`; combat/turn tests.

- [ ] Add `BumpAttackKeepsPlayerPosition`, `ArmorMitigatesWithMinimumOneDamage`, `ClosedCornerBlocksAttack`, and `EnemyUsesSameMovementRules`.
- [ ] Implement weapon/armor-based damage and legal adjacent melee attacks.
- [ ] Add `EachEnemyActsAtMostOnce`, `EnemiesNeverOverlap`, `RejectedCommandSkipsEnemyPhase`, and `DeathStopsRemainingEnemyActions`. Use fixed levels and stable enemy IDs.
- [ ] Implement a sequential enemy phase, visibility-based pursuit/simple wandering, and depth-based enemy tiers. Population must never block the player entry.
- [ ] Add `CommandsAfterDeathDoNothing`; implement the death screen and fresh-run action with a new seed and starting resources.
- [ ] Run `dotnet test Icod.LiteRogue.sln --filter "FullyQualifiedName~Combat|FullyQualifiedName~Turns|FullyQualifiedName~Death"`; live-test corridor fights and death; commit.

**Exit:** Combat and death obey the same Model rules in tests and in the application.

## T1004 — Equipment and potion healing

**Depends on:** T1003.  
**Files:** `Items/Equipment.cs`, `Inventory.cs`, `GameRules.cs`, `World/LevelPopulation.cs`, `GameSession.cs`; item/healing tests.

- [ ] Record exact initial enemy, weapon, armor, and depth-based loot tables in `GameRules` and the player guide. Maximum health remains 30 initially; potion healing remains 10 initially.
- [ ] Add `WalkingCollectsLoot`, `OnlyBetterEquipmentReplacesCurrentItem`, and `PotionPickupIncrementsCount`; implement automatic collection without an inventory menu.
- [ ] Add `PotionClampsToMaximum`, `DrinkingCostsOneTurn`, `EmptyInventoryRejectsDrink`, and `FullHealthRejectsDrink`. Assert health, potion count, and enemy-action count.
- [ ] Add `PopulationRespectsReservedTiles` and `DensePopulationRemainsValid`; cap population to available floor tiles instead of retrying without a bound.
- [ ] Render item messages and accurate resource totals. Add `InvalidDrinkDoesNotAdvanceRandomness`.
- [ ] Run `dotnet test Icod.LiteRogue.sln --filter "FullyQualifiedName~Items|FullyQualifiedName~Healing|FullyQualifiedName~Population"`; live-test pickup, replacement, potion use under attack, and death; commit.

**Alpha.2 gate:** A terminal run supports combat, loot, healing, and permanent death.

## T1005 — Ten-level descent and victory

**Depends on:** T1004.  
**Files:** `GameSession.cs`, generation/population; Controller/View outcome handling; descent/victory tests.

- [ ] Add `DescentRequiresStaircase`, `DescentPreservesResources`, `NewFloorStartsUnexplored`, and `NoAscentCommandExists`.
- [ ] Implement depth progression from 1 through 10. Drop prior-floor gameplay state, keeping run/player resources and reproducible seed handling.
- [ ] Add `TenthLevelHasGoalAndNoDownStaircase`, `GoalWinsBeforeEnemyPhase`, and `CommandsAfterVictoryDoNothing`.
- [ ] Implement the final destination, immediate victory, final screen, and fresh-run action. Enemy-free fixtures must complete all ten levels without a hidden combat requirement.
- [ ] Run `dotnet test Icod.LiteRogue.sln --filter "FullyQualifiedName~Descent|FullyQualifiedName~Victory|FullyQualifiedName~CompleteRun"`; complete a live win and loss, record seed/version, and commit.

**Alpha.3 gate:** Every approved gameplay system is integrated. A new run can complete from floor 1 to victory on floor 10.

## T1006 — Balance and hardening

**Depends on:** T1005.  
**Files:** Rules/content tables; Model/application regression tests; `docs/Terminal-Testing.md`; player guide and changelog.

- [ ] Repeat the seed/depth sweep with full population. Check objectives, entity overlap, nonnegative resources, and terminal outcomes.
- [ ] Add `SameSeedAndCommandsRepeatRun`, `RedrawDoesNotAdvanceRandomness`, and `SnapshotsCannotLeakHiddenEntities`.
- [ ] Add regressions for the five Review Focus conditions, including repeated host cleanup and recovery from a tiny terminal. Fix each observed failure before adding broader tests.
- [ ] Play a recorded set of at least ten varied seeds, including wins and deaths. Record loot scarcity, early unavoidable fights, damage spikes, and usefulness of exploration; tune the content tables without introducing new systems.
- [ ] Verify terminal behavior on Windows, Linux, and macOS, including at least one x64 and one ARM64 environment across the matrix. Record exact terminal/OS/build versions and limitations.
- [ ] Run the complete tests in Debug, Staging, and Release; commit the tuned tables and acceptance evidence.

**Alpha.4 gate:** The complete game has documented balance feedback and passes generation, gameplay, input, resize, and terminal-restoration checks.

## T1007 — Packaging, documentation, and release candidate

**Depends on:** T1006.  
**Files:** Application package metadata; release/build/packaging scripts and workflows; README, changelog, player guide, terminal guide, package license notices.

- [ ] Package one .NET tool: package ID `Icod.LiteRogue`, command `literogue`. Include the private Model assembly, pinned runtime dependencies, README, and the exact root license.
- [ ] Complete and harden the distribution workflow introduced at T1000. Validate PR Staging and main Release artifacts; tagged releases require the tag version to match package metadata and the tagged source to be contained in main.
- [ ] Produce and verify framework-dependent archives for `win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-x64`, and `osx-arm64`. Follow the shared archive convention and document the required .NET 10 runtime. Copy the player guide and license notices alongside the executable.
- [ ] Add clean-directory package installation checks. Run `dotnet tool install Icod.LiteRogue --version 1.0.0-rc.1 --add-source artifacts/packages --tool-path artifacts/tool-smoke`, then verify the installed `literogue --version` and `--help`.
- [ ] Verify archives contain the app and dependencies, version information, player documentation, GPL text and required dependency notices. Create checksums and test an extracted installation on each advertised platform.
- [ ] Finish README install/run examples, controls, glyph legend, potion/gear rules, victory/death/descent explanation, seed reporting, terminal requirements, uninstall instructions, and troubleshooting.
- [ ] Run `build.cmd` on Windows and `./build.sh` on Unix-like hosts; require clean/restore/build/test/pack/validate success. Record live launch, resize, quit, and terminal restoration from the actual packaged application.

**RC.1 gate:** Version `1.0.0-rc.1` is ready to publish as a prerelease. Freeze controls, distribution layout, and gameplay scope. Remaining changes fix defects or documentation.

## T1008 — Stable 1.0.0 closure

**Depends on:** Accepted RC and T1007.  
**Files:** Version metadata, README/changelog, roadmap status; release validation evidence.

- [ ] Resolve release-blocking defects; document accepted platform limitations and verify no core gameplay issue remains.
- [ ] Set package/application version to `1.0.0` and stable assembly version to `1.0.0.0`; verify `--version`, package metadata, archive names, and release notes agree.
- [ ] Rebuild and verify the final source commit in Release on the full six-RID matrix. Rerun clean-install smoke tests against the stable artifacts.
- [ ] Complete at least one final packaged victory and one packaged death; verify a new run resets resources and the terminal restores after exit.
- [ ] Mark the implementation tranches complete only after their evidence is accepted; retain alpha/RC history in the changelog.
- [ ] Present the final commit, successful checks, artifacts, and release notes for the maintainer's merge/tag/publication decision. Publish `v1.0.0` only when requested.

**Exit:** A stable release is reproducible, documented, installable, playable to either terminal outcome, and verified from its actual shipped artifacts.

## Verification commands

Once T1000 supplies the projects and scripts:

- `dotnet restore Icod.LiteRogue.sln`
- `dotnet build Icod.LiteRogue.sln -c Staging --no-restore`
- `dotnet test Icod.LiteRogue.sln -c Staging --no-build`
- `dotnet build Icod.LiteRogue.sln -c Release --no-restore`
- `dotnet test Icod.LiteRogue.sln -c Release --no-build`
- `dotnet pack src/Icod.LiteRogue/Icod.LiteRogue.csproj -c Release --no-build -o artifacts/packages`
- `build.cmd` or `./build.sh` for the complete local Debug sequence.

A passing command requires exit code 0, passing tests, and the expected package/artifact outputs. Live-terminal acceptance supplements automated tests; it is recorded rather than inferred from a successful headless build.

## Plan review and execution

Review the proposed defaults and tranche gates before implementation. Keep the design, this plan, and the main roadmap together. Implement one accepted tranche at a time, recording tests and playability at each checkpoint. No implementation or release is claimed by this planning PR.
