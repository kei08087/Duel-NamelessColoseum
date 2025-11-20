using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillsetBase", menuName = "Scriptable Objects/SkillsetBase")]
public class SkillsetBase : ScriptableObject
{
    public Skill LeftClickSO;
    public Skill RightClickSO;
    public int RCLevel;
    public Skill QSkillSO;
    public int QLevel;
    public Skill ESkillSO;
    public int ELevel;
    public Skill LShiftSO;
    public int LSLevel;
    public Skill SpaceSO;
    public int SLevel;
    public Skill LCtrlSO;
    public int LCtrlLevel;

    [HideInInspector]
    [System.NonSerialized]
    public Skill LeftClick;
    [HideInInspector]
    [System.NonSerialized]
    public Skill RightClick;
    [HideInInspector]
    [System.NonSerialized]
    public Skill QSkill;
    [HideInInspector]
    [System.NonSerialized]
    public Skill ESkill;
    [HideInInspector]
    [System.NonSerialized]
    public Skill LShift;
    [HideInInspector]
    [System.NonSerialized]
    public Skill Space;
    [HideInInspector]
    [System.NonSerialized]
    public Skill LCtrl;

    private Dictionary<string, Skill> skillDict = new();

    public void OnEnable()
    {
        if (LeftClickSO != null)
        {
            LeftClick = ScriptableObject.Instantiate(LeftClickSO);
        }
        else
        {
            LeftClick = ScriptableObject.CreateInstance<DummySkill>();
        }
        skillDict["LClick"] = LeftClick;

        if (RightClickSO != null && RCLevel != 0)
        {
            RightClick = ScriptableObject.Instantiate(RightClickSO);
            RightClick.skillLevel = RCLevel - 1;
            RightClick.init();
        }
        else
        {
            RightClick = ScriptableObject.CreateInstance<DummySkill>();
        }
        skillDict["RClick"] = RightClick;

        if (QSkillSO != null && QLevel != 0)
        {
            QSkill = ScriptableObject.Instantiate(QSkillSO);
            QSkill.skillLevel = QLevel - 1;
            QSkill.init();
        }
        else
        {
            QSkill = ScriptableObject.CreateInstance<DummySkill>();
        }
        skillDict["Q"] = QSkill;

        if (ESkillSO != null && ELevel != 0)
        {
            ESkill = ScriptableObject.Instantiate(ESkillSO);
            ESkill.skillLevel = ELevel - 1;
            ESkill.init();
        }
        else
        {
            ESkill = ScriptableObject.CreateInstance<DummySkill>();
        }
        skillDict["E"]  = ESkill;

        if (LShiftSO != null && LSLevel != 0)
        {
            LShift = ScriptableObject.Instantiate(LShiftSO);
            LShift.skillLevel = LSLevel - 1;
            LShift.init();
        }
        else
        {
            LShift = ScriptableObject.CreateInstance<DummySkill>();
        }
        skillDict["LShift"] = LShift;

        if (SpaceSO != null && SLevel != 0)
        {
            Space = ScriptableObject.Instantiate(SpaceSO);
            Space.skillLevel = SLevel - 1;
            Space.init();
        }
        else
        {
            Space = ScriptableObject.CreateInstance<DummySkill>();
        }
        skillDict["Space"] = Space;

        if (LCtrlSO != null && LCtrlLevel != 0)
        {
            LCtrl = ScriptableObject.Instantiate(LCtrlSO);
            LCtrl.skillLevel = LCtrlLevel - 1;
            LCtrl.init();
        }
        else
        {
            LCtrl = ScriptableObject.CreateInstance<DummySkill>();
        }
        skillDict["LCtrl"] = LCtrl;
    }

    public Skill getSkill(string skillSlot)
    {
        if(skillDict.TryGetValue(skillSlot, out Skill s))
            return s;
        Debug.Log("Null Retrun3d");
        return null;
    }

}
