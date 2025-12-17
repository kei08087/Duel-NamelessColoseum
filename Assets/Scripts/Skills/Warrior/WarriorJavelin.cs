using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WarriorJavelin", menuName = "Scriptable Objects/WarriorJavelin")]
public class WarriorJavelin : Skill
{
    [System.Serializable]
    public class SkillStructure
    {
        public basicModule basicMd;
        public damageModule damageMd;
        public missleRangeModule missleRange;
    }

    private List<skillModule> modules = new()
    {
        new basicModule(),
        new damageModule(),
        new missleRangeModule()
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
        
        GameObject javelin = caster.transform.Find("Javelin").gameObject;
        Javelin javel = javelin.GetComponent<Javelin>();
        javel.parent = javelin.transform.parent.gameObject;
        javelin.transform.SetParent(null,true);

        javel.layer = targetMask;
        javel.damage = currentStat.damageMd.damage;
        javel.length = currentStat.missleRange.length;
        javel.coolDown = currentStat.basicMd.cooldown;
        javel.speed = currentStat.missleRange.objectSpeed;

        javel.activate=true;
    }
}
