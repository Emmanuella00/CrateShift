using UnityEngine;

// =====================================================================
// MoveCommand - COMMAND PATTERN (concrete command): one step of the player.
//
// Execute: step into an empty cell, or push whatever is in the way.
//          It remembers whether it pushed a crate...
// Undo:    ...so it can put the player AND that crate back exactly.
// POLYMORPHISM: blocker.TryPush() - the command does not check "is this
//   a wall or a crate?"; each object decides for itself.
// =====================================================================
public class MoveCommand : ICommand
{
    private readonly Board board;
    private readonly Player player;
    private readonly Vector2Int direction;

    private GridObject pushedObject;   // remembered for Undo (null if nothing was pushed)

    public MoveCommand(Board board, Player player, Vector2Int direction)
    {
        this.board = board;
        this.player = player;
        this.direction = direction;
    }

    public bool Execute()
    {
        Vector2Int target = player.GridPosition + direction;
        if (!board.IsInside(target)) return false;

        GridObject blocker = board.Get(target);
        if (blocker != null)
        {
            if (!blocker.TryPush(board, direction)) return false;   // POLYMORPHISM
            pushedObject = blocker;
        }

        board.Move(player, target);
        return true;
    }

    public void Undo()
    {
        // Reverse order of Execute: player steps back first, then the crate follows into its old cell
        board.Move(player, player.GridPosition - direction);
        if (pushedObject != null)
        {
            board.Move(pushedObject, pushedObject.GridPosition - direction);
        }
    }
}
