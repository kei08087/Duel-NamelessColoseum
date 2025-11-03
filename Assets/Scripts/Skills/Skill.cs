using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.GraphicsBuffer;
using System.Collections;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill")]
public class Skill : ScriptableObject
{
    [Header("Basic Attack Skill Stats")]
    public string skillID;
    public float cooldown;
    public float delayFront;
    public float delayBack;
    public float damage;

    [Header("Layer Settings")]
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [Header("Cone Range Setting")]
    public float radius;
    public float angle;

    [Header("Move Stats")]
    public float distance;
    public float hitboxOn;
    public float hitboxOff;

    [Header("Animations")]
    public AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public virtual void execute(Transform caster, SkillExecutor exc)
    {
    }

    public IEnumerator remoteMove(Transform caster)
    {
        Vector3 startPos = caster.transform.position;
        Vector3 dir = caster.transform.forward;
        Vector3 endPos = startPos + dir * distance;

        if (Physics.Raycast(startPos, dir, out RaycastHit hit, distance, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            endPos = hit.point - dir * caster.gameObject.GetComponent<CharacterStatistics>().characterSize;
        }

        float t = 0;

        var chctrl = caster.gameObject.GetComponent<CharacterControll>();
        chctrl.isDashing = true;

        while( t < delayBack)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / delayBack);
            float k = easing.Evaluate(u);                 // ÀÌÂ¡ °î¼±
            caster.transform.position = Vector3.Lerp(startPos, endPos, k);
            yield return null;
        }

        caster.transform.position = endPos;
        chctrl.isDashing = false;

    }

    public IEnumerator remoteMove(Transform caster, SkillExecutor exc)
    {
        Vector3 startPos = caster.transform.position;
        Vector3 dir = caster.transform.forward.normalized;
        Vector3 endPos = startPos + dir * distance;

        Debug.Log("Original Endpos: " + endPos);
        if (Physics.Raycast(startPos, dir, out RaycastHit hit, distance+0.5f, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            endPos = hit.point - dir * caster.gameObject.GetComponent<CharacterStatistics>().characterSize;
        }
        Debug.Log("Endpos: "+endPos);
        float t = 0;

        var chctrl = caster.gameObject.GetComponent<CharacterControll>();
        chctrl.isDashing = true;
        bool hitten = false;

        while (t < delayBack)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / delayBack);
            float k = easing.Evaluate(u);                 // ÀÌÂ¡ °î¼±
            caster.transform.position = Vector3.Lerp(startPos, endPos, k);
            if (u >= hitboxOn && u < hitboxOff && !hitten)
            {
                hitten = exc.DoOverlapCone(caster, caster.transform.position, radius, angle, targetMask, damage);
            }
            yield return null;
        }

        caster.transform.position = endPos;
        chctrl.isDashing = false;

    }
}
