# Icod.LiteRogue

A small, turn-based Rogue-style game rendered with Icod.DCurses and organized using Model-View-Controller.

Explore ten procedurally generated dungeon levels. Each level uses a 3x3 arrangement of possible rooms joined by corridors. Fight increasingly dangerous foes, collect better weapons and armor, and manage healing potions. Reach the designated destination on level 10 to win. Death ends the run, and descent is permanent.

The approved gameplay and architecture foundation is recorded in [the game design](docs/Icod.LiteRogue-Design.md).

The planned project structure consists of a game-model library and a console application containing the Controller and Icod.DCurses View.
