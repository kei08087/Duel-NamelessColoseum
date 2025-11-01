using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    public void DoOverlapCone(Transform caster, Vector3 center, float radius, float angle, LayerMask mask, float damage)
    {
        DrawConeGizmo(caster, radius, angle, 0.5f);
        //var hits = Physics.OverlapSphere(center, radius, mask, QueryTriggerInteraction.Collide);

        var allHits = Physics.OverlapSphere(center, radius, ~0, QueryTriggerInteraction.Collide);
        Debug.Log($"[Probe/ALL] count={allHits.Length} (r={radius}, angle={angle})");

        // 각 히트의 레이어, 이름, 콜라이더 타입을 출력
        foreach (var h in allHits)
        {
            var go = h.gameObject;
            Debug.Log($"[ALL] {go.name} layer={LayerMask.LayerToName(go.layer)} type={h.GetType().Name}");
        }

        // ② 그 다음 실제 마스크로 필터
        var hits = Physics.OverlapSphere(center, radius, mask, QueryTriggerInteraction.Collide);
        Debug.Log($"[Probe/MASK] count={hits.Length}, maskBits={mask.value}");


        foreach (var hit in hits)
        {
            Vector3 to = hit.transform.position - caster.position;
            to.y = 0f;
            float a = Vector3.Angle(caster.forward, to);
            if (a <= angle * 0.5f)
                if (hit.TryGetComponent<IDamageable>(out var d)) d.TakeDamage(damage);
        }
    }

    public static void DrawConeGizmo(Transform caster, float radius, float angle, float duration = 0.1f)
    {
        var forward = caster.forward;
        var left = Quaternion.Euler(0, -angle * 0.5f, 0) * forward;
        var right = Quaternion.Euler(0, angle * 0.5f, 0) * forward;
        Debug.DrawRay(caster.position, forward * radius, Color.yellow, duration);
        Debug.DrawRay(caster.position, left * radius, Color.yellow, duration);
        Debug.DrawRay(caster.position, right * radius, Color.yellow, duration);
    }
}
