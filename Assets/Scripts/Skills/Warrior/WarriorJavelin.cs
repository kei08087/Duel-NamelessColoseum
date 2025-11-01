using UnityEngine;

[CreateAssetMenu(fileName = "WarriorJavelin", menuName = "Scriptable Objects/WarriorJavelin")]
public class WarriorJavelin : Skill
{
    public override void execute(Transform caster, SkillExecutor exc)
    {
        GameObject javelin = caster.transform.Find("Javelin").gameObject;
        Javelin javel = javelin.GetComponent<Javelin>();
        javel.parent = javelin.transform.parent.gameObject;
        javelin.transform.SetParent(null,true);

        javel.layer = targetMask;
        javel.damage = damage;
        javel.length = 3.5f;
        javel.coolDown = cooldown;
        javel.speed = 6;

        javel.activate=true;
    }
}
