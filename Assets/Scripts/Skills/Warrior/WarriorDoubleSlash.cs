using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WarriorDoubleSlash", menuName = "Scriptable Objects/WarriorDoubleSlash")]
public class WarriorDoubleSlash : Skill
{
    public override void execute(Transform caster, SkillExecutor exc)
    {

        exc.executeCoroutine(doubleAttack(caster, exc));

    }

    public IEnumerator doubleAttack(Transform caster, SkillExecutor exc)
    {
        Vector3 origin = caster.transform.position;

        exc.DoOverlapCone(caster, origin, radius, angle, targetMask, damage);
        yield return new WaitForSeconds(0.3f);
        exc.DoOverlapCone(caster, origin, radius, angle, targetMask, damage);
    }
}
