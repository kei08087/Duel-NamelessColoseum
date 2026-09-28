using UnityEngine;

// Shared wall sweep for walking and movement skills.
public static class MovementCollision
{
    public static readonly int HighWallMask = LayerMask.GetMask("HighWall");
    public static readonly int WallMask = LayerMask.GetMask("LowWall", "HighWall");
    private const float Skin = 0.01f;

    public static float LimitDistance(Transform actor, Vector3 direction, float distance)
    {
        if (actor == null || distance <= 0f || direction.sqrMagnitude < 0.0001f)
            return 0f;

        direction.Normalize();
        CapsuleCollider capsule = actor.GetComponent<CapsuleCollider>();
        if (capsule == null)
            return Physics.Raycast(actor.position, direction, out RaycastHit rayHit, distance,
                WallMask, QueryTriggerInteraction.Ignore)
                ? Mathf.Max(0f, rayHit.distance - Skin)
                : distance;

        Vector3 scale = actor.lossyScale;
        float radius = capsule.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
        float halfHeight = Mathf.Max(radius, capsule.height * Mathf.Abs(scale.y) * 0.5f);
        Vector3 center = actor.TransformPoint(capsule.center);
        Vector3 vertical = actor.up * (halfHeight - radius);

        if (Physics.CapsuleCast(center + vertical, center - vertical, radius, direction,
            out RaycastHit hit, distance, WallMask, QueryTriggerInteraction.Ignore))
            return Mathf.Max(0f, hit.distance - Skin);

        return distance;
    }
}
