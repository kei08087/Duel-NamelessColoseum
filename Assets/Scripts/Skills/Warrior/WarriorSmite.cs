using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WarriorSmite", menuName = "Scriptable Objects/WarriorSmite")]
public class WarriorSmite : Skill
{
    [System.Serializable]
    public class SkillStructure
    {
        public basicModule basicMd;
        public damageModule damageMd;
        public coneArea area;
        public movementDebuffModule movementDebuff;
        public passiveModule passiveMD;
    }

    public SkillStructure[] skillStructures = new SkillStructure[5];
    public override basicModule basic => skillStructures[skillLevel - 1].basicMd;

    SkillStructure currentStat;

    public override void init()
    {
        currentStat = skillStructures[skillLevel - 1];
    }
    public override void execute(Transform caster, SkillExecutor exc)
    {
        Vector3 origin = caster.transform.position;
        GameObject hitten = exc.DoOverlapCone(caster, origin, currentStat.area.coneRange, currentStat.area.angle, targetMask, currentStat.damageMd.damage);
        if(hitten)
        {
            exc.executeCoroutine(slowEffect(hitten,currentStat.movementDebuff,currentStat.passiveMD));
        }
    }

    public IEnumerator slowEffect(GameObject hitten, movementDebuffModule rdM, passiveModule pM)
    {
        CharacterStatistics chstats = hitten.GetComponent<CharacterStatistics>();
        var template = rdM.reduceSpeed ? ScriptableObject.Instantiate(rdM.reduceSpeed) : ScriptableObject.CreateInstance<ReduceSpeed>();
        template.reducing = rdM.reduceAmount;
        template.skillName = skillID;
        chstats.assignModifier(template);
        try
        {
            yield return new WaitForSeconds(pM.duration);
        }
        finally
        {
            if (chstats != null)
                chstats.unassignModifier(template);
            Object.Destroy(template);
        }
    }
}
