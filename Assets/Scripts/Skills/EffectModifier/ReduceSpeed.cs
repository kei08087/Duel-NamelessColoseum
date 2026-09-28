using UnityEngine;

[CreateAssetMenu(fileName = "ReduceSpeed", menuName = "Scriptable Objects/ReduceSpeed")]
public class ReduceSpeed : ScriptableObject, IMoveProcess
{
    public int priority => 2;
    public float reducing;
    public string skillName;

    public void preprocess(ref float speed, CharacterStatistics chstats)
    {
        speed *= 1f - Mathf.Clamp01(reducing);
    }

    public void postprocess(in float speed, CharacterStatistics chstats) { }
}
