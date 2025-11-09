using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WarriorShieldUp", menuName = "Scriptable Objects/WarriorShieldUp")]
public class WarriorShieldUp : Skill
{
    public ReduceDamage processType;
    public override void execute(Transform caster, SkillExecutor exc)
    {
        exc.executeCoroutine(shieldUp(caster));
    }

    public IEnumerator shieldUp(Transform caster)
    {
        CharacterStatistics chstats = caster.gameObject.GetComponent<CharacterStatistics>();

        var template = processType ? ScriptableObject.Instantiate(processType) : ScriptableObject.CreateInstance<ReduceDamage>();

        template.reducing=damage;
        template.skillName = skillID;
        chstats.assignModifier(template);
        yield return new WaitForSeconds(duration);
        chstats.unassignModifier(template);
    }


}
