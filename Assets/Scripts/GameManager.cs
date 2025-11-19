using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject player;
    public GameObject enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    bool GameStart = false;
    void Update()
    {
        if (!GameStart)
        {
            GameStart= true;
            EventManager.GameSet();
            EventManager.SpawnPlayer();
            EventManager.SetCamera(player);
        }
    }

    public void RegisterPlayer(GameObject player)
    {
        this.player = player;
    }
    public void RegisterEnemy(GameObject enemy)
    {
        this.enemy = enemy;
    }
}
