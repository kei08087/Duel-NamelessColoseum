using System.Collections.Generic;

// One authority for damage and victory. Hits queued during resolution wait for the next tick.
public sealed class CombatAdjudicator
{
    private struct PendingDamage
    {
        public CharacterStatistics target;
        public DamageBlock damage;
    }

    private readonly List<PendingDamage> pendingDamage = new();
    public long TickIndex { get; private set; }

    public void QueueDamage(CharacterStatistics target, DamageBlock damage)
    {
        if (target != null)
            pendingDamage.Add(new PendingDamage { target = target, damage = damage });
    }

    public GameManager.MatchResult AdvanceTick(CharacterStatistics player,
        CharacterStatistics enemy, bool timeoutRequested)
    {
        TickIndex++;
        player.GetComponent<CombatStatusController>()?.AdvanceTick(TickIndex);
        enemy.GetComponent<CombatStatusController>()?.AdvanceTick(TickIndex);

        int count = pendingDamage.Count;
        for (int index = 0; index < count; index++)
        {
            PendingDamage hit = pendingDamage[index];
            if (hit.target != null)
                hit.target.ResolveDamage(hit.damage);
        }
        if (count > 0)
            pendingDamage.RemoveRange(0, count);

        bool playerDead = player.hp <= 0f;
        bool enemyDead = enemy.hp <= 0f;
        if (playerDead)
            player.GetComponent<CombatStatusController>()?.Clear();
        if (enemyDead)
            enemy.GetComponent<CombatStatusController>()?.Clear();

        if (playerDead && enemyDead)
            return GameManager.MatchResult.Draw;
        if (playerDead)
            return GameManager.MatchResult.EnemyWin;
        if (enemyDead)
            return GameManager.MatchResult.PlayerWin;
        if (!timeoutRequested)
            return GameManager.MatchResult.InProgress;

        float playerRatio = player.Mhp > 0f ? player.hp / player.Mhp : 0f;
        float enemyRatio = enemy.Mhp > 0f ? enemy.hp / enemy.Mhp : 0f;
        if (UnityEngine.Mathf.Abs(playerRatio - enemyRatio) < 0.0001f)
            return GameManager.MatchResult.Draw;
        return playerRatio > enemyRatio
            ? GameManager.MatchResult.PlayerWin : GameManager.MatchResult.EnemyWin;
    }

    public void ClearPending() => pendingDamage.Clear();
}
