using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillsetBase", menuName = "Scriptable Objects/SkillsetBase")]
public class SkillsetBase : ScriptableObject
{
    public CharacterEnum type;
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

    [System.NonSerialized] public Skill LeftClick;
    [System.NonSerialized] public Skill RightClick;
    [System.NonSerialized] public Skill QSkill;
    [System.NonSerialized] public Skill ESkill;
    [System.NonSerialized] public Skill LShift;
    [System.NonSerialized] public Skill Space;
    [System.NonSerialized] public Skill LCtrl;

    private readonly Dictionary<string, Skill> skillDict = new();

    public void init()
    {
        ReleaseRuntimeSkills();
        LeftClick = CreateRuntimeSkill(LeftClickSO, 1);
        RightClick = CreateRuntimeSkill(RightClickSO, RCLevel);
        QSkill = CreateRuntimeSkill(QSkillSO, QLevel);
        ESkill = CreateRuntimeSkill(ESkillSO, ELevel);
        LShift = CreateRuntimeSkill(LShiftSO, LSLevel);
        Space = CreateRuntimeSkill(SpaceSO, SLevel);
        LCtrl = CreateRuntimeSkill(LCtrlSO, LCtrlLevel);

        skillDict.Clear();
        skillDict["LClick"] = LeftClick;
        skillDict["RClick"] = RightClick;
        skillDict["Q"] = QSkill;
        skillDict["E"] = ESkill;
        skillDict["LShift"] = LShift;
        skillDict["Space"] = Space;
        skillDict["LCtrl"] = LCtrl;
    }

    public void ReleaseRuntimeSkills()
    {
        foreach (Skill skill in skillDict.Values)
        {
            if (skill != null)
                Destroy(skill);
        }
        skillDict.Clear();
        LeftClick = null;
        RightClick = null;
        QSkill = null;
        ESkill = null;
        LShift = null;
        Space = null;
        LCtrl = null;
    }

    private static Skill CreateRuntimeSkill(Skill definition, int investedPoints)
    {
        if (definition == null || investedPoints <= 0)
            return null;

        Skill runtime = Instantiate(definition);
        runtime.skillLevel = Mathf.Clamp(investedPoints, 1, 5);
        runtime.init();
        return runtime;
    }

    public Skill getSkill(string skillSlot)
    {
        skillDict.TryGetValue(skillSlot, out Skill skill);
        return skill;
    }

    public Skill getSkillSO(string skillSlot)
    {
        switch (skillSlot)
        {
            case "LClick": return LeftClickSO;
            case "RClick": return RightClickSO;
            case "Q": return QSkillSO;
            case "E": return ESkillSO;
            case "LShift": return LShiftSO;
            case "Space": return SpaceSO;
            case "LCtrl": return LCtrlSO;
            default: return null;
        }
    }

    public void setSkillLevel(string skillSlot, int level)
    {
        int points = Mathf.Clamp(level, 0, 5);
        switch (skillSlot)
        {
            case "RClick": RCLevel = points; break;
            case "Q": QLevel = points; break;
            case "E": ELevel = points; break;
            case "LShift": LSLevel = points; break;
            case "Space": SLevel = points; break;
            case "LCtrl": LCtrlLevel = points; break;
        }
    }
}
