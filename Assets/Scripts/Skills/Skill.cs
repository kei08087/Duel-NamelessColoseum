using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill")]
public class Skill : ScriptableObject
{
    public string skillID = "Basic Attack";
    public float cooldown = 1f;
    public float delayFront = 0f;
    public float delayBack = 0.2f;
    public float damage = 12f;
    public LayerMask targetMask;

    public float radius = 0.9f;
    public float angle = 90f;

    public virtual void execute(Transform caster, SkillExecutor exc)
    {
    }
}
