# Crate Shift

A 2D box-pushing puzzle made in **Unity 6** for the *Mini Game: Principles of OOP and Design Patterns* assignment.
Push every crate onto a red target to solve the level. Three levels, unlimited undo, click-to-walk, and a warning when a crate gets stuck.

## How to run

1. Install **Unity Hub** and **Unity 6** (6000.x).
2. Clone this repository:

3. In Unity Hub, click **Add → Add project from disk** and select the cloned folder.
4. Open the scene **Assets/Scenes/CrateShiftScene** and press **Play**.



## Controls

| Action | Key |
|---|---|
| Move / push | Arrow keys or WASD |
| Walk to a square | Left-click it (shortest path) |
| Undo | Z (or the Undo button) |
| Restart level | R (or the Restart button) |
| Next level | Enter or Space (or the button) |

## Project structure

```
Assets/Scripts/
├── Grid/         GridObject (abstract), Wall, Crate, Player, Board, LevelBuilder
├── Commands/     ICommand, MoveCommand, CommandHistory
├── Algorithms/   GridPathfinder (BFS), DeadlockDetector
├── Managers/     GameManager, UIManager
└── Input/        PlayerInput
```

## OOP principles, design patterns and algorithms

Every script starts with a comment explaining which principle, pattern or algorithm it shows.

| Concept | Where |
|---|---|
| **Encapsulation** | `Board` (private grid, `Move()` is the only way to move objects), `GridObject.GridPosition` (private setter) |
| **Abstraction** | `GridObject` (abstract class), `ICommand` (interface) |
| **Inheritance** | `Wall`, `Crate`, `Player` extend `GridObject` |
| **Polymorphism** | `TryPush()`: walls refuse, crates slide |
| **Command pattern** | `ICommand`, `MoveCommand`, `CommandHistory` (undo) |
| **Singleton pattern** | `GameManager.Instance` |
| **Observer pattern** | `GameManager` events → `UIManager` |
| Factory (bonus) and generics | `LevelBuilder.CreateObject()`, `Track<T>()` |
| **Algorithm 1: BFS pathfinding** | `GridPathfinder.FindPath()`, O(width × height) |
| **Algorithm 2: Deadlock detection** | `DeadlockDetector.HasDeadlock()`, O(number of crates) |

## Adding a level

Levels are text. Add one to **GameManager → Levels** in the Inspector (if the list is empty, the 3 built-in levels are used):

```
#######
#     #
# @$ .#
#     #
#######
```

`#` wall · `@` player · `$` crate · `.` target · `*` crate on target · `+` player on target

## Credits

- Art: [Kenney – Sokoban pack](https://kenney.nl/assets/sokoban), licensed **CC0** (public domain).
- Made by [Your name].
