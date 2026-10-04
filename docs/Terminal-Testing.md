# Terminal and release acceptance

Release candidate: **1.0.0**, promoted from the complete alpha.3 implementation. On October 4, 2026, the maintainer accepted the existing game for stable release as-is, retaining Icod.DCurses 2.2.0. Further human balance feedback and live platform coverage are follow-up work. Automated checks still apply to the exact 1.0.0 artifacts before publication. Acceptance evidence below distinguishes completed checks from follow-up testing.

## Automated evidence

- Model and application tests cover generation, visibility, eight-direction movement, closed corners, combat, sequential enemy turns, death, loot, healing, descent, victory, deterministic commands, immutable snapshots, help, resize, CLI validation and cleanup on input failure/cancellation.
- Stable promotion: all 104 Release tests pass locally. The candidate package declares 1.0.0; installation from its isolated local feed/cache reports `1.0.0` and passes `--help`. Both packaged README copies match the source exactly.
- Seeds 0–999 at all ten depths: one combined sweep checks connected terrain, sector bounds, reachable objectives, separated doors, one-cell-wide corridors, intact room walls, a solid outer border, reserved population tiles and no initial entity overlaps. Additional checks cover dense, sparse and minimum-size layouts and short passages between two rooms with facing walls.
- Debug shared build: clean, restore, build, test, pack, exact artifact validation.
- Staging and Release distribution procedures include clean local tool installation, help/version checks, archive creation and extracted executable checks. Pre-promotion CI run [37230672468](https://github.com/uniblab/Icod.LiteRogue/actions/runs/37230672468) passed Staging distribution checks on Windows/Linux/macOS, x64 and ARM64. These automated procedures apply again to the exact stable candidate and tagged source. A successful headless job does not prove live terminal behavior.
- One tool package contains the Model and pinned dependencies. Package checks require exact GPL and dependency license hashes, tool metadata, documentation and all runtime assemblies. Archives include a framework-dependent single-file app, player guide, README and license notices.

## Linux live evidence

The full victory/death traces and balance simulations below were recorded at alpha.3 before the later generator revisions. Their exact routes and outcomes are historical evidence. After those revisions, the installed tool was checked for prompt startup, immediate quit and terminal restoration; the current generator remains covered by the seed/depth sweep.

Environment: Debian 12 Linux x64; native pseudo-terminal using `TERM=xterm-256color`; .NET SDK 10.0.401/runtime 10.0.12; DCurses 2.2.0. Native pseudo-terminal checks exercise the real rendering/input/lifecycle adapter rather than a fake View.

The initial Unix input check revealed that `Console.OpenStandardInput()` waited for a newline. The adapter now supplies a FileStream over `/dev/stdin` through the supported Terminal/DCurses byte-service interface. Windows retains the standard DCurses session factory. No dependency source was modified.

Live acceptance checks include immediate movement/help/quit, a small-terminal prompt followed by enlargement, a complete seed-7 exploration trace ending in victory, seed-0 direct-descent trace ending in death, new-run reset, exit status and restoration of canonical/echo settings. These replay generated command traces; they are not human balance playtests. Both traces passed through a clean installed tool: 2,396 commands for the win and 491 for the death; exit 0 and canonical/echo restoration on both. Tiny-window recovery and fresh-run reset passed in the win trace.

## Automated balance simulations

Both policies use known generated terrain; exploration collects generated loot before the destination, while rushing heads directly for the destination. Both fight encountered foes and drink at 20 health or less. These omniscient simulations establish achievable wins and losses, not player difficulty.

| Seed | Explore | Rush |
| --- | --- | --- |
| 0 | Won; 28 HP | Died on 8 |
| 1 | Won; 21 HP | Won; 19 HP |
| 7 | Won; 25 HP | Won; 21 HP |
| 13 | Won; 21 HP | Died on 9 |
| 42 | Won; 26 HP | Won; 22 HP |
| 73 | Won; 25 HP | Died on 8 |
| 100 | Won; 25 HP | Died on 9 |
| 256 | Won; 25 HP | Won; 21 HP |
| 512 | Won; 22 HP | Died on 9 |
| 999 | Won; 25 HP | Died on 8 |

Exploration found the strongest gear and left 19–25 potions in these simulations. Rushing produced four wins and six deaths. Potion supply may be generous; retain the initial tables until human feedback can judge route knowledge, exploration cost and combat pressure. Enemies start at least four tiles from the entry, avoiding immediate arrival attacks. Record unavoidable opening fights and late damage spikes during human runs.

## Follow-up live platform coverage

| Platform | Automated checks | Live coverage |
| --- | --- | --- |
| Linux x64 | Build/test/install/archive | Native PTY; acceptance report in PR |
| Linux ARM64 | CI build/test/install/archive | Pending |
| Windows x64 / ARM64 | CI build/test/install/archive | Pending; Windows Terminal/ConHost keyboard, resize and restoration |
| macOS x64 / ARM64 | CI build/test/install/archive | Pending; Terminal.app/iTerm2 keyboard, resize and restoration |

For each live host record OS, CPU, terminal name/version, game/runtime version and seed. Test arrows, all diagonals (including keypad with Num Lock off), room/corridor fog, corridor fights, pickup, drinking under attack, death, victory and a fresh run. Shrink below 30x6 and enlarge again; the map and turn must stay unchanged. Quit normally, send end of input/interrupt and confirm the terminal cursor/echo/input modes recover. Repeat disposal to verify cleanup is idempotent.

Publish accepted versions only from source on main. The release workflow must match the package version exactly and pass all six archive jobs before registry publication. The prepared NuGet Trusted Publishing policy is for repository `uniblab/Icod.LiteRogue`, workflow `release.yaml`, environment `Release`; the default NuGet username is `uniblab`. No static NuGet API key is required.
