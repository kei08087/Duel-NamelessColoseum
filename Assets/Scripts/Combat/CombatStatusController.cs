using System.Collections.Generic;
using UnityEngine;

public interface ICombatStatusEffect
{
    void Apply(CharacterStatistics target, long tick);
    void OnTick(CharacterStatistics target, long tick);
    void Remove(CharacterStatistics target);
}

// Owns timed effects for one combatant. The match adjudication tick advances them.
public class CombatStatusController : MonoBehaviour
{
    private struct ActiveStatus
    {
        public ICombatStatusEffect effect;
        public long expiresAtTick;
    }

    private readonly List<ActiveStatus> active = new();
    private CharacterStatistics stats;
    private long currentTick;
    private long stunExpiresAtTick;

    public int ActiveCount => active.Count;
    public bool IsStunned => currentTick < stunExpiresAtTick;

    public static long DurationTicks(float seconds) =>
        Mathf.Max(1, Mathf.CeilToInt(Mathf.Max(0f, seconds) / Time.fixedDeltaTime));

    public void ApplyTimed(ICombatStatusEffect effect, float durationSeconds)
    {
        if (effect == null || durationSeconds <= 0f)
            return;
        if (stats == null) stats = GetComponent<CharacterStatistics>();
        if (stats == null || stats.hp <= 0f ||
            (GameManager.Instance != null && GameManager.Instance.gameEnd))
            return;

        currentTick = GameManager.Instance != null ? GameManager.Instance.CombatTick : currentTick;
        effect.Apply(stats, currentTick);
        active.Add(new ActiveStatus
        {
            effect = effect,
            expiresAtTick = currentTick + DurationTicks(durationSeconds)
        });
    }

    public void ApplyStun(float durationSeconds)
    {
        if (durationSeconds <= 0f)
            return;
        currentTick = GameManager.Instance != null ? GameManager.Instance.CombatTick : currentTick;
        stunExpiresAtTick = System.Math.Max(stunExpiresAtTick,
            currentTick + DurationTicks(durationSeconds));
    }

    public void AdvanceTick(long tick)
    {
        currentTick = tick;
        if (stats == null) stats = GetComponent<CharacterStatistics>();
        if (stats == null || stats.hp <= 0f)
        {
            Clear();
            return;
        }

        for (int index = active.Count - 1; index >= 0; index--)
        {
            ActiveStatus entry = active[index];
            entry.effect.OnTick(stats, tick);
            if (tick >= entry.expiresAtTick)
            {
                entry.effect.Remove(stats);
                active.RemoveAt(index);
            }
        }
    }

    public void Clear()
    {
        if (stats == null) stats = GetComponent<CharacterStatistics>();
        for (int index = active.Count - 1; index >= 0; index--)
        {
            if (stats != null)
                active[index].effect.Remove(stats);
        }
        active.Clear();
        stunExpiresAtTick = currentTick;
    }

    private void OnDisable() => Clear();
}
