using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;




[System.Serializable]
public class damageModule
{
    public float damage;
}


[System.Serializable]
public class coneArea
{
    public float coneRange;
    public float angle;
}

[System.Serializable]
public class boxArea
{
    public float distance;
    public float width;
}

[System.Serializable]
public class basicModule
{
    public float cooldown;
    public float delayFront;
    public float delayBack;
}

[System.Serializable]
public class passiveModule
{
    public float duration;
}

[System.Serializable]
public class moveModule
{
    public float distance;
    public float hitboxOn;
    public float hitboxOff;
}

[System.Serializable]
public class missleRangeModule
{
    public float length;
    public float objectSpeed;
}

[System.Serializable]
public class animationModule
{
    public AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);
}

[System.Serializable]
public class movementDebuffModule
{
    public ReduceSpeed reduceSpeed;
    public float reduceAmount;
}

[System.Serializable]
public class damageDebuffModule
{
    public ReduceDamage reduceDamage;
    public float reduceAmount;
}

[System.Serializable]
public class healModule
{
    public float healAmount;
}