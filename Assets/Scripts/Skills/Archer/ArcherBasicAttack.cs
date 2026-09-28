using UnityEngine;

[CreateAssetMenu(fileName = "ArcherBasicAttack", menuName = "Scriptable Objects/ArcherBasicAttack")]
public class ArcherBasicAttack : Skill
{
    public basicModule timing;
    public float damage = 6f;
    public float range = 3f;
    public float speed = 6f;
    public float width = 0.25f;

    public override basicModule basic => timing;
    public override AttackWallPolicy WallPolicy => AttackWallPolicy.BlockHighWalls;
    public override void init() { }

    public override void execute(Transform caster, SkillExecutor executor)
    {
        CharacterStatistics stats = caster.GetComponent<CharacterStatistics>();
        StraightProjectile.Launch(caster, caster.forward,
            stats != null ? stats.BasicAttackRange(range) : range,
            stats != null ? stats.ProjectileSpeed(speed) : speed,
            width, stats != null ? stats.GetBasicAttackDamage(damage) : damage,
            targetMask, WallPolicy);
    }
}
