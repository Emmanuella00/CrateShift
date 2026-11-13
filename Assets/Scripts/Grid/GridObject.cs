using UnityEngine;

// =====================================================================
// GridObject - base class for everything that stands on a grid cell.
//
// ABSTRACTION: abstract class. Every object on the board has a grid
//   position and can be asked "can you be pushed?" - but HOW it answers
//   is left to each subclass.
// INHERITANCE: Wall, Crate and Player all extend GridObject and reuse its
//   position and movement code.
// POLYMORPHISM: TryPush() is abstract. MoveCommand calls it on whatever
//   object is in the way without knowing what it is: a Wall refuses,
//   a Crate slides if the cell behind it is free.
// ENCAPSULATION: GridPosition has a private setter - only Place() can
//   change it, so the logical position and the sprite never get out of sync.
// =====================================================================
public abstract class GridObject : MonoBehaviour
{
    [SerializeField] private float slideSpeed = 12f;   // how fast the sprite glides between cells

    private Vector3 targetWorldPosition;

    public Vector2Int GridPosition { get; private set; }

    // Puts the object on a cell. snap = true jumps there instantly (used when building a level)
    public void Place(Vector2Int cell, bool snap = false)
    {
        GridPosition = cell;
        targetWorldPosition = new Vector3(cell.x, cell.y, 0f);
        if (snap) transform.position = targetWorldPosition;
    }

    protected virtual void Update()
    {
        // Glide the sprite towards its cell (purely visual - the logic already moved)
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, slideSpeed * Time.deltaTime);
    }

    // POLYMORPHISM: "something is pushing you in this direction - do you move?"
    public abstract bool TryPush(Board board, Vector2Int direction);
}
