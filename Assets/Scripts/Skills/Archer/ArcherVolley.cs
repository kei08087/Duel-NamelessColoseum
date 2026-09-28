using UnityEngine;

[CreateAssetMenu(fileName = "ArcherVolley", menuName = "Scriptable Objects/ArcherVolley")]
public class ArcherVolley : Skill
{
    [System.Serializable]
    public class Level
    {
        public basicModule timing;
        public float damage;
        public float range;
    }

    public Level[] levels = new Level[5];
    public int arrowCount = 3;
    public float angleStep = 40f;
    public float speed = 6f;
    public float width = 0.25f;
    public StraightProjectile projectilePrefab;

    public override basicModule basic => levels[skillLevel - 1].timing;
    public override AttackWallPolicy WallPolicy => AttackWallPolicy.BlockHighWalls;
    public override void init() { }

    public override void execute(Transform caster, SkillExecutor executor)
    {
        Level level = levels[skillLevel - 1];
        CharacterStatistics stats = caster.GetComponent<CharacterStatistics>();
        float projectileSpeed = stats != null ? stats.ProjectileSpeed(speed) : speed;
        for (int i = 0; i < arrowCount; i++)
        {
            float angle = (i - (arrowCount - 1) * 0.5f) * angleStep;
            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * caster.forward;
            StraightProjectile.Launch(projectilePrefab, caster, direction, level.range, projectileSpeed,
                width, level.damage, targetMask, WallPolicy);
        }
    }
}
