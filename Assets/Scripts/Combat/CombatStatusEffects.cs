using UnityEngine;

public sealed class AgilityStatus : ICombatStatusEffect
{
    private readonly float attackRate;
    private readonly float moveSpeed;

    public AgilityStatus(float attackRate, float moveSpeed)
    {
        this.attackRate = attackRate;
        this.moveSpeed = moveSpeed;
    }

    public void Apply(CharacterStatistics target, long tick) => target.AddAgility(attackRate, moveSpeed);
    public void OnTick(CharacterStatistics target, long tick) { }
    public void Remove(CharacterStatistics target) => target.AddAgility(-attackRate, -moveSpeed);
}

public sealed class WindBlessingStatus : ICombatStatusEffect
{
    private readonly float projectileSpeed;
    private readonly float basicRange;

    public WindBlessingStatus(float projectileSpeed, float basicRange)
    {
        this.projectileSpeed = projectileSpeed;
        this.basicRange = basicRange;
    }

    public void Apply(CharacterStatistics target, long tick) =>
        target.AddWindBlessing(projectileSpeed, basicRange);
    public void OnTick(CharacterStatistics target, long tick) { }
    public void Remove(CharacterStatistics target) =>
        target.AddWindBlessing(-projectileSpeed, -basicRange);
}

public sealed class ShieldUpStatus : ICombatStatusEffect, IDamageProcess
{
    private readonly float reduction;
    public int priority => 1;

    public ShieldUpStatus(float reduction) => this.reduction = Mathf.Max(0f, reduction);

    public void Apply(CharacterStatistics target, long tick)
    {
        target.assignModifier((IDamageProcess)this);
        target.AddBasicAttackPenalty(1f);
    }

    public void OnTick(CharacterStatistics target, long tick) { }

    public void Remove(CharacterStatistics target)
    {
        target.unassignModifier((IDamageProcess)this);
        target.AddBasicAttackPenalty(-1f);
    }

    public void preprocess(ref DamageBlock damage, CharacterStatistics target) =>
        damage.damage -= reduction;
    public void postprocess(in DamageBlock damage, CharacterStatistics target) { }
}

public sealed class SlowStatus : ICombatStatusEffect, IMoveProcess
{
    private readonly float reduction;
    public int priority => 2;

    public SlowStatus(float reduction) => this.reduction = Mathf.Clamp01(reduction);

    public void Apply(CharacterStatistics target, long tick) => target.assignModifier((IMoveProcess)this);
    public void OnTick(CharacterStatistics target, long tick) { }
    public void Remove(CharacterStatistics target) => target.unassignModifier((IMoveProcess)this);

    public void preprocess(ref float speed, CharacterStatistics target) => speed *= 1f - reduction;
    public void postprocess(in float speed, CharacterStatistics target) { }
}

public sealed class PeriodicHealStatus : ICombatStatusEffect
{
    private readonly float amount;
    private long nextHealTick;
    private long periodTicks;

    public PeriodicHealStatus(float amount) => this.amount = Mathf.Max(0f, amount);

    public void Apply(CharacterStatistics target, long tick)
    {
        periodTicks = CombatStatusController.DurationTicks(1f);
        nextHealTick = tick + periodTicks;
    }

    public void OnTick(CharacterStatistics target, long tick)
    {
        while (tick >= nextHealTick && target.hp > 0f)
        {
            target.gainHealth(amount);
            nextHealTick += periodTicks;
        }
    }

    public void Remove(CharacterStatistics target) { }
}
