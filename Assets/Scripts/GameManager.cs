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

    [SerializeField]
    private SkillsetBase playerSkillset;
    [SerializeField]
    private UITools uiTools;
    [SerializeField]
    private GameObject gameOverUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        playerSkillset = SceneManagering.Instance.playerSkillset;
        gameTime = gameTimeSet;
        EventManager.GameSet();
        EventManager.SpawnPlayer(playerSkillset);
        EventManager.SetCamera(player);
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
