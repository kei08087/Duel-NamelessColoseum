using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject player;
    public GameObject enemy;
    public bool gameEnd = false;
    public int gameTimeSet;
    public int gameTime;
    public float TempoScale = 1;

    [SerializeField]
    private SkillsetBase playerSkillset;
    [SerializeField]
    private UITools uiTools;
    [SerializeField]
    private GameObject gameOverUI;
    private bool ownsPlayerSkillset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        SkillsetBase selectedSkillset = SceneManagering.Instance != null
            ? SceneManagering.Instance.playerSkillset
            : null;
        if (selectedSkillset != null)
            playerSkillset = selectedSkillset;

        if (playerSkillset == null)
        {
            Debug.LogError("Cannot start the match without a player skillset.");
            return;
        }

        // Each match owns its runtime skill instances, including when this scene
        // is started directly in the Editor from its serialized fallback asset.
        playerSkillset = Instantiate(playerSkillset);
        ownsPlayerSkillset = true;
        gameTime = gameTimeSet;
        EventManager.GameSet();
        EventManager.SpawnPlayer(playerSkillset);
        if (player == null || enemy == null)
        {
            Debug.LogError("The map must spawn both the player and the test opponent.");
            return;
        }

        EventManager.SetCamera(player);
        MobileCombatUI controls = gameObject.AddComponent<MobileCombatUI>();
        controls.Initialize(player.GetComponent<CombatInputSource>(), playerSkillset);
        StartCoroutine(gameTick());
    }

    public void OnEnable()
    {
        EventManager.GameEnd += endGame;
    }

    public void OnDisable()
    {
        EventManager.GameEnd -= endGame;
        
    }

    private void OnDestroy()
    {
        if(Instance==this)
            Instance = null;
        if (ownsPlayerSkillset && playerSkillset != null)
        {
            playerSkillset.ReleaseRuntimeSkills();
            Destroy(playerSkillset);
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

    public void endGame()
    {
        gameEnd = true;
        uiTools.openGroup(gameOverUI);
    }

    public IEnumerator gameTick()
    {
        while(!gameEnd&&gameTime>=0)
        {
            yield return new WaitForSeconds(1);
            gameTime--;
        }
        gameEnd = true;
    }
}
