using UnityEngine;

// =====================================================================
// DeadlockDetector - ALGORITHM 2: corner deadlock detection.
//
// Problem it solves: in box-pushing puzzles you can push a crate into a
//   corner where it can never move again. If that corner is not a goal,
//   the level can no longer be solved - but beginners don't notice and
//   keep playing. This check spots it and the UI tells the player to undo.
//
// How it works (for each crate):
//   1. Skip crates already on a goal (a crate in a goal corner is fine).
//   2. Is there a wall above OR below it?   -> it can't move vertically
//   3. Is there a wall left OR right of it? -> it can't move horizontally
//   4. Both true = the crate is stuck in a corner -> deadlock.
//
// Complexity: O(c) for c crates - 4 neighbour checks per crate.
// Limitation (good for the "improvements" oral question): it only catches
//   corner deadlocks, not crates stuck along a wall with no goal on it,
//   or two crates blocking each other.
// =====================================================================
public static class DeadlockDetector
{
    public static bool HasDeadlock(Board board)
    {
        foreach (Crate crate in board.Crates)
        {
            Vector2Int p = crate.GridPosition;
            if (board.IsGoal(p)) continue;

            bool blockedVertically = board.IsWall(p + Vector2Int.up) || board.IsWall(p + Vector2Int.down);
            bool blockedHorizontally = board.IsWall(p + Vector2Int.left) || board.IsWall(p + Vector2Int.right);

            if (blockedVertically && blockedHorizontally) return true;
        }
        return false;
    }
}
