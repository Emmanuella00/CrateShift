using System.Collections.Generic;
using UnityEngine;

// =====================================================================
// GridPathfinder - ALGORITHM 1: Breadth-First Search (BFS) pathfinding.
//
// Problem it solves: the player can CLICK any floor tile and walk there
//   automatically along the shortest route, around walls and crates
//   (it never pushes crates while auto-walking).
//
// How it works:
//   1. Put the start cell in a queue.
//   2. Take the next cell out of the queue and look at its 4 neighbours.
//   3. Every free neighbour not seen before is remembered with "I came
//      from <current cell>" and added to the back of the queue.
//   4. Repeat until the target is reached or the queue is empty.
//   5. Walk the "came from" links backwards from the target to the start
//      and reverse them -> the path.
//   Because BFS explores in rings of increasing distance, the FIRST time
//   it reaches the target it has found a SHORTEST path.
//
// Complexity: O(W x H) - each cell is visited at most once.
// =====================================================================
public static class GridPathfinder
{
    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    // Returns the cells from start to target (both included), or null if unreachable
    public static List<Vector2Int> FindPath(Board board, Vector2Int start, Vector2Int target)
    {
        if (start == target || !board.IsFree(target)) return null;

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        frontier.Enqueue(start);
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            if (current == target) break;

            foreach (Vector2Int direction in Directions)
            {
                Vector2Int next = current + direction;
                if (!board.IsFree(next) || cameFrom.ContainsKey(next)) continue;
                cameFrom[next] = current;
                frontier.Enqueue(next);
            }
        }

        if (!cameFrom.ContainsKey(target)) return null;   // walled off

        // Rebuild the path by following the "came from" links backwards
        List<Vector2Int> path = new List<Vector2Int>();
        for (Vector2Int p = target; p != start; p = cameFrom[p]) path.Add(p);
        path.Add(start);
        path.Reverse();
        return path;
    }
}
