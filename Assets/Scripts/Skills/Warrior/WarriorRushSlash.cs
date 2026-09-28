using System.Collections;
using UnityEngine;

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
    public override basicModule basic => skillStructures[skillLevel - 1].basicMd;

    private SkillStructure currentStat;
    private float tempoScale;

    public override void init()
    {
        currentStat = skillStructures[skillLevel - 1];
        tempoScale = GameManager.Instance.TempoScale;
    }

    public override void execute(Transform caster, SkillExecutor executor)
    {
        executor.executeCoroutine(MoveAndStrike(caster, executor, currentStat));
    }

    private IEnumerator MoveAndStrike(Transform caster, SkillExecutor executor, SkillStructure stats)
    {
        Vector3 start = caster.position;
        Vector3 direction = caster.forward;
        direction.y = 0f;
        direction.Normalize();

        float requestedDistance = stats.moveMd.distance * tempoScale;
        float travelDistance = MovementCollision.LimitDistance(caster, direction, requestedDistance);
        Vector3 end = start + direction * travelDistance;
        float duration = Mathf.Max(0.0001f, stats.basicMd.delayBack);
        float elapsed = 0f;
        GameObject hitTarget = null;
        CharacterControll controller = caster.GetComponent<CharacterControll>();

        if (controller != null)
            controller.isDashing = true;

        try
        {
            while (caster != null && elapsed < duration &&
                   (GameManager.Instance == null || !GameManager.Instance.gameEnd))
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                float eased = Mathf.Clamp01(stats.animationMd.easing.Evaluate(progress));
                caster.position = Vector3.Lerp(start, end, eased);

                if (progress >= stats.moveMd.hitboxOn && progress < stats.moveMd.hitboxOff && hitTarget == null)
                    hitTarget = executor.DoOverlapCone(caster, caster.position, stats.area.coneRange,
                        stats.area.angle, targetMask, stats.damageMd.damage);

                yield return null;
            }

            if (caster != null && (GameManager.Instance == null || !GameManager.Instance.gameEnd))
                caster.position = end;
        }
        finally
        {
            if (controller != null)
                controller.isDashing = false;
        }
    }
}
