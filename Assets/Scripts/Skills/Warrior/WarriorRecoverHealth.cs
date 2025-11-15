using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WarriorRecoverHealth", menuName = "Scriptable Objects/WarriorRecoverHealth")]
public class WarriorRecoverHealth : Skill
{

    [System.Serializable]
    public class SkillStructure
    {
        public basicModule basicMd;
        public healModule healMd;
        public passiveModule passiveMd;
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
        exc.executeCoroutine(recover(caster, currentStat.healMd, currentStat.passiveMd));
    }

    public IEnumerator recover(Transform caster, healModule hM, passiveModule pM)
    {
        float t = 0;
        var chstats = caster.gameObject.GetComponent<CharacterStatistics>();
        while (t < pM.duration)
        {
            yield return new WaitForSeconds(1f);
            chstats.gainHealth(hM.healAmount);
            t += 1;
        }
    }
}
