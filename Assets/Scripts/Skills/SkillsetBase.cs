using UnityEngine;

[CreateAssetMenu(fileName = "SkillsetBase", menuName = "Scriptable Objects/SkillsetBase")]
public class SkillsetBase : ScriptableObject
{
    public Skill LeftClick;
    public Skill RightClick;
    public int RCLevel;
    public Skill QSkill;
    public int QLevel;
    public Skill ESkill;
    public int ELevel;
    public Skill LShift;
    public int LSLevel;
    public Skill Space;
    public int SLevel;
    public Skill LCtrl;
    public int LCtrlLevel;



    public void OnEnable()
    {
        if (LeftClick != null)
        {
            LeftClick = ScriptableObject.Instantiate(LeftClick);
        }
        else
        {
            LeftClick = ScriptableObject.CreateInstance<DummySkill>();
        }

        if (RightClick != null && RCLevel != 0)
        {
            RightClick = ScriptableObject.Instantiate(RightClick);
            RightClick.skillLevel = RCLevel - 1;
            RightClick.init();
        }
        else
        {
            RightClick = ScriptableObject.CreateInstance<DummySkill>();
        }

        if (QSkill != null && QLevel != 0)
        {
            QSkill = ScriptableObject.Instantiate(QSkill);
            QSkill.skillLevel = QLevel - 1;
            QSkill.init();
        }
        else
        {
            QSkill = ScriptableObject.CreateInstance<DummySkill>();
        }

        if (ESkill != null && ELevel != 0)
        {
            ESkill = ScriptableObject.Instantiate(ESkill);
            ESkill.skillLevel = ELevel - 1;
            ESkill.init();
        }
        else
        {
            ESkill = ScriptableObject.CreateInstance<DummySkill>();
        }

        if (LShift != null && LSLevel != 0)
        {
            LShift = ScriptableObject.Instantiate(LShift);
            LShift.skillLevel = LSLevel - 1;
            LShift.init();
        }
        else
        {
            LShift = ScriptableObject.CreateInstance<DummySkill>();
        }

        if (Space != null && SLevel != 0)
        {
            Space = ScriptableObject.Instantiate(Space);
            Space.skillLevel = SLevel - 1;
            Space.init();
        }
        else
        {
            Space = ScriptableObject.CreateInstance<DummySkill>();
        }

        if (LCtrl != null && LCtrlLevel != 0)
        {
            LCtrl = ScriptableObject.Instantiate(LCtrl);
            LCtrl.skillLevel = LCtrlLevel - 1;
            LCtrl.init();
        }
        else
        {
            LCtrl = ScriptableObject.CreateInstance<DummySkill>();
        }
    }

}
