using System.Collections.Generic;
using UnityEngine;

public class CharacterStatistics : MonoBehaviour, IDamageable, IHealable, IMoveable
{
    public SkillsetBase skillSet;
    public float Mhp = 100;
    public float hp;
    public int basicAttack = 12;
    public float basicAttackSpeed = 1;
    public float characterSize = 0.8f;
    public float moveSpeed;
    public float instanceSpeed;
    public float Shield { get; private set; }
    private float basicAttackPenalty;

    readonly List<IDamageProcess> _damageModifiers = new();
    readonly List<IMoveProcess> _moveModifiers = new();

    private void Awake()
    {
        hp = Mhp;
        if (TryGetComponent(out Rigidbody body))
        {
            body.constraints |= RigidbodyConstraints.FreezeRotation;
            body.angularVelocity = Vector3.zero;
        }
    }

    private void Update()
    {
        instanceSpeed = moveSpeed;
        foreach (var modifier in _moveModifiers)
            modifier.preprocess(ref instanceSpeed, this);
        instanceSpeed = Mathf.Max(0f, instanceSpeed);
    }

    public void TakeDamage(float amount)
    {
        TakeDamage(new DamageBlock { damage = amount });
    }

    public void TakeDamage(DamageBlock damage)
    {
        if (hp <= 0f || (GameManager.Instance != null && GameManager.Instance.gameEnd))
            return;

        if (GameManager.Instance != null)
            GameManager.Instance.QueueDamage(this, damage);
        else
            ResolveDamage(damage);
    }

    // Damage modifiers run first. Remaining damage consumes shield before HP.
    public void ResolveDamage(DamageBlock damage)
    {
        if (hp <= 0f)
            return;

        foreach (var modifier in _damageModifiers)
            modifier.preprocess(ref damage, this);

        float remaining = damage.blocked ? 0f : Mathf.Max(0f, damage.damage);
        float absorbed = Mathf.Min(Shield, remaining);
        Shield -= absorbed;
        damage.finalDamage = remaining - absorbed;
        hp = Mathf.Max(0f, hp - damage.finalDamage);

        foreach (var modifier in _damageModifiers)
            modifier.postprocess(in damage, this);
        Debug.Log($"{gameObject.name} took {damage.finalDamage} HP damage; shield absorbed {absorbed}. HP = {hp}");
    }

    public void GrantShield(float amount)
    {
        Shield += Mathf.Max(0f, amount);
    }

    public float GetBasicAttackDamage(float baseDamage)
    {
        return Mathf.Max(0f, baseDamage - basicAttackPenalty);
    }

    public void AddBasicAttackPenalty(float amount)
    {
        basicAttackPenalty = Mathf.Max(0f, basicAttackPenalty + amount);
    }

    public void assignModifier(IDamageProcess modifier)
    {
        _damageModifiers.Add(modifier);
        _damageModifiers.Sort((a, b) => a.priority.CompareTo(b.priority));
    }

    public void assignModifier(IMoveProcess modifier)
    {
        _moveModifiers.Add(modifier);
        _moveModifiers.Sort((a, b) => a.priority.CompareTo(b.priority));
    }

    public void unassignModifier(IDamageProcess modifier)
    {
        _damageModifiers.Remove(modifier);
    }

    public void unassignModifier(IMoveProcess modifier)
    {
        _moveModifiers.Remove(modifier);
    }

    public void gainHealth(float amount)
    {
        if (hp <= 0f)
            return;
        amount = Mathf.Min(Mathf.Max(0f, amount), Mathf.Max(0f, Mhp - hp));
        hp += amount;
        Debug.Log($"{gameObject.name} gain {amount} heal. HP = {hp}");
    }

    public void Move(Vector3 direction)
    {
        if (hp <= 0f || GameManager.Instance == null || GameManager.Instance.gameEnd)
            return;
        Vector3 displacement = direction * instanceSpeed * Time.deltaTime * GameManager.Instance.TempoScale;
        displacement.y = 0f;
        float distance = MovementCollision.LimitDistance(transform, displacement, displacement.magnitude);
        if (distance > 0f)
            transform.position += displacement.normalized * distance;
    }

    public void setSkillset(SkillsetBase skillset)
    {
        skillSet = skillset;
    }
}
