using System.Collections;
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
        executor.executeCoroutine(Buff(caster.GetComponent<CharacterStatistics>(), levels[skillLevel - 1]));
    }

    private static IEnumerator Buff(CharacterStatistics stats, Level level)
    {
        if (stats == null) yield break;
        stats.AddAgility(level.attackRate, level.moveSpeed);
        try { yield return new WaitForSeconds(level.duration); }
        finally
        {
            if (stats != null) stats.AddAgility(-level.attackRate, -level.moveSpeed);
        }
    }
}
