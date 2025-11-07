using UnityEngine;

[CreateAssetMenu(fileName = "DebugingSkill_SelfDamage", menuName = "Scriptable Objects/DebugingSkill_SelfDamage")]
public class DebugingSkill_SelfDamage : Skill
{
    public override void execute(Transform caster, SkillExecutor exc)
    {
        caster.gameObject.GetComponent<CharacterStatistics>().TakeDamage(damage);
    }
    
}
