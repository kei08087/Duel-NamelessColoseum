using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WarriorSmite", menuName = "Scriptable Objects/WarriorSmite")]
public class WarriorSmite : Skill
{
    [Header("Skill Uniques")]
    public ReduceSpeed processType;
    public float reduceAmount;
    public override void execute(Transform caster, SkillExecutor exc)
    {
        Vector3 origin = caster.transform.position;
        GameObject hitten = exc.DoOverlapCone(caster, origin, radius, angle, targetMask, damage);
        if(hitten)
        {
            exc.executeCoroutine(slowEffect(hitten));
        }
    }

    public IEnumerator slowEffect(GameObject hitten)
    {
        CharacterStatistics chstats = hitten.GetComponent<CharacterStatistics>();
        var template = processType ? ScriptableObject.Instantiate(processType) : ScriptableObject.CreateInstance<ReduceSpeed>();
        template.reducing = reduceAmount;
        template.skillName = skillID;
        chstats.assignModifier(template);
        yield return new WaitForSeconds(duration);
        chstats.unassignModifier(template);
        
    }
}
