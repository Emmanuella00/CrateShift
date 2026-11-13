using TMPro;
using UnityEngine;

// =====================================================================
// UIManager - shows level, moves, hints and the end-of-level panels.
//
// OBSERVER PATTERN (subscriber): listens to GameManager's events.
//   GameManager does not know the UI exists - it just raises events.
//   You could delete this script and the puzzle would still work.
// =====================================================================
public class UIManager : MonoBehaviour
{
    private const string DefaultHint = "Arrows / WASD: move   ·   Click a tile: walk there   ·   Z: undo   ·   R: restart";
    private const string StuckHint = "A crate is stuck in a corner! Press Z to undo.";

    [SerializeField] private GameManager gameManager;

    [Header("HUD")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text movesText;
    [SerializeField] private TMP_Text hintText;

    [Header("Panels")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TMP_Text levelCompleteText;
    [SerializeField] private GameObject allCompletePanel;

    private void OnEnable()
    {
        // OBSERVER: subscribe
        gameManager.OnLevelLoaded += HandleLevelLoaded;
        gameManager.OnMovesChanged += HandleMovesChanged;
        gameManager.OnDeadlockChanged += HandleDeadlockChanged;
        gameManager.OnLevelComplete += HandleLevelComplete;
        gameManager.OnAllLevelsComplete += HandleAllLevelsComplete;
    }

    private void OnDisable()
    {
        // OBSERVER: always unsubscribe
        gameManager.OnLevelLoaded -= HandleLevelLoaded;
        gameManager.OnMovesChanged -= HandleMovesChanged;
        gameManager.OnDeadlockChanged -= HandleDeadlockChanged;
        gameManager.OnLevelComplete -= HandleLevelComplete;
        gameManager.OnAllLevelsComplete -= HandleAllLevelsComplete;
    }

    private void HandleLevelLoaded(int level, int totalLevels)
    {
        levelText.text = "Level " + level + " / " + totalLevels;
        levelCompletePanel.SetActive(false);
        allCompletePanel.SetActive(false);
    }

    private void HandleMovesChanged(int moves)
    {
        movesText.text = "Moves: " + moves;
    }

    private void HandleDeadlockChanged(bool stuck)
    {
        hintText.text = stuck ? StuckHint : DefaultHint;
        hintText.color = stuck ? new Color(1f, 0.55f, 0.45f) : Color.white;
    }

    private void HandleLevelComplete(int level, int moves)
    {
        levelCompleteText.text = "Level " + level + " solved!\n" + moves + " moves";
        levelCompletePanel.SetActive(true);
    }

    private void HandleAllLevelsComplete()
    {
        levelCompletePanel.SetActive(false);
        allCompletePanel.SetActive(true);
    }
}
