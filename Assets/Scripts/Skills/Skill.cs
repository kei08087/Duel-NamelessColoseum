using System.Collections;
using UnityEngine;

public enum AttackWallPolicy
{
    BlockAllWalls,
    BlockHighWalls,
    IgnoreWalls
}

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill")]
public abstract class Skill : ScriptableObject
{
    [Header("Basic Attack Skill Stats")]
    public string skillID;
    public int skillLevel;

    public abstract basicModule basic { get; }
    public virtual bool CanCancelBasicRecovery => false;
    public virtual AttackWallPolicy WallPolicy => AttackWallPolicy.BlockAllWalls;

    [Header("Layer Settings")]
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public abstract void init();
    public abstract void execute(Transform caster, SkillExecutor executor);

    // Return an enumerator when the action itself takes time. Recovery starts
    // only after that enumerator completes; lasting buffs may run separately.
    public virtual IEnumerator ExecuteSequence(Transform caster, SkillExecutor executor)
    {
        execute(caster, executor);
        return null;
    }
}
