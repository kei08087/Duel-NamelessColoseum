using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "WarriorBasicAttack", menuName = "Scriptable Objects/WarriorBasicAttack")]
public class WarriorBasicAttack : Skill
{
    

    public override void execute(Transform caster, SkillExecutor exc)
    {
        Vector3 origin = caster.transform.position;

        GameObject hitten = exc.DoOverlapCone(caster, origin, radius, angle, targetMask, damage);
        if (hitten)
        {
            var chctrl = caster.gameObject.GetComponent<CharacterControll>();
            if(chctrl.coolEnd.ContainsKey("DoubleSlash"))
            {
                chctrl.coolEnd["DoubleSlash"] -= 1;
                Debug.Log("Double Slash cooldown reduced");
            }
        }
    }
}
