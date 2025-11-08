using UnityEngine;

[CreateAssetMenu(fileName = "SkillsetBase", menuName = "Scriptable Objects/SkillsetBase")]
public class SkillsetBase : ScriptableObject
{
    public Skill LeftClick;
    public Skill RightClick;
    public Skill QSkill;
    public Skill ESkill;
    public Skill LShift;
    public Skill Space;
    public Skill LCtrl;
    
}
