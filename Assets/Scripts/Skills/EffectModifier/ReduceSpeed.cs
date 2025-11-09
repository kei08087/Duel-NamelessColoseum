using UnityEngine;

[CreateAssetMenu(fileName = "ReduceSpeed", menuName = "Scriptable Objects/ReduceSpeed")]
public class ReduceSpeed : ScriptableObject, IMoveProcess
{
    public int priority => 2;
    public float reducing;
    public string skillName;

    public void preprocess(ref float speed, CharacterStatistics chstats)
    {
        Debug.Log($"Effect of {skillName}, {(1f-reducing)*100}% speed was reduced.");
        speed *= (1f - reducing);
    }

    public void postprocess(in float speed, CharacterStatistics chstats) { }
}
