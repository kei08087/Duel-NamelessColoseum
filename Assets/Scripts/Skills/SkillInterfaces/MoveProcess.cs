using UnityEngine;

public interface IMoveProcess
{
    int priority { get; }
    void preprocess(ref float speed, CharacterStatistics chstats);
    void postprocess(in float speed, CharacterStatistics chstats);
}
