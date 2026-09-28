using UnityEngine;

// A visible arrow prefab with a swept hitbox so narrow shots cannot skip targets.
public class StraightProjectile : MonoBehaviour
{
    // The javelin is documented as 0.5 m wide but uses a 0.2 m sweep in game.
    // Keep arrows at the same 40% of their documented width.
    public const float HitWidthScale = 0.4f;

    private Vector3 direction;
    private float remaining;
    private float speed;
    private float radius;
    private float damage;
    private float knockback;
    private LayerMask targets;
    private AttackWallPolicy walls;

    public static void Launch(StraightProjectile prefab, Transform caster, Vector3 direction, float range, float speed,
        float width, float damage, LayerMask targets, AttackWallPolicy walls, float knockback = 0f)
    {
        direction.y = 0f;
        if (caster == null || direction.sqrMagnitude < 0.0001f)
            return;
        if (prefab == null)
        {
            Debug.LogError("Straight projectile prefab is not assigned.");
            return;
        }

        Vector3 travelDirection = direction.normalized;
        StraightProjectile projectile = Instantiate(prefab, caster.position,
            Quaternion.FromToRotation(Vector3.up, travelDirection));
        float liveWidth = Mathf.Max(0.01f, width * HitWidthScale);
        // The prefab reuses the javelin's narrow mesh. Widen large skill arrows visibly.
        projectile.transform.localScale = new Vector3(liveWidth,
            Mathf.Clamp(0.75f * width / 0.25f, 0.5f, 1.2f), liveWidth);
        projectile.direction = direction.normalized;
        // WarriorRushSlash applies TempoScale to its documented travel distance.
        // Archer skill assets also keep the documented distances and use that scale in game.
        float rangeScale = GameManager.Instance != null ? GameManager.Instance.TempoScale : 1f;
        projectile.remaining = Mathf.Max(0f, range * rangeScale);
        projectile.speed = Mathf.Max(0f, speed);
        projectile.radius = liveWidth * 0.5f;
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
