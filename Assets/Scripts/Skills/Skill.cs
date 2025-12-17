using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.GraphicsBuffer;



[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill")]
public abstract class Skill : ScriptableObject
{
    
    [Header("Basic Attack Skill Stats")]
    public string skillID;
    public int skillLevel;

    public abstract basicModule basic {  get; }
    public abstract List<skillModule> moduleSet {  get; }

    public abstract void init();




    [Header("Layer Settings")]
    public LayerMask targetMask;
    public LayerMask obstacleMask;



    
    public abstract void execute(Transform caster, SkillExecutor exc);

    public bool HasModule<T>() where T : skillModule
    => moduleSet.Exists(m => m is T);

    public T GetModule<T>() where T : skillModule
        => (T)moduleSet.Find(m => m is T);
}
