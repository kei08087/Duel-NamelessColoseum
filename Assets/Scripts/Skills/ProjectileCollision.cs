using UnityEngine;

// Shared collision policy for straight projectile movement and trigger fallback.
public static class ProjectileCollision
{
    public static int CollisionMask(LayerMask targets, AttackWallPolicy wallPolicy)
    {
        return targets.value | MovementCollision.AttackMask(wallPolicy);
    }

    public static bool ShouldImpact(int layer, LayerMask targets, AttackWallPolicy wallPolicy)
    {
        return ((1 << layer) & CollisionMask(targets, wallPolicy)) != 0;
    }

    public static bool Sweep(Vector3 origin, Vector3 direction, float radius, float distance,
        LayerMask targets, AttackWallPolicy wallPolicy, Collider self, out RaycastHit nearest)
    {
        nearest = default;
        if (distance <= 0f || direction.sqrMagnitude < 0.0001f)
            return false;

        RaycastHit[] hits = Physics.SphereCastAll(origin, radius, direction.normalized, distance,
            CollisionMask(targets, wallPolicy), QueryTriggerInteraction.Collide);
        bool found = false;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null || hit.collider == self)
                continue;
            if (!found || hit.distance < nearest.distance)
            {
                nearest = hit;
                found = true;
            }
        }
        return found;
    }
}
