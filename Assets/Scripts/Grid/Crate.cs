using UnityEngine;

// =====================================================================
// Crate - the boxes the player pushes onto the targets.
//
// INHERITANCE: a Crate is a GridObject.
// POLYMORPHISM: its TryPush slides one cell IF the cell behind it is
//   free. It never pushes another crate (that's the classic rule).
// =====================================================================
public class Crate : GridObject
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite onGoalSprite;              // optional: different look on a target
    [SerializeField] private Color onGoalTint = new Color(0.55f, 1f, 0.55f);

    public bool IsOnGoal { get; private set; }

    public override bool TryPush(Board board, Vector2Int direction)
    {
        Vector2Int next = GridPosition + direction;
        if (!board.IsFree(next)) return false;   // wall, another crate, or off the board
        board.Move(this, next);
        return true;
    }

    // Called by Board whenever the crate lands on a cell
    public void SetOnGoal(bool onGoal)
    {
        IsOnGoal = onGoal;
        if (spriteRenderer == null) return;

        if (onGoalSprite != null)
        {
            spriteRenderer.sprite = onGoal ? onGoalSprite : normalSprite;
        }
        else
        {
            spriteRenderer.color = onGoal ? onGoalTint : Color.white;   // no sprite given: tint green
        }
    }
}
