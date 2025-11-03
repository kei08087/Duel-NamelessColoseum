using UnityEngine;

[CreateAssetMenu(fileName = "WarriorJavelin", menuName = "Scriptable Objects/WarriorJavelin")]
public class WarriorJavelin : Skill
{

    public float length = 3.5f;
    public float speed = 6f;
    public override void execute(Transform caster, SkillExecutor exc)
    {
        GameObject javelin = caster.transform.Find("Javelin").gameObject;
        Javelin javel = javelin.GetComponent<Javelin>();
        javel.parent = javelin.transform.parent.gameObject;
        javelin.transform.SetParent(null,true);

        javel.layer = targetMask;
        javel.damage = damage;
        javel.length = length;
        javel.coolDown = cooldown;
        javel.speed = speed;

        javel.activate=true;
    }
}
