using UnityEngine;

[CreateAssetMenu(fileName = "ReduceDamage", menuName = "Scriptable Objects/ReduceDamage")]
public class ReduceDamage : ScriptableObject, IDamageProcess
{
    public int priority => 1;
    public float reducing;
    public string skillName;

    public void preprocess(ref DamageBlock dmgB, CharacterStatistics chstats)
    {
        Debug.Log($"Effect of {skillName}, {reducing} damage was reduced from {dmgB.damage}");
        dmgB.damage -= reducing;
    }

    public void postprocess(in DamageBlock dmgB, CharacterStatistics chstats)
    {

    }
}
