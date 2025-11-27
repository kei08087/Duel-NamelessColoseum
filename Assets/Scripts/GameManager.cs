using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject player;
    public GameObject enemy;

    [SerializeField]
    private SkillsetBase playerSkillset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        playerSkillset = SceneManagering.Instance.playerSkillset;
        EventManager.GameSet();
        EventManager.SpawnPlayer(playerSkillset);
        EventManager.SetCamera(player);
    }

    private void OnDestroy()
    {
        if(Instance==this)
            Instance = null;
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
