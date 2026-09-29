using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum MatchResult { InProgress, PlayerWin, EnemyWin, Draw }

    public static GameManager Instance { get; private set; }
    public GameObject player;
    public GameObject enemy;
    public bool gameEnd = false;
    public int gameTimeSet;
    public int gameTime;
    public float TempoScale = 1;
    public MatchResult Result { get; private set; } = MatchResult.InProgress;
    private readonly CombatAdjudicator adjudicator = new();
    public long CombatTick => adjudicator.TickIndex;
    private bool timeoutRequested;

    [SerializeField]
    private SkillsetBase playerSkillset;
    [SerializeField]
    private SkillsetBase enemySkillset;
    [SerializeField]
    private UITools uiTools;
    [SerializeField]
    private GameObject gameOverUI;
    private bool ownsPlayerSkillset;
    private bool ownsEnemySkillset;
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

        SkillsetBase selectedEnemySkillset = SceneManagering.Instance != null
            ? SceneManagering.Instance.enemySkillset
            : null;
        if (selectedEnemySkillset != null)
            enemySkillset = selectedEnemySkillset;

        if (playerSkillset == null)
        {
            Debug.LogError("Cannot start the match without a player skillset.");
            return;
        }

        // A missing opponent selection makes a mirror match; the scene can also
        // provide a different local test opponent in its serialized field.
        if (enemySkillset == null)
            enemySkillset = playerSkillset;

        // Both sides own separate skillset and skill instances, even in a mirror match.
        playerSkillset = Instantiate(playerSkillset);
        ownsPlayerSkillset = true;
        enemySkillset = Instantiate(enemySkillset);
        ownsEnemySkillset = true;
        gameTime = gameTimeSet;
        EventManager.GameSet();
        EventManager.SpawnCombatants(playerSkillset, enemySkillset);
        if (player == null || enemy == null)
        {
            Debug.LogError("The map must spawn both combatants.");
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
        if (ownsEnemySkillset && enemySkillset != null)
        {
            enemySkillset.ReleaseRuntimeSkills();
            Destroy(enemySkillset);
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

    public void QueueDamage(CharacterStatistics target, DamageBlock damage)
    {
        if (!gameEnd && target != null)
            adjudicator.QueueDamage(target, damage);
    }

    public void RequestTimeout() => timeoutRequested = true;

    // FixedUpdate is the single adjudication boundary for queued damage and death.
    private void FixedUpdate() => AdvanceCombatTick();

    public MatchResult AdvanceCombatTick()
    {
        if (gameEnd || player == null || enemy == null)
            return Result;

        CharacterStatistics playerStats = player.GetComponent<CharacterStatistics>();
        CharacterStatistics enemyStats = enemy.GetComponent<CharacterStatistics>();
        if (playerStats == null || enemyStats == null)
            return Result;

        MatchResult outcome = adjudicator.AdvanceTick(playerStats, enemyStats, timeoutRequested);
        if (outcome != MatchResult.InProgress)
            Conclude(outcome);
        return Result;
    }

    private void Conclude(MatchResult result)
    {
        if (gameEnd)
            return;
        Result = result;
        EventManager.EndTheGame();
    }

    public void endGame()
    {
        if (gameEnd)
            return;
        gameEnd = true;
        adjudicator.ClearPending();
        player?.GetComponent<CombatStatusController>()?.Clear();
        enemy?.GetComponent<CombatStatusController>()?.Clear();
        if (gameOverUI != null)
        {
            TMP_Text resultText = gameOverUI.GetComponentInChildren<TMP_Text>(true);
            if (resultText != null)
                resultText.text = Result switch
                {
                    MatchResult.PlayerWin => "Victory",
                    MatchResult.EnemyWin => "Defeat",
                    MatchResult.Draw => "Draw",
                    _ => "Game Over"
                };
            if (uiTools != null)
                uiTools.openGroup(gameOverUI);
        }
    }

    public IEnumerator gameTick()
    {
        while (!gameEnd && gameTime > 0)
        {
            yield return new WaitForSeconds(1);
            if (!gameEnd)
                gameTime--;
        }
        if (!gameEnd)
            RequestTimeout();
    }
}
