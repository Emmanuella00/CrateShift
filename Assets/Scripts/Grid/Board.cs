using System.Collections.Generic;
using UnityEngine;

// =====================================================================
// Board - the logical grid: what stands on each cell and where the goals are.
// Plain C# class (not a MonoBehaviour).
//
// ENCAPSULATION: the grid arrays are private. Other scripts can only ask
//   questions (IsFree, IsWall, IsGoal) or move objects through Move(),
//   which keeps the array, the object's position and the crate's
//   "on goal" look all in sync in ONE place.
// =====================================================================
public class Board
{
    private readonly GridObject[,] cells;
    private readonly bool[,] goals;
    private readonly List<Crate> crates = new List<Crate>();
    private readonly List<Vector2Int> goalCells = new List<Vector2Int>();

    public int Width { get; private set; }
    public int Height { get; private set; }
    public IReadOnlyList<Crate> Crates { get { return crates; } }
    public IReadOnlyList<Vector2Int> GoalCells { get { return goalCells; } }

    public Board(int width, int height)
    {
        Width = width;
        Height = height;
        cells = new GridObject[width, height];
        goals = new bool[width, height];
    }

    public bool IsInside(Vector2Int p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < Width && p.y < Height;
    }

    public GridObject Get(Vector2Int p)
    {
        return IsInside(p) ? cells[p.x, p.y] : null;
    }

    // Empty floor the player or a crate could move onto
    public bool IsFree(Vector2Int p)
    {
        return IsInside(p) && cells[p.x, p.y] == null;
    }

    // Outside the board counts as a wall
    public bool IsWall(Vector2Int p)
    {
        return !IsInside(p) || cells[p.x, p.y] is Wall;
    }

    public bool IsGoal(Vector2Int p)
    {
        return IsInside(p) && goals[p.x, p.y];
    }

    public void AddGoal(Vector2Int p)
    {
        goals[p.x, p.y] = true;
        goalCells.Add(p);
    }

    // Used while building a level
    public void Add(GridObject obj, Vector2Int p)
    {
        cells[p.x, p.y] = obj;
        obj.Place(p, snap: true);
        if (obj is Crate crate)
        {
            crates.Add(crate);
            crate.SetOnGoal(IsGoal(p));
        }
    }

    // The ONLY way objects move during play
    public void Move(GridObject obj, Vector2Int to)
    {
        Vector2Int from = obj.GridPosition;
        cells[from.x, from.y] = null;
        cells[to.x, to.y] = obj;
        obj.Place(to);
        if (obj is Crate crate) crate.SetOnGoal(IsGoal(to));
    }

    // WIN CHECK: every crate is standing on a goal. O(c) for c crates.
    public bool IsComplete()
    {
        foreach (Crate crate in crates)
        {
            if (!crate.IsOnGoal) return false;
        }
        return crates.Count > 0;
    }
}
