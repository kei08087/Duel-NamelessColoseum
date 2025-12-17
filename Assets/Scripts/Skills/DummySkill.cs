using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "DummySkill", menuName = "Scriptable Objects/DummySkill")]
public class DummySkill : Skill
{
    [System.Serializable]
    public class SkillStructure
    {
        public basicModule basicMd;
    }

    private List<skillModule> modules = new()
    {
        new basicModule()
    };

    public SkillStructure[] skillStructures = new SkillStructure[1];
    public override basicModule basic => skillStructures[0].basicMd;
    public override List<skillModule> moduleSet => modules;

    public void OnEnable()
    {
        this.skillID = "Dummy Skill";
        skillStructures[0] = new SkillStructure();
        skillStructures[0].basicMd = new basicModule();
        skillStructures[0].basicMd.cooldown = 0;
        skillStructures[0].basicMd.delayBack = 0;
        skillStructures[0].basicMd.delayFront = 0;
    }

    public override void init()
    {

    }

    public override void execute(Transform caster, SkillExecutor exc)
    {
        Debug.Log("Empty Skill");
    }
}
