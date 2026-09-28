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
        var availableCharacters = new List<CharacterEnum>();
        foreach (CharacterEnum character in Enum.GetValues(typeof(CharacterEnum)))
        {
            SkillsetBase definition = bank.getSkillset(character);
            if (definition != null && definition.characterPrefab != null)
                availableCharacters.Add(character);
        }
        charEnum = availableCharacters.ToArray();

        drop.ClearOptions();

        var options = new List<TMP_Dropdown.OptionData>();
        foreach(var c in charEnum)
        {
            options.Add(new TMP_Dropdown.OptionData(c.ToString()));
        }
        drop.AddOptions(options);

        

        drop.onValueChanged.AddListener(onDropDownChanged);
        if (charEnum.Length == 0)
        {
            Debug.LogError("No playable character skillsets are registered.");
            return;
        }

        drop.SetValueWithoutNotify(0);
        drop.RefreshShownValue();
        onDropDownChanged(0);
    }

    private void onDropDownChanged(int index)
    {
        if (charEnum == null || index < 0 || index >= charEnum.Length)
            return;
        CharacterEnum selected = charEnum[index];
        if (selectedSkillSet != null)
        {
            selectedSkillSet.ReleaseRuntimeSkills();
            Destroy(selectedSkillSet);
        }
        selectedSkillSet = Instantiate(bank.getSkillset(selected));
    }

    public SkillsetBase getSkillset()
    {
        return selectedSkillSet;
    }
}
