using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    private CharacterControll chCtrl;
    private Skill skill;      // 예: skillSet.QSkill
    public string slotName;  // "Q", "LClick" 같은 문자열
    public Image cdFill;     // 사각형 아이콘 위에 덮을 Image

    public void OnEnable()
    {
        EventManager.PlayerUIConnection += Connect;
    }

    public void OnDisable()
    {
        EventManager.PlayerUIConnection -= Connect;
    }

    void Update()
    {
        if (chCtrl != null && skill != null && cdFill != null)
        {

            if (!chCtrl.coolEnd.TryGetValue(slotName, out float endTime))
            {
                // 아직 한 번도 안 쓴 스킬 → 쿨타임 없음
                cdFill.fillAmount = 0f;
                return;
            }

            float totalCd = skill.basic.cooldown;
            float remaining = Mathf.Max(0f, endTime - Time.time);
            float ratio = Mathf.Clamp01(remaining / totalCd);

            cdFill.fillAmount = ratio; // 1 = 꽉찬 쿨, 0 = 사용 가능
        }
    }

    void Connect(GameObject obj, bool isPlayer)
    {
        if (isPlayer)
        {
            chCtrl = obj.GetComponent<CharacterControll>();
            skill = chCtrl.skillSet.getSkill(slotName);
        }
    }
}
