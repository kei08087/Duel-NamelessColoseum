using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WarriorDoubleSlash", menuName = "Scriptable Objects/WarriorDoubleSlash")]
public class WarriorDoubleSlash : Skill
{

    [System.Serializable]
    public class SkillStructure
    {
        public basicModule basicMd;
        public damageModule damageMd;
        public coneArea area;
    }

    public SkillStructure[] skillStructures = new SkillStructure[5];
    public override basicModule basic => skillStructures[skillLevel - 1].basicMd;
    public override bool CanCancelBasicRecovery => true;

    SkillStructure currentStat;

    public override void init()
    {
        currentStat = skillStructures[skillLevel - 1];
    }

    public override void execute(Transform caster, SkillExecutor exc)
    {
        caster.GetComponent<CastController>()?.ResetCooldown("LClick");
        exc.executeCoroutine(doubleAttack(caster, exc,currentStat.damageMd,currentStat.area));

    }

    public IEnumerator doubleAttack(Transform caster, SkillExecutor exc, damageModule dM, coneArea area)
    {
        exc.DoOverlapCone(caster, caster.position, area.coneRange, area.angle, targetMask, dM.damage);
        yield return new WaitForSeconds(0.3f);
        if (caster != null && (GameManager.Instance == null || !GameManager.Instance.gameEnd) &&
            caster.GetComponent<CharacterStatistics>().hp > 0f)
            exc.DoOverlapCone(caster, caster.position, area.coneRange, area.angle, targetMask, dM.damage);
    }
}
