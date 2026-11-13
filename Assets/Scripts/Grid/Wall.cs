// INHERITANCE: a Wall is a GridObject.
// POLYMORPHISM: its version of TryPush always refuses - walls never move.
public class Wall : GridObject
{
    public override bool TryPush(Board board, UnityEngine.Vector2Int direction)
    {
        return false;
    }
}
