using UnityEngine;

[CreateAssetMenu(fileName = "WarriorJavelin", menuName = "Scriptable Objects/WarriorJavelin")]
public class WarriorJavelin : Skill
{
    [System.Serializable]
    public class SkillStructure
    {
        public basicModule basicMd;
        public damageModule damageMd;
        public missleRangeModule missleRange;
    }

    public SkillStructure[] skillStructures = new SkillStructure[5];
    public override basicModule basic => skillStructures[skillLevel - 1].basicMd;

    SkillStructure currentStat;

    public override void init()
    {
        currentStat = skillStructures[skillLevel - 1];
    }

    public override void execute(Transform caster, SkillExecutor exc)
    {
        
        GameObject javelin = caster.transform.Find("Javelin").gameObject;
        Javelin javel = javelin.GetComponent<Javelin>();
        javelin.transform.localPosition = new Vector3(0.6f, 0.5f, 0f);
        javelin.transform.localEulerAngles = new Vector3(90f, 90f, 90f);
        javel.parent = javelin.transform.parent.gameObject;
        javelin.transform.SetParent(null,true);

        javel.layer = targetMask;
        javel.damage = currentStat.damageMd.damage;
        javel.length = currentStat.missleRange.length;
        javel.coolDown = currentStat.basicMd.cooldown;
        javel.speed = currentStat.missleRange.objectSpeed;

        javel.activate=true;
    }
}
