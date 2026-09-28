using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum MatchResult { InProgress, PlayerWin, EnemyWin, Draw }

    private struct PendingDamage
    {
        public CharacterStatistics target;
        public DamageBlock damage;
    }

    public static GameManager Instance { get; private set; }
    public GameObject player;
    public GameObject enemy;
    public bool gameEnd = false;
    public int gameTimeSet;
    public int gameTime;
    public float TempoScale = 1;
    public MatchResult Result { get; private set; } = MatchResult.InProgress;
    private readonly List<PendingDamage> pendingDamage = new();
    private bool timeoutRequested;

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

    public void QueueDamage(CharacterStatistics target, DamageBlock damage)
    {
        if (!gameEnd && target != null)
            pendingDamage.Add(new PendingDamage { target = target, damage = damage });
    }

    // Resolve every hit gathered before this physics tick, then inspect both HP totals.
    private void FixedUpdate()
    {
        if (gameEnd || player == null || enemy == null)
            return;

        int count = pendingDamage.Count;
        for (int index = 0; index < count; index++)
        {
            PendingDamage hit = pendingDamage[index];
            if (hit.target != null)
                hit.target.ResolveDamage(hit.damage);
        }
        if (count > 0)
            pendingDamage.RemoveRange(0, count);

        CharacterStatistics playerStats = player.GetComponent<CharacterStatistics>();
        CharacterStatistics enemyStats = enemy.GetComponent<CharacterStatistics>();
        if (playerStats == null || enemyStats == null)
            return;

        bool playerDead = playerStats.hp <= 0f;
        bool enemyDead = enemyStats.hp <= 0f;
        if (playerDead && enemyDead)
            Conclude(MatchResult.Draw);
        else if (playerDead)
            Conclude(MatchResult.EnemyWin);
        else if (enemyDead)
            Conclude(MatchResult.PlayerWin);
        else if (timeoutRequested)
        {
            float playerRatio = playerStats.Mhp > 0f ? playerStats.hp / playerStats.Mhp : 0f;
            float enemyRatio = enemyStats.Mhp > 0f ? enemyStats.hp / enemyStats.Mhp : 0f;
            if (Mathf.Abs(playerRatio - enemyRatio) < 0.0001f)
                Conclude(MatchResult.Draw);
            else
                Conclude(playerRatio > enemyRatio ? MatchResult.PlayerWin : MatchResult.EnemyWin);
        }
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
            timeoutRequested = true;
    }
}
