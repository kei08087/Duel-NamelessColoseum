using System;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryMaker
{
    [SerializeReference]
    public skillModule skillModule;
    public GameObject gameObject;
}

public class SkillExplanationUIMaker : MonoBehaviour
{

    public List<DictionaryMaker> dictionaryMakers = new List<DictionaryMaker>();
    public Dictionary<Type, GameObject> UIDictionary;

    public List<GameObject> UIs;

    private void Awake()
    {
        foreach(DictionaryMaker dictMaker in dictionaryMakers)
        {
            UIDictionary[dictMaker.skillModule.GetType()] = dictMaker.gameObject;
        }
    }

    public void createUIList(Skill targetSkill)
    {
        foreach(skillModule module in targetSkill.moduleSet)
        {
            if(UIDictionary.TryGetValue(module.GetType(), out GameObject prePrefab))
            {
                GameObject prefab = Instantiate(prePrefab, transform);
                UIs.Add(prefab);
                ISkillExplanationUI skillExplanationUI = prefab.GetComponent<ISkillExplanationUI>();
                skillExplanationUI.getSkill(targetSkill);
                skillExplanationUI.init();
            }
        }
    }

    public void clearUIList()
    {
        foreach(GameObject ui in UIs)
        {
            Destroy(ui);
        }
        UIs.Clear();
    }
}
