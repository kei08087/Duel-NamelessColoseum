using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;

public class CharacterSelectionUI : MonoBehaviour
{

    [SerializeField]
    private TMP_Dropdown drop;
    [SerializeField]
    private SkillsetBank bank;

    private CharacterEnum[] charEnum;

    [SerializeField]
    private SkillsetBase selectedSkillSet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        charEnum = (CharacterEnum[])Enum.GetValues(typeof(CharacterEnum));

        drop.ClearOptions();

        var options = new List<TMP_Dropdown.OptionData>();
        foreach(var c in charEnum)
        {
            options.Add(new TMP_Dropdown.OptionData(c.ToString()));
        }
        drop.AddOptions(options);

        

        drop.onValueChanged.AddListener(onDropDownChanged);
        drop.value = -1;
        drop.RefreshShownValue();
    }

    private void onDropDownChanged(int index)
    {
        if (index < 0)
            return;
        CharacterEnum selected = charEnum[index];
        selectedSkillSet = Instantiate(bank.getSkillset(selected));
    }

    public SkillsetBase getSkillset()
    {
        return selectedSkillSet;
    }
}
