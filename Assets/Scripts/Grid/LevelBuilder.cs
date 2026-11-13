using System.Collections.Generic;
using UnityEngine;

// =====================================================================
// LevelBuilder - turns a level written as text into a playable board.
//
// FACTORY (bonus pattern): CreateObject() takes one character and returns
//   the right kind of GridObject. The rest of the game never calls
//   Instantiate for walls, crates or the player itself.
//
// Level text uses the standard box-pushing notation:
//   #  wall          @  player          $  crate
//   .  goal          +  player on goal  *  crate on goal
//   (space) floor
// =====================================================================
public class LevelBuilder : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Wall wallPrefab;
    [SerializeField] private Crate cratePrefab;
    [SerializeField] private Player playerPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject goalPrefab;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float cameraPadding = 1f;

    private readonly List<GameObject> spawned = new List<GameObject>();

    public Board Build(string levelText, out Player player)
    {
        Clear();

        string[] rows = SplitRows(levelText);
        int height = rows.Length;
        int width = 0;
        foreach (string row in rows) width = Mathf.Max(width, row.Length);

        Board board = new Board(width, height);
        player = null;

        for (int r = 0; r < height; r++)
        {
            for (int x = 0; x < width; x++)
            {
                char symbol = x < rows[r].Length ? rows[r][x] : ' ';
                Vector2Int cell = new Vector2Int(x, height - 1 - r);   // row 0 is the TOP of the screen

                if (symbol != '#') Spawn(floorPrefab, cell);
                if (symbol == '.' || symbol == '*' || symbol == '+')
                {
                    board.AddGoal(cell);
                    Spawn(goalPrefab, cell);
                }

                GridObject obj = CreateObject(symbol);   // FACTORY
                if (obj == null) continue;

                board.Add(obj, cell);
                if (obj is Player p) player = p;
            }
        }

        FitCamera(width, height);
        return board;
    }

    // FACTORY METHOD: one character in, the right object out
    private GridObject CreateObject(char symbol)
    {
        switch (symbol)
        {
            case '#': return Track(Instantiate(wallPrefab, transform));
            case '$':
            case '*': return Track(Instantiate(cratePrefab, transform));
            case '@':
            case '+': return Track(Instantiate(playerPrefab, transform));
            default: return null;
        }
    }

    // GENERICS: works for Wall, Crate and Player alike
    private T Track<T>(T obj) where T : Component
    {
        spawned.Add(obj.gameObject);
        return obj;
    }

    private void Spawn(GameObject prefab, Vector2Int cell)
    {
        if (prefab == null) return;
        GameObject tile = Instantiate(prefab, new Vector3(cell.x, cell.y, 0f), Quaternion.identity, transform);
        spawned.Add(tile);
    }

    // Removes the previous level
    private void Clear()
    {
        foreach (GameObject obj in spawned)
        {
            if (obj != null) Destroy(obj);
        }
        spawned.Clear();
    }

    // Centres the camera on the board and zooms so the whole level fits
    private void FitCamera(int width, int height)
    {
        if (cam == null) return;
        cam.transform.position = new Vector3((width - 1) / 2f, (height - 1) / 2f, -10f);
        cam.orthographicSize = Mathf.Max(height, width / cam.aspect) / 2f + cameraPadding;
    }

    // Splits the text into rows and drops empty lines at the start and end
    private static string[] SplitRows(string levelText)
    {
        string[] lines = levelText.Replace("\r", "").Split('\n');
        int first = 0;
        int last = lines.Length - 1;
        while (first <= last && lines[first].Trim().Length == 0) first++;
        while (last >= first && lines[last].Trim().Length == 0) last--;

        string[] rows = new string[last - first + 1];
        for (int i = 0; i < rows.Length; i++) rows[i] = lines[first + i];
        return rows;
    }
}
