using UnityEngine;

public class MapGenerater : MonoBehaviour
{
    [SerializeField] private MapLayout layout;
    private Material waterMaterial;

    public MapLayout Layout => layout;
    public static MapGenerater Active { get; private set; }

    private void OnEnable()
    {
        Active = this;
        EventManager.GameSetup += CreateMap;
    }

    private void OnDisable()
    {
        EventManager.GameSetup -= CreateMap;
        if (Active == this)
            Active = null;
    }

    private void OnDestroy()
    {
        if (waterMaterial != null)
            Destroy(waterMaterial);
    }

    private void CreateMap() => Build(layout);

    public bool Build(MapLayout selectedLayout)
    {
        string error;
        if (selectedLayout == null)
            error = "no layout assigned";
        else if (!selectedLayout.IsValid(out error))
            error = error ?? "invalid layout";
        else
            error = null;
        if (error != null)
        {
            Debug.LogError("Cannot build map: " + error, this);
            return false;
        }

        GameObject floor = LoadBlock(MapEnum.Basic_Floor);
        GameObject lowWall = LoadBlock(MapEnum.Basic_Low_Wall);
        GameObject highWall = LoadBlock(MapEnum.Basic_High_Wall);
        GameObject playerSpawn = LoadBlock(MapEnum.SpawnerPlayer);
        GameObject enemySpawn = LoadBlock(MapEnum.SpawnerEnemy);
        if (floor == null || lowWall == null || highWall == null || playerSpawn == null || enemySpawn == null)
        {
            Debug.LogError("A map block or spawner prefab is missing.", this);
            return false;
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        layout = selectedLayout;
        for (int z = 0; z < layout.Height; z++)
        {
            for (int x = 0; x < layout.Width; x++)
            {
                char tile = layout.TileAt(x, z);
                Vector3 position = new Vector3(x, tile == 'F' || tile == 'W' ? 0.5f : tile == 'L' ? 1f : 1.5f, z);
                GameObject block = tile == 'H' ? highWall : tile == 'L' ? lowWall : floor;
                Instantiate(block, position, Quaternion.identity, transform);
                if (tile == 'W')
                    AddWaterSurface(x, z);
            }
        }

        Instantiate(playerSpawn, layout.PlayerSpawn, Quaternion.identity, transform);
        Instantiate(enemySpawn, layout.EnemySpawn, Quaternion.identity, transform);
        return true;
    }

    private static GameObject LoadBlock(MapEnum block) =>
        Resources.Load<GameObject>("Prefabs/MapBlocks/" + block);

    private void AddWaterSurface(int x, int z)
    {
        GameObject surface = GameObject.CreatePrimitive(PrimitiveType.Cube);
        surface.name = "WaterSurface";
        surface.layer = LayerMask.NameToLayer("Water");
        surface.transform.SetParent(transform, false);
        surface.transform.position = new Vector3(x, 1.01f, z);
        surface.transform.localScale = new Vector3(1f, 0.02f, 1f);
        surface.GetComponent<Collider>().enabled = false;

        if (waterMaterial == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Unlit/Color");
            waterMaterial = new Material(shader) { color = new Color(0.15f, 0.45f, 0.8f) };
        }
        surface.GetComponent<Renderer>().sharedMaterial = waterMaterial;
    }
}
