using UnityEngine;

[CreateAssetMenu(fileName = "ArcherDivineBow", menuName = "Scriptable Objects/ArcherDivineBow")]
public class ArcherDivineBow : Skill
{
    [System.Serializable]
    public class Level
    {
        public basicModule timing;
        public float damage;
    }

    public Level[] levels = new Level[5];
    public float range = 30f;
    public float speed = 8f;
    public float width = 0.75f;

    public override basicModule basic => levels[skillLevel - 1].timing;
    public override AttackWallPolicy WallPolicy => AttackWallPolicy.BlockHighWalls;
    public override void init() { }

    public override void execute(Transform caster, SkillExecutor executor)
    {
        CharacterStatistics stats = caster.GetComponent<CharacterStatistics>();
        StraightProjectile.Launch(caster, caster.forward, range,
            stats != null ? stats.ProjectileSpeed(speed) : speed, width,
            levels[skillLevel - 1].damage, targetMask, WallPolicy);
    }
}
