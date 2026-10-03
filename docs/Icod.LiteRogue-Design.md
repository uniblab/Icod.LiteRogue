# Icod.LiteRogue

**Status:** Gameplay and architecture foundation  
**Date:** October 3, 2026

**Repository:** [uniblab/Icod.LiteRogue](https://github.com/uniblab/Icod.LiteRogue)

## Premise

A single adventurer descends through ten procedurally generated dungeon levels. Each level contains rooms, connecting corridors, hostile creatures, and useful loot. Reaching a designated destination on level 10 wins the game. Death ends the run.

The central decision is whether to risk further exploration for equipment and healing potions or descend with the resources already found.

## Dungeon Layout

- Each dungeon level is divided into nine sections arranged in a 3x3 grid.
- Each section contains zero or one room.
- Room size and position vary within the section.
- Corridors connect the rooms and may pass through sections containing no room.
- Every generated level contains at least one room.
- Every room is reachable from every other room through traversable terrain.
- Levels 1 through 9 contain a reachable downward staircase.
- Level 10 contains a reachable victory destination, represented by a throne, fountain, or another distinctive landmark.

Descending permanently leaves the previous level behind. Health, equipment, and carried potions accompany the adventurer to the next level. The new level begins with unexplored terrain.

Each new run creates a fresh dungeon. The terrain of the current level remains stable during play.

## Exploration and Visibility

- Entering a room reveals its interior.
- Corridors become visible as the adventurer travels through them.
- Explored terrain remains on the map.
- Enemies and loot appear only when currently visible.

The player must discover the staircase and decide how much of the remaining level is worth exploring.

## Movement and Turns

The adventurer and enemies move in eight directions: north, northeast, east, southeast, south, southwest, west, and northwest.

Walls block movement. Diagonal movement cannot squeeze between two touching wall corners.

Gameplay is turn-based. The player takes an action and enemies then respond. Moving, attacking, and drinking a potion are gameplay actions. Display operations such as resizing and redrawing do not advance the game.

## Combat and Enemies

- Moving into an adjacent enemy makes a melee attack.
- Weapons affect damage.
- Armor reduces incoming damage.
- Position matters: narrow corridors and doorways limit how many enemies can approach and attack at once.
- Early levels contain weaker foes.
- Later levels introduce stronger foes and a variety of opponents.

Defeating every enemy is not required to descend or win. Survival and reaching the final destination are the objectives.

## Equipment and Healing

Advancement comes entirely from equipment. The adventurer's maximum health remains fixed throughout the run.

The inventory consists of:

- One equipped weapon.
- One equipped suit of armor.
- A count of carried healing potions.

Walking onto loot collects it. Better weapons and armor automatically replace the currently equipped items.

Healing potions are the sole means of restoring health. Drinking a potion consumes a turn, so the player must account for nearby enemies when choosing to heal.

Equipment upgrades and potion availability make exploration useful throughout the descent.

## Victory and Death

Reaching the designated destination on level 10 immediately wins the game.

If health reaches zero, the adventurer dies and the run ends. A new attempt starts on level 1 with starting equipment and a freshly generated dungeon.

## MVC Architecture

The game uses Model-View-Controller. Icod.DCurses provides rendering through the View.

| Component | Responsibilities |
| --- | --- |
| Model | Dungeon generation; terrain; player and enemies; movement; combat; equipment and potions; health; visibility and exploration; turn progression; descent; death and victory. |
| View | Render the dungeon, currently visible creatures and loot, status information, messages, and death/victory screens through Icod.DCurses; handle display layout and resizing. |
| Controller | Receive input, translate it into game commands, submit commands to the Model, coordinate the application loop, and request display updates. |

### Model

The Model owns all gameplay rules and state. It determines whether movement is legal, resolves attacks and equipment upgrades, advances enemies, computes visibility, and decides whether the run has ended.

The Model has no dependency on Icod.DCurses or terminal input. Its state can be exercised and inspected without opening a terminal.

It supplies read-only presentation data describing explored terrain, currently visible entities, player status, and action results. The View uses this data without modifying game state.

### View

The View translates presentation data into text graphics using Icod.DCurses. It displays the dungeon and the adventurer's health, weapon, armor, potion count, and current dungeon depth.

It also displays short action messages and the final win or death state.

Rendering does not change health, move creatures, reveal terrain, or advance turns. Resizing changes the display arrangement while preserving the dungeon's geometry and state.

### Controller

The Controller translates input into commands such as move, drink potion, descend, and quit.

It coordinates this sequence:

1. Receive a player command.
2. Submit the command to the Model.
3. Let the Model resolve the action and advance enemies when appropriate.
4. Obtain the updated presentation data and action messages.
5. Ask the View to render the result.

The Model remains responsible for action legality, turn consumption, visibility, and win/loss decisions.

### Project Organization

Use a game-model library plus a console application containing the Controller and the Icod.DCurses View.

The library provides the game state and rules. The application supplies keyboard interaction, presentation, and the application lifecycle.

## Verification Goals

Verification should focus on the game's defining rules:

- Generated rooms fit their sections and all rooms are connected.
- Every starting position can reach the staircase or final destination.
- Eight-direction movement respects walls and blocked corners.
- Exploration persists while creatures and loot obey current visibility.
- Weapons, armor, automatic upgrades, and healing potions affect the Model correctly.
- Player actions and enemy responses follow the turn sequence.
- Descent preserves player resources and advances to the next level.
- Reaching the final destination wins; reaching zero health ends the run.
- Display refreshes and terminal resizing preserve gameplay state.

Generation should be checked across many reproducible random layouts. The Model's rules should be verified independently of the terminal, with separate checks for the application's input and display behavior.
