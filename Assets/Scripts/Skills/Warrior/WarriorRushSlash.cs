using UnityEngine;

[CreateAssetMenu(fileName = "WarriorRushSlash", menuName = "Scriptable Objects/WarriorRushSlash")]
public class WarriorRushSlash : Skill
{
    public override void execute(Transform caster, SkillExecutor exc)
    {
        Vector3 origin = caster.transform.position;


        exc.executeCoroutine(remoteMove(caster,exc));
    }
}
