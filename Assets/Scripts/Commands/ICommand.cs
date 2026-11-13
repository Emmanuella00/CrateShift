// =====================================================================
// ICommand - COMMAND PATTERN contract.
//
// Every player action is turned into an object that knows how to DO
// itself and how to UNDO itself. Because moves are objects, we can store
// them in a history and undo them in reverse order.
// ABSTRACTION: CommandHistory only knows this interface, not what a
//   "move" is - a future "teleport" or "swap" action would just be a new
//   class implementing ICommand.
// =====================================================================
public interface ICommand
{
    bool Execute();   // returns false if the action was impossible (e.g. walking into a wall)
    void Undo();
}
