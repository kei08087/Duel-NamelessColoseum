using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "WarriorRushSlash", menuName = "Scriptable Objects/WarriorRushSlash")]
public class WarriorRushSlash : Skill
{

    [System.Serializable]
    public class SkillStructure
    {
        public basicModule basicMd;
        public damageModule damageMd;
        public coneArea area;
        public moveModule moveMd;
        public animationModule animationMd;
    }

    public SkillStructure[] skillStructures = new SkillStructure[5];
    public override basicModule basic => skillStructures[skillLevel].basicMd;

    SkillStructure currentStat;

    public override void init()
    {
        currentStat = skillStructures[skillLevel];
    }
    public override void execute(Transform caster, SkillExecutor exc)
    {
        Vector3 origin = caster.transform.position;


        exc.executeCoroutine(remoteMove(caster,exc,currentStat));
    }

    public IEnumerator remoteMove(Transform caster, SkillExecutor exc, SkillStructure sSt)
    {
        Vector3 startPos = caster.transform.position;
        Vector3 dir = caster.transform.forward.normalized;
        Vector3 endPos = startPos + dir * sSt.moveMd.distance;

        Debug.Log("Original Endpos: " + endPos);
        if (Physics.Raycast(startPos, dir, out RaycastHit hit, sSt.moveMd.distance + 0.5f, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            endPos = hit.point - dir * caster.gameObject.GetComponent<CharacterStatistics>().characterSize;
        }
        Debug.Log("Endpos: " + endPos);
        float t = 0;

        var chctrl = caster.gameObject.GetComponent<CharacterControll>();
        chctrl.isDashing = true;
        GameObject hitten = null;

        while (t < sSt.basicMd.delayBack)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / sSt.basicMd.delayBack);
            float k = sSt.animationMd.easing.Evaluate(u);                 // ÀÌÂ¡ °î¼±
            caster.transform.position = Vector3.Lerp(startPos, endPos, k);
            if (u >= sSt.moveMd.hitboxOn && u < sSt.moveMd.hitboxOff && !hitten)
            {
                hitten = exc.DoOverlapCone(caster, caster.transform.position, sSt.area.coneRange, sSt.area.angle, targetMask, sSt.damageMd.damage);
            }
            yield return null;
        }

        caster.transform.position = endPos;
        chctrl.isDashing = false;

    }
}
