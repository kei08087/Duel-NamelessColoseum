using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public interface skillModule
{

}

[System.Serializable]
public class damageModule : skillModule
{
    public float damage;
}


[System.Serializable]
public class coneArea : skillModule
{
    public float coneRange;
    public float angle;
}

[System.Serializable]
public class boxArea : skillModule
{
    public float distance;
    public float width;
}

[System.Serializable]
public class basicModule : skillModule
{
    public float cooldown;
    public float delayFront;
    public float delayBack;
}

[System.Serializable]
public class passiveModule : skillModule
{
    public float duration;
}

[System.Serializable]
public class moveModule : skillModule
{
    public float distance;
    public float hitboxOn;
    public float hitboxOff;
}

[System.Serializable]
public class missleRangeModule : skillModule
{
    public float length;
    public float objectSpeed;
}

[System.Serializable]
public class animationModule : skillModule
{
    public AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);
}

[System.Serializable]
public class movementDebuffModule : skillModule
{
    public ReduceSpeed reduceSpeed;
    public float reduceAmount;
}

[System.Serializable]
public class damageDebuffModule : skillModule
{
    public ReduceDamage reduceDamage;
    public float reduceAmount;
}

[System.Serializable]
public class healModule : skillModule
{
    public float healAmount;
}