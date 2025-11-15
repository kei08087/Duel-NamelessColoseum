using UnityEngine;

[CreateAssetMenu(fileName = "SkillsetBase", menuName = "Scriptable Objects/SkillsetBase")]
public class SkillsetBase : ScriptableObject
{
    public Skill LeftClickSO;
    public Skill LeftClick;
    public Skill RightClickSO;
    public Skill RightClick;
    public int RCLevel;
    public Skill QSkillSO;
    public Skill QSkill;
    public int QLevel;
    public Skill ESkillSO;
    public Skill ESkill;
    public int ELevel;
    public Skill LShiftSO;
    public Skill LShift;
    public int LSLevel;
    public Skill SpaceSO;
    public Skill Space;
    public int SLevel;
    public Skill LCtrlSO;
    public Skill LCtrl;
    public int LCtrlLevel;



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
    }

}
