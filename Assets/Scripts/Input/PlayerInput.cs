using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// =====================================================================
// PlayerInput - reads the keyboard and mouse and turns them into actions.
//
// It never moves anything itself: it asks the GameManager (SINGLETON),
// which turns each step into a MoveCommand (COMMAND PATTERN).
// Mouse click: asks GridPathfinder (ALGORITHM 1, BFS) for the shortest
//   route, then walks it one step at a time - each step is still a
//   normal MoveCommand, so every step can be undone with Z.
// =====================================================================
public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float stepDelay = 0.09f;   // pause between auto-walk steps

    private readonly Queue<Vector2Int> plannedSteps = new Queue<Vector2Int>();
    private float nextStepTime;

    private void Update()
    {
        GameManager game = GameManager.Instance;
        if (game == null || game.Board == null) return;

        if (game.IsLevelComplete)
        {
            plannedSteps.Clear();
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) game.NextLevel();
            return;
        }

        Vector2Int direction = ReadDirection();
        if (direction != Vector2Int.zero)
        {
            plannedSteps.Clear();          // keyboard overrides any click-walk
            game.TryMove(direction);
        }

        if (Input.GetKeyDown(KeyCode.Z)) { plannedSteps.Clear(); game.Undo(); }
        if (Input.GetKeyDown(KeyCode.R)) { plannedSteps.Clear(); game.Restart(); }

        if (Input.GetMouseButtonDown(0)) PlanWalkTo(game);

        // Walk the planned path one step at a time
        if (plannedSteps.Count > 0 && Time.time >= nextStepTime)
        {
            game.TryMove(plannedSteps.Dequeue());
            nextStepTime = Time.time + stepDelay;
        }
    }

    private Vector2Int ReadDirection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) return Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) return Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) return Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) return Vector2Int.right;
        return Vector2Int.zero;
    }

    private void PlanWalkTo(GameManager game)
    {
        // Ignore clicks on UI buttons
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int target = new Vector2Int(Mathf.RoundToInt(world.x), Mathf.RoundToInt(world.y));

        List<Vector2Int> path = GridPathfinder.FindPath(game.Board, game.Player.GridPosition, target);   // ALGORITHM 1
        if (path == null) return;

        // Turn the list of cells into a list of single steps (directions)
        plannedSteps.Clear();
        for (int i = 1; i < path.Count; i++) plannedSteps.Enqueue(path[i] - path[i - 1]);
        nextStepTime = Time.time;
    }
}
