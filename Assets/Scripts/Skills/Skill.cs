using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.GraphicsBuffer;
using System.Collections;



[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill")]
public abstract class Skill : ScriptableObject
{
    
    [Header("Basic Attack Skill Stats")]
    public string skillID;
    public int skillLevel;

    public abstract basicModule basic {  get; }

    public abstract void init();




    [Header("Layer Settings")]
    public LayerMask targetMask;
    public LayerMask obstacleMask;



    
    public abstract void execute(Transform caster, SkillExecutor exc);
    /*
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
    */
    
}
