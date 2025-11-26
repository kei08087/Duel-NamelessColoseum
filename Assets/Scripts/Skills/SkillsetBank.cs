using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SkillsetBank", menuName = "Scriptable Objects/SkillsetBank")]
public class SkillsetBank : ScriptableObject
{
    [System.Serializable]
    public class label
    {
        public CharacterEnum character;
        public SkillsetBase skillset;
    }

    public List<label> lists;

    private Dictionary<CharacterEnum, SkillsetBase> dict;

    public SkillsetBase getSkillset(CharacterEnum c)
    {
        if (dict == null)
        {
            dict = new Dictionary<CharacterEnum, SkillsetBase>();
            foreach (var labels in lists)
            {
                dict[labels.character] = labels.skillset;
            }
        }
        return dict.TryGetValue(c, out var val) ? val : null;
    }
}
