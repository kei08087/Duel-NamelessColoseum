using UnityEngine;

public interface IDamageProcess
{
    int priority { get; }
    void preprocess(ref DamageBlock dmgB, CharacterStatistics chstats);
    void postprocess(in DamageBlock dmgB, CharacterStatistics chstats);
}
