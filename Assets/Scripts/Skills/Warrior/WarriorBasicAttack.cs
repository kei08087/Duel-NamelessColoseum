using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "WarriorBasicAttack", menuName = "Scriptable Objects/WarriorBasicAttack")]
public class WarriorBasicAttack : Skill
{

    [System.Serializable]
    public class SkillStructure
    {
        public basicModule basicMd;
        public damageModule damageMd;
        public coneArea area;
    }

    public SkillStructure[] skillStructures = new SkillStructure[1];
    public override basicModule basic => skillStructures[0].basicMd;

    public override void init()
    {
        
    }

    public override void execute(Transform caster, SkillExecutor exc)
    {
        Vector3 origin = caster.transform.position;

        GameObject hitten = exc.DoOverlapCone(caster, origin, skillStructures[0].area.coneRange , skillStructures[0].area.angle, targetMask, skillStructures[0].damageMd.damage);
        if (hitten)
        {
            var chctrl = caster.gameObject.GetComponent<CharacterControll>();
            chctrl.coolDownEffect("RClick", 1);
        }
    }
}
