using UnityEngine;

[CreateAssetMenu(fileName = "ArcherSpiritArrow", menuName = "Scriptable Objects/ArcherSpiritArrow")]
public class ArcherSpiritArrow : Skill
{
    [System.Serializable]
    public class Level
    {
        public basicModule timing;
        public float damage;
    }

    public Level[] levels = new Level[5];
    public float range = 3f;
    public float speed = 5f;
    public float width = 0.4f;
    public float knockback = 2f;
    public StraightProjectile projectilePrefab;

    public override basicModule basic => levels[skillLevel - 1].timing;
    public override AttackWallPolicy WallPolicy => AttackWallPolicy.BlockHighWalls;
    public override void init() { }

    public override void execute(Transform caster, SkillExecutor executor)
    {
        CharacterStatistics stats = caster.GetComponent<CharacterStatistics>();
        StraightProjectile.Launch(projectilePrefab, caster, caster.forward, range,
            stats != null ? stats.ProjectileSpeed(speed) : speed, width,
            levels[skillLevel - 1].damage, targetMask, WallPolicy, knockback);
    }
}
