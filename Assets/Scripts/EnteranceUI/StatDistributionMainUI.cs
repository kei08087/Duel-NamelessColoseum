using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatDistributionMainUI : MonoBehaviour
{

    [SerializeField]
    private int usableSkillPoint;
    [SerializeField]
    private CharacterSelectionUI characterSelectionUI;

    public int maxSkillPoint;

    public Slider[] sliders;

    public SkillsetBase selectedCharacterSkillset;

    public TextMeshProUGUI currentSkillpointDisplay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnEnable()
    {
        resetSkillPoint();
    }

    // Update is called once per frame

    public void resetSkillPoint()
    {
        usableSkillPoint = maxSkillPoint;
        foreach (Slider sld in sliders)
        {
            sld.value = 0;
            applySkillPoint(sld);
            string sliderName = sld.name;
            string adjustedName = sliderName.Replace("SkillSlot", "");
            TextMeshProUGUI textM = sld.GetComponentInChildren<TextMeshProUGUI>();
            textM.text = selectedCharacterSkillset.getSkillSO(adjustedName).skillID;
        }
        display();
    }

    public void setSkillPoint(Slider signaledFrom)
    {
        int curPoint = maxSkillPoint;
        foreach (Slider sld in sliders)
        {
            curPoint -= (int)sld.value;
        }
        if (curPoint > maxSkillPoint)
        {
            curPoint = maxSkillPoint;
        }
        if (curPoint < 0)
        {
            signaledFrom.value += curPoint;
            curPoint = 0;
        }
        usableSkillPoint = curPoint;
        applySkillPoint(signaledFrom);
        display();
    }

    void applySkillPoint(Slider signaledFrom)
    {
        string sliderName = signaledFrom.name;
        string adjustedName = sliderName.Replace("SkillSlot", "");
        Debug.Log(adjustedName);
        Debug.Log(signaledFrom.value);
        selectedCharacterSkillset.setSkillLevel(adjustedName, (int)signaledFrom.value);
    }

    public void display()
    {
        currentSkillpointDisplay.text = usableSkillPoint.ToString();
    }

    public int getSkillPoint()
    {
        return usableSkillPoint;
    }

    public void recieveSkillset()
    {
        selectedCharacterSkillset = characterSelectionUI.getSkillset();
    }

    public void returnSkillset(SceneManagering sceneManager)
    {
        sceneManager.playerSkillset =  selectedCharacterSkillset;
    }
}
