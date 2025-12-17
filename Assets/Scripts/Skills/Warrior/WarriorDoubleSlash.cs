using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WarriorDoubleSlash", menuName = "Scriptable Objects/WarriorDoubleSlash")]
public class WarriorDoubleSlash : Skill
{

    [System.Serializable]
    public class SkillStructure
    {
        public basicModule basicMd;
        public damageModule damageMd;
        public coneArea area;
    }

    private List<skillModule> modules = new()
    {
        new basicModule(),
        new damageModule(),
        new coneArea()
    };

    public SkillStructure[] skillStructures = new SkillStructure[5];
    public override basicModule basic => skillStructures[skillLevel].basicMd;
    public override List<skillModule> moduleSet => modules;

    SkillStructure currentStat;

    public override void init()
    {
        currentStat = skillStructures[skillLevel];
    }

    public override void execute(Transform caster, SkillExecutor exc)
    {

        exc.executeCoroutine(doubleAttack(caster, exc,currentStat.damageMd,currentStat.area));

    }

    public IEnumerator doubleAttack(Transform caster, SkillExecutor exc, damageModule dM, coneArea area)
    {
        Vector3 origin = caster.transform.position;

        exc.DoOverlapCone(caster, origin, area.coneRange, area.angle, targetMask, dM.damage);
        yield return new WaitForSeconds(0.3f);
        exc.DoOverlapCone(caster, origin, area.coneRange, area.angle, targetMask, dM.damage);
    }
}
