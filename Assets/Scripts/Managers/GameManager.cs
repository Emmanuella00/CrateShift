using UnityEngine;

// =====================================================================
// GameManager - runs the puzzle: loads levels, applies moves, undo,
// restart, and decides when a level is solved.
//
// SINGLETON PATTERN: exactly one GameManager exists, reachable from any
//   script through GameManager.Instance (PlayerInput uses it).
// COMMAND PATTERN: every move becomes a MoveCommand run through
//   CommandHistory, which is what makes Undo possible.
// OBSERVER PATTERN (publisher): raises events (moves changed, level
//   loaded, level complete...). UIManager listens; GameManager never
//   touches the UI directly.
// ENCAPSULATION: level index, move count and history are private;
//   other scripts only get read-only properties and public actions.
// =====================================================================
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private LevelBuilder levelBuilder;

    [Tooltip("Leave empty to use the 3 built-in levels")]
    [SerializeField, TextArea(6, 14)] private string[] levels;

    // OBSERVER: events the UI subscribes to
    public event System.Action<int, int> OnLevelLoaded;     // (level number, total levels)
    public event System.Action<int> OnMovesChanged;         // (moves)
    public event System.Action<bool> OnDeadlockChanged;     // (is a crate stuck?)
    public event System.Action<int, int> OnLevelComplete;   // (level number, moves)
    public event System.Action OnAllLevelsComplete;

    private readonly CommandHistory history = new CommandHistory();
    private int levelIndex;
    private int moves;

    public Board Board { get; private set; }
    public Player Player { get; private set; }
    public bool IsLevelComplete { get; private set; }

    // Built-in levels (all tested to be solvable)
    public static readonly string[] DefaultLevels =
    {
        "#######\n" +
        "#     #\n" +
        "# @$ .#\n" +
        "#     #\n" +
        "#######",

        "########\n" +
        "#   .  #\n" +
        "# $ #  #\n" +
        "# @ $ .#\n" +
        "#      #\n" +
        "########",

        "#########\n" +
        "#   .   #\n" +
        "#  $#$  #\n" +
        "# . @ . #\n" +
        "#   $   #\n" +
        "#       #\n" +
        "#########",
    };

    private void Awake()
    {
        // SINGLETON: destroy any duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (levels == null || levels.Length == 0) levels = DefaultLevels;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        LoadLevel(0);
    }

    public void LoadLevel(int index)
    {
        levelIndex = index;
        Player player;
        Board = levelBuilder.Build(levels[index], out player);
        Player = player;

        history.Clear();
        moves = 0;
        IsLevelComplete = false;

        OnLevelLoaded?.Invoke(levelIndex + 1, levels.Length);
        OnMovesChanged?.Invoke(moves);
        OnDeadlockChanged?.Invoke(false);
    }

    // COMMAND PATTERN: wrap the move in a command and let the history run it
    public void TryMove(Vector2Int direction)
    {
        if (IsLevelComplete || Board == null) return;

        ICommand move = new MoveCommand(Board, Player, direction);
        if (!history.Execute(move)) return;   // blocked: nothing happens, nothing is recorded

        moves++;
        AfterBoardChanged();
    }

    // Buttons call these (no parameters, so they show up in the Button OnClick list)
    public void Undo()
    {
        if (IsLevelComplete || !history.Undo()) return;
        moves--;
        AfterBoardChanged();
    }

    public void Restart()
    {
        LoadLevel(levelIndex);
    }

    public void NextLevel()
    {
        if (levelIndex + 1 < levels.Length) LoadLevel(levelIndex + 1);
        else OnAllLevelsComplete?.Invoke();
    }

    public void PlayAgain()
    {
        LoadLevel(0);
    }

    private void AfterBoardChanged()
    {
        OnMovesChanged?.Invoke(moves);
        OnDeadlockChanged?.Invoke(DeadlockDetector.HasDeadlock(Board));   // ALGORITHM 2

        if (Board.IsComplete())
        {
            IsLevelComplete = true;
            OnLevelComplete?.Invoke(levelIndex + 1, moves);
        }
    }
}
