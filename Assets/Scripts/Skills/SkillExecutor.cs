using System.Collections;
using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    public GameObject DoOverlapCone(Transform caster, Vector3 center, float radius, float angle,
        LayerMask targetMask, float damage)
    {
        DrawConeGizmo(caster, radius, angle, 0.5f);
        Physics.SyncTransforms();
        Collider[] hits = Physics.OverlapSphere(center, radius, targetMask, QueryTriggerInteraction.Collide);
        foreach (Collider hit in hits)
        {
            Vector3 toTarget = hit.bounds.center - caster.position;
            toTarget.y = 0f;
            if (Vector3.Angle(caster.forward, toTarget) > angle * 0.5f)
                continue;
            if (Physics.Linecast(center, hit.bounds.center, MovementCollision.HighWallMask,
                QueryTriggerInteraction.Ignore))
                continue;
            if (hit.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damage);
                return hit.gameObject;
            }
        }
        return null;
    }

    public static void DrawConeGizmo(Transform caster, float radius, float angle, float duration = 0.1f)
    {
        Vector3 forward = caster.forward;
        Vector3 left = Quaternion.Euler(0, -angle * 0.5f, 0) * forward;
        Vector3 right = Quaternion.Euler(0, angle * 0.5f, 0) * forward;
        Debug.DrawRay(caster.position, forward * radius, Color.yellow, duration);
        Debug.DrawRay(caster.position, left * radius, Color.yellow, duration);
        Debug.DrawRay(caster.position, right * radius, Color.yellow, duration);
    }

    public void executeCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}
