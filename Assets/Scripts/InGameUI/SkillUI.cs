using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    private CharacterControll character;
    private Skill skill;
    public string slotName;
    public Image cdFill;

    private void OnEnable()
    {
        EventManager.PlayerUIConnection += Connect;
    }

    private void OnDisable()
    {
        EventManager.PlayerUIConnection -= Connect;
    }

    private void Update()
    {
        if (cdFill == null) return;
        if (skill == null)
        {
            cdFill.fillAmount = 1f;
            return;
        }

        if (character == null || !character.coolEnd.TryGetValue(slotName, out float endTime))
        {
            cdFill.fillAmount = 0f;
            return;
        }

        float cooldown = skill.basic.cooldown;
        float remaining = Mathf.Max(0f, endTime - Time.time);
        cdFill.fillAmount = cooldown > 0f ? Mathf.Clamp01(remaining / cooldown) : 0f;
    }

    private void Connect(GameObject obj, bool isPlayer)
    {
        if (!isPlayer) return;
        character = obj.GetComponent<CharacterControll>();
        CharacterStatistics stats = obj.GetComponent<CharacterStatistics>();
        skill = stats.skillSet.getSkill(slotName);
    }
}
