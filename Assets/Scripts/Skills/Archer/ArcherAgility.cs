using UnityEngine;

[CreateAssetMenu(fileName = "ArcherAgility", menuName = "Scriptable Objects/ArcherAgility")]
public class ArcherAgility : Skill
{
    [System.Serializable]
    public class Level
    {
        public basicModule timing;
        public float duration;
        public float attackRate;
        public float moveSpeed;
    }

    public Level[] levels = new Level[5];
    public override basicModule basic => levels[skillLevel - 1].timing;
    public override void init() { }

    public override void execute(Transform caster, SkillExecutor executor)
    {
        CharacterStatistics stats = caster.GetComponent<CharacterStatistics>();
        Level level = levels[skillLevel - 1];
        stats?.Statuses?.ApplyTimed(new AgilityStatus(level.attackRate, level.moveSpeed), level.duration);
    }
}
