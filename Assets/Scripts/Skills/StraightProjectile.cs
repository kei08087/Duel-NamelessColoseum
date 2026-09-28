using UnityEngine;

// A graphics-free projectile. Its collision is swept so narrow arrows cannot skip targets.
public class StraightProjectile : MonoBehaviour
{
    private Vector3 direction;
    private float remaining;
    private float speed;
    private float radius;
    private float damage;
    private float knockback;
    private LayerMask targets;
    private AttackWallPolicy walls;

    public static void Launch(Transform caster, Vector3 direction, float range, float speed,
        float width, float damage, LayerMask targets, AttackWallPolicy walls, float knockback = 0f)
    {
        direction.y = 0f;
        if (caster == null || direction.sqrMagnitude < 0.0001f)
            return;

        GameObject arrow = new GameObject("Arrow");
        arrow.transform.position = caster.position;
        StraightProjectile projectile = arrow.AddComponent<StraightProjectile>();
        projectile.direction = direction.normalized;
        projectile.remaining = Mathf.Max(0f, range);
        projectile.speed = Mathf.Max(0f, speed);
        projectile.radius = Mathf.Max(0.01f, width * 0.5f);
        projectile.damage = damage;
        projectile.knockback = knockback;
        projectile.targets = targets;
        projectile.walls = walls;
        projectile.CheckInitialHit();
    }

    private void CheckInitialHit()
    {
        Physics.SyncTransforms();
        foreach (Collider candidate in Physics.OverlapSphere(transform.position, radius, targets,
            QueryTriggerInteraction.Collide))
        {
            if (candidate.TryGetComponent<IDamageable>(out _))
            {
                Hit(candidate);
                return;
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameEnd)
        {
            Destroy(gameObject);
            return;
        }

        float step = Mathf.Min(remaining, speed * Time.deltaTime);
        if (step <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        Physics.SyncTransforms();
        if (ProjectileCollision.Sweep(transform.position, direction, radius, step, targets,
            walls, null, out RaycastHit hit))
        {
            transform.position += direction * Mathf.Max(0f, hit.distance);
            Hit(hit.collider);
            return;
        }

        transform.position += direction * step;
        remaining -= step;
        if (remaining <= 0f)
            Destroy(gameObject);
    }

    private void Hit(Collider target)
    {
        if (target != null && ((1 << target.gameObject.layer) & targets.value) != 0 &&
            target.TryGetComponent<IDamageable>(out var receiver))
        {
            receiver.TakeDamage(damage);
            if (knockback > 0f && target.TryGetComponent<CharacterStatistics>(out var stats))
                stats.ApplyKnockback(direction, knockback);
        }
        Destroy(gameObject);
    }
}
