using System.Collections.Generic;

// =====================================================================
// CommandHistory - COMMAND PATTERN (invoker): runs commands and keeps a
// history so they can be undone.
//
// Uses a Stack (Last In, First Out): the most recent move is always the
// first one undone. Push and Pop are both O(1).
// ENCAPSULATION: the stack is private; you can only Execute, Undo or Clear.
// =====================================================================
public class CommandHistory
{
    private readonly Stack<ICommand> history = new Stack<ICommand>();

    public int Count { get { return history.Count; } }

    // Runs the command; only successful commands are remembered
    public bool Execute(ICommand command)
    {
        if (!command.Execute()) return false;
        history.Push(command);
        return true;
    }

    public bool Undo()
    {
        if (history.Count == 0) return false;
        history.Pop().Undo();
        return true;
    }

    public void Clear()
    {
        history.Clear();
    }
}
