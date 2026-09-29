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
        CharacterStatistics stats = caster.GetComponent<CharacterStatistics>();
        stats?.Statuses?.ApplyTimed(new ShieldUpStatus(currentStat.damageDebuff.reduceAmount),
            currentStat.passiveMd.duration);
    }
}
