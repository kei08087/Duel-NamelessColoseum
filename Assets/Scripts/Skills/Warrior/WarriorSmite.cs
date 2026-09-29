using UnityEngine;

[CreateAssetMenu(fileName = "WarriorSmite", menuName = "Scriptable Objects/WarriorSmite")]
public class WarriorSmite : Skill
{
    [System.Serializable]
    public class SkillStructure
    {
        public basicModule basicMd;
        public damageModule damageMd;
        public coneArea area;
        public movementDebuffModule movementDebuff;
        public passiveModule passiveMD;
    }

    public SkillStructure[] skillStructures = new SkillStructure[5];
    public override basicModule basic => skillStructures[skillLevel - 1].basicMd;
    public override AttackWallPolicy WallPolicy => AttackWallPolicy.BlockAllWalls;

    SkillStructure currentStat;

    public override void init()
    {
        currentStat = skillStructures[skillLevel - 1];
    }
    public override void execute(Transform caster, SkillExecutor exc)
    {
        Vector3 origin = caster.transform.position;
        GameObject hitten = exc.DoOverlapCone(caster, origin, currentStat.area.coneRange, currentStat.area.angle, targetMask, currentStat.damageMd.damage, WallPolicy);
        if(hitten)
        {
            CharacterStatistics target = hitten.GetComponent<CharacterStatistics>();
            target?.Statuses?.ApplyTimed(new SlowStatus(currentStat.movementDebuff.reduceAmount),
                currentStat.passiveMD.duration);
        }
    }
}
