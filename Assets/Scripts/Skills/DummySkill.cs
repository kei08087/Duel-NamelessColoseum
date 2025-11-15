using UnityEngine;

[CreateAssetMenu(fileName = "DummySkill", menuName = "Scriptable Objects/DummySkill")]
public class DummySkill : Skill
{
    [System.Serializable]
    public class SkillStructure
    {
        public basicModule basicMd;
    }

    public SkillStructure[] skillStructures = new SkillStructure[1];
    public override basicModule basic => skillStructures[0].basicMd;

    public override void init()
    {
        
    }

    public override void execute(Transform caster, SkillExecutor exc)
    {
        Debug.Log("Empty Skill");
    }
}
