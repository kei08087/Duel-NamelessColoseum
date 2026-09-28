using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill")]
public abstract class Skill : ScriptableObject
{
    [Header("Basic Attack Skill Stats")]
    public string skillID;
    public int skillLevel;

    public abstract basicModule basic { get; }
    public virtual bool CanCancelBasicRecovery => false;

    [Header("Layer Settings")]
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public abstract void init();
    public abstract void execute(Transform caster, SkillExecutor executor);
}
