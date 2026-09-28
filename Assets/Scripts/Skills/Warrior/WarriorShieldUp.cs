using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WarriorShieldUp", menuName = "Scriptable Objects/WarriorShieldUp")]
public class WarriorShieldUp : Skill
{
    [System.Serializable]
    public class SkillStructure
    {
        public basicModule basicMd;
        public damageDebuffModule damageDebuff;
        public passiveModule passiveMd;
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
        exc.executeCoroutine(shieldUp(caster,currentStat.damageDebuff,currentStat.passiveMd));
    }

    public IEnumerator shieldUp(Transform caster, damageDebuffModule ddM, passiveModule pM)
    {
        CharacterStatistics chstats = caster.gameObject.GetComponent<CharacterStatistics>();

        var template = ddM.reduceDamage ? ScriptableObject.Instantiate(ddM.reduceDamage) : ScriptableObject.CreateInstance<ReduceDamage>();

        template.reducing=ddM.reduceAmount;
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
