// INHERITANCE: the Player is a GridObject (same position + movement code).
// POLYMORPHISM: nothing can push the player, so TryPush refuses.
// The player is moved by MoveCommand, not by being pushed.
public class Player : GridObject
{
    public override bool TryPush(Board board, UnityEngine.Vector2Int direction)
    {
        return false;
    }
}
