using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MousePointerDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public StatDistributionMainUI StatDistributionMainUI;
    public GameObject skillExplanationUI;
    public SkillExplanationUIMaker maker;
    public Slider sld;
    private Skill skill;


    public void OnPointerEnter(PointerEventData eventData)
    {
        clearUI();
        skill = StatDistributionMainUI.getSkill(sld);
        skillExplanationUI.SetActive(true);
        maker.createUIList(skill);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        clearUI();
    }

    void clearUI()
    {
        maker.clearUIList();
        skillExplanationUI.SetActive(false);
    }
}
