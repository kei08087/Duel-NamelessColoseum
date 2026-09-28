using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CastPhase
{
    Ready,
    Windup,
    Recovery
}

public class CastController : MonoBehaviour
{
    private CharacterStatistics stats;
    private SkillExecutor executor;
    private Coroutine activeCast;
    private float stunnedUntil;

    public CastPhase Phase { get; private set; } = CastPhase.Ready;
    public bool CanMove => Phase != CastPhase.Windup && !IsStunned;
    public bool CanTurn => Phase == CastPhase.Ready && !IsStunned;
    public bool IsStunned => Time.time < stunnedUntil;
    public readonly Dictionary<string, float> coolEnd = new();

    private void Awake()
    {
        stats = GetComponent<CharacterStatistics>();
        executor = GetComponent<SkillExecutor>();
    }

    public bool TryCast(CombatCommand command)
    {
        if (Phase != CastPhase.Ready || IsStunned || stats == null || stats.hp <= 0f ||
            stats.skillSet == null || GameManager.Instance == null || GameManager.Instance.gameEnd)
            return false;

        Skill skill = stats.skillSet.getSkill(command.Slot);
        if (skill == null || (coolEnd.TryGetValue(command.Slot, out float end) && Time.time < end))
            return false;

        Vector3 direction = command.Direction;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(direction.normalized);

        Phase = CastPhase.Windup;
        activeCast = StartCoroutine(Cast(skill, command.Slot));
        return true;
    }

    public void ApplyStun(float duration)
    {
        stunnedUntil = Mathf.Max(stunnedUntil, Time.time + Mathf.Max(0f, duration));
        if (Phase != CastPhase.Windup)
            return;

        if (activeCast != null)
            StopCoroutine(activeCast);
        activeCast = null;
        Phase = CastPhase.Ready;
    }

    public void ReduceCooldown(string slot, float amount)
    {
        if (coolEnd.TryGetValue(slot, out float end))
            coolEnd[slot] = Mathf.Max(Time.time, end - Mathf.Max(0f, amount));
    }

    private IEnumerator Cast(Skill skill, string slot)
    {
        basicModule timing = skill.basic;
        if (timing.delayFront > 0f)
            yield return new WaitForSeconds(timing.delayFront);

        if (IsStunned || stats.hp <= 0f || GameManager.Instance == null || GameManager.Instance.gameEnd)
        {
            FinishCast();
            yield break;
        }

        Phase = CastPhase.Recovery;
        skill.execute(transform, executor);
        coolEnd[slot] = Time.time + timing.cooldown;

        if (timing.delayBack > 0f)
            yield return new WaitForSeconds(timing.delayBack);

        FinishCast();
    }

    private void FinishCast()
    {
        Phase = CastPhase.Ready;
        activeCast = null;
    }
}
