using UnityEngine;

[CreateAssetMenu(fileName = "ArcherWindBlessing", menuName = "Scriptable Objects/ArcherWindBlessing")]
public class ArcherWindBlessing : Skill
{
    [System.Serializable]
    public class Level
    {
        public basicModule timing;
        public float duration;
        public float projectileSpeed;
        public float basicRange;
    }

    public Level[] levels = new Level[5];
    public override basicModule basic => levels[skillLevel - 1].timing;
    public override void init() { }

    public override void execute(Transform caster, SkillExecutor executor)
    {
        CharacterStatistics stats = caster.GetComponent<CharacterStatistics>();
        Level level = levels[skillLevel - 1];
        stats?.Statuses?.ApplyTimed(new WindBlessingStatus(level.projectileSpeed, level.basicRange),
            level.duration);
    }
}
