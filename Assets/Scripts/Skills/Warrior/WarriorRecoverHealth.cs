using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WarriorRecoverHealth", menuName = "Scriptable Objects/WarriorRecoverHealth")]
public class WarriorRecoverHealth : Skill
{
    public override void execute(Transform caster, SkillExecutor exc)
    {
        exc.executeCoroutine(recover(caster));
    }

    public IEnumerator recover(Transform caster)
    {
        float t = 0;
        var chstats = caster.gameObject.GetComponent<CharacterStatistics>();
        while (t < duration)
        {
            yield return new WaitForSeconds(1f);
            chstats.gainHealth(damage);
            t += 1;
        }
    }
}
