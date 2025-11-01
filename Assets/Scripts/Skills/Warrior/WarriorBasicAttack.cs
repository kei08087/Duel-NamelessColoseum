using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "WarriorBasicAttack", menuName = "Scriptable Objects/WarriorBasicAttack")]
public class WarriorBasicAttack : Skill
{
    

    public override void execute(Transform caster, SkillExecutor exc)
    {
        Vector3 origin = caster.transform.position;

        exc.DoOverlapCone(caster, origin, radius, angle, targetMask, damage);
    }
}
