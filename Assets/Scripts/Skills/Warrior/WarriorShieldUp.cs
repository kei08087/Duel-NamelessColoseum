using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
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

    private List<skillModule> modules = new()
    {
        new basicModule(),
        new damageDebuffModule(),
        new passiveModule()
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
        exc.executeCoroutine(shieldUp(caster,currentStat.damageDebuff,currentStat.passiveMd));
    }

    public IEnumerator shieldUp(Transform caster, damageDebuffModule ddM, passiveModule pM)
    {
        CharacterStatistics chstats = caster.gameObject.GetComponent<CharacterStatistics>();

        var template = ddM.reduceDamage ? ScriptableObject.Instantiate(ddM.reduceDamage) : ScriptableObject.CreateInstance<ReduceDamage>();

        template.reducing=ddM.reduceAmount;
        template.skillName = skillID;
        chstats.assignModifier(template);
        yield return new WaitForSeconds(pM.duration);
        chstats.unassignModifier(template);
    }


}
