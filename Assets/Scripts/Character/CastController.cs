using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum CastPhase
{
    Ready,
    Windup,
    Executing,
    Recovery
}

public class CastController : MonoBehaviour
{
    private CharacterStatistics stats;
    private SkillExecutor executor;
    private Coroutine activeCast;
    private string activeSlot;
    private Skill windupSkill;
    private float windupStartedAt;
    private bool earlyReleaseRequested;

    public CastPhase Phase { get; private set; } = CastPhase.Ready;
    public bool CanMove => Phase != CastPhase.Windup && !IsStunned;
    public bool CanTurn => (Phase == CastPhase.Ready ||
        (Phase == CastPhase.Windup && windupSkill != null && windupSkill.CanAimDuringWindup)) && !IsStunned;
    public bool IsStunned => stats != null && stats.Statuses != null && stats.Statuses.IsStunned;
    public readonly Dictionary<string, float> coolEnd = new();

    private void Awake()
    {
        stats = GetComponent<CharacterStatistics>();
        executor = GetComponent<SkillExecutor>();
    }

    private void Update()
    {
        if (Phase == CastPhase.Windup &&
            (stats == null || stats.hp <= 0f || GameManager.Instance == null || GameManager.Instance.gameEnd))
            CancelWindup();
    }

    private void OnDisable()
    {
        CancelWindup();
    }

    public bool TryCast(CombatCommand command)
    {
        if (IsStunned || stats == null || stats.hp <= 0f ||
            stats.skillSet == null || GameManager.Instance == null || GameManager.Instance.gameEnd)
            return false;

        if (Phase == CastPhase.Windup && command.Slot == activeSlot &&
            windupSkill != null && Time.time - windupStartedAt >= windupSkill.EarlyReleaseAfterSeconds)
        {
            earlyReleaseRequested = true;
            return true;
        }

        Skill skill = stats.skillSet.getSkill(command.Slot);
        if (skill == null)
            return false;
        bool cancelBasicRecovery = Phase == CastPhase.Recovery && activeSlot == "LClick" &&
            skill.CanCancelBasicRecovery;
        if ((Phase != CastPhase.Ready && !cancelBasicRecovery) ||
            (coolEnd.TryGetValue(command.Slot, out float end) && Time.time < end))
            return false;

        if (cancelBasicRecovery)
        {
            if (activeCast != null)
                StopCoroutine(activeCast);
            activeCast = null;
        }

        Vector3 direction = command.Direction;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(direction.normalized);

        Phase = CastPhase.Windup;
        activeSlot = command.Slot;
        windupSkill = skill;
        windupStartedAt = Time.time;
        earlyReleaseRequested = false;
        skill.OnWindupStart(transform);
        Coroutine started = StartCoroutine(Cast(skill, command.Slot));
        activeCast = Phase == CastPhase.Ready ? null : started;
        return true;
    }

    public void ApplyStun(float duration)
    {
        if (duration <= 0f) return;
        stats?.Statuses?.ApplyStun(duration);
        if (Phase == CastPhase.Windup)
            CancelWindup();
    }

    public void ReduceCooldown(string slot, float amount)
    {
        if (coolEnd.TryGetValue(slot, out float end))
            coolEnd[slot] = Mathf.Max(Time.time, end - Mathf.Max(0f, amount));
    }

    public void ResetCooldown(string slot)
    {
        coolEnd[slot] = Time.time;
    }

    private IEnumerator Cast(Skill skill, string slot)
    {
        basicModule timing = skill.basic;
        while (Time.time - windupStartedAt < timing.delayFront && !earlyReleaseRequested)
            yield return null;

        EndWindup();

        if (IsStunned || stats.hp <= 0f || GameManager.Instance == null || GameManager.Instance.gameEnd)
        {
            FinishCast();
            yield break;
        }

        Phase = CastPhase.Executing;
        float cooldown = slot == "LClick" ? stats.BasicAttackCooldown(timing.cooldown) : timing.cooldown;
        coolEnd[slot] = Time.time + cooldown;
        IEnumerator action = skill.ExecuteSequence(transform, executor);
        if (action != null)
        {
            try
            {
                while (action.MoveNext())
                    yield return action.Current;
            }
            finally
            {
                (action as IDisposable)?.Dispose();
            }
        }

        Phase = CastPhase.Recovery;
        if (timing.delayBack > 0f)
            yield return new WaitForSeconds(timing.delayBack);

        FinishCast();
    }

    private void FinishCast()
    {
        EndWindup();
        Phase = CastPhase.Ready;
        activeCast = null;
        activeSlot = null;
        earlyReleaseRequested = false;
    }

    private void EndWindup()
    {
        if (windupSkill == null)
            return;
        Skill skill = windupSkill;
        windupSkill = null;
        skill.OnWindupEnd(transform);
    }

    private void CancelWindup()
    {
        if (Phase != CastPhase.Windup)
            return;
        EndWindup();
        if (activeCast != null)
            StopCoroutine(activeCast);
        FinishCast();
    }
}
