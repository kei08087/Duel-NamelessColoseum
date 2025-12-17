using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebugingSkill_SelfDamage", menuName = "Scriptable Objects/DebugingSkill_SelfDamage")]
public class DebugingSkill_SelfDamage : Skill
{
    [System.Serializable]
    public class SkillStructure
    {
        public basicModule basicMd;
        public damageModule damageMd;
    }

    private List<skillModule> modules = new()
    {
        new basicModule(),
        new damageModule()
    };

    public SkillStructure[] skillStructures = new SkillStructure[1];
    public override basicModule basic => skillStructures[0].basicMd;
    public override List<skillModule> moduleSet => modules;

    public override void init()
    {
        
    }

    public override void execute(Transform caster, SkillExecutor exc)
    {
        caster.gameObject.GetComponent<CharacterStatistics>().TakeDamage(skillStructures[0].damageMd.damage);
    }
    
}
