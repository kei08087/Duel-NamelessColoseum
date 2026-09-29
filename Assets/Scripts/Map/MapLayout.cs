using UnityEngine;

[CreateAssetMenu(fileName = "MapLayout", menuName = "Duel/Map Layout")]
public class MapLayout : ScriptableObject
{
    [SerializeField] private string[] rows;
    [SerializeField] private Vector3 playerSpawn;
    [SerializeField] private Vector3 enemySpawn;

    public int Width => rows == null || rows.Length == 0 || rows[0] == null ? 0 : rows[0].Length;
    public int Height => rows?.Length ?? 0;
    public Vector3 PlayerSpawn => playerSpawn;
    public Vector3 EnemySpawn => enemySpawn;

    // Rows are ordered by increasing world Z. H: high wall, L: low wall,
    // F: floor, W: water over floor. Each character is one world-metre cell.
    public char TileAt(int x, int z) => rows[z][x];

    public bool IsWaterAt(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x + 0.5f);
        int z = Mathf.FloorToInt(worldPosition.z + 0.5f);
        return x >= 0 && z >= 0 && x < Width && z < Height && TileAt(x, z) == 'W';
    }

    public bool IsValid(out string error)
    {
        error = null;
        if (Width < 3 || Height < 3)
            error = "layout must contain at least 3x3 cells";
        else
        {
            for (int z = 0; z < Height; z++)
            {
                if (rows[z] == null || rows[z].Length != Width)
                {
                    error = $"row {z} has the wrong width";
                    break;
                }
                for (int x = 0; x < Width; x++)
                    if (rows[z][x] != 'F' && rows[z][x] != 'W' && rows[z][x] != 'L' && rows[z][x] != 'H')
                    {
                        error = $"invalid tile at ({x}, {z})";
                        break;
                    }
                if (error != null)
                    break;
            }
            if (error == null && (!SpawnIsFloor(playerSpawn) || !SpawnIsFloor(enemySpawn)))
                error = "both spawn points must be on floor";
        }
        return error == null;
    }

    private bool SpawnIsFloor(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x);
        int z = Mathf.FloorToInt(position.z);
        return x > 0 && z > 0 && x < Width - 1 && z < Height - 1 && TileAt(x, z) == 'F';
    }
}
