using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;

public class CharacterStatistics : MonoBehaviour, IDamageable, IHealable, IMoveable
{
    public float Mhp = 100;
    public float hp;
    public int basicAttack = 12;
    public float basicAttackSpeed = 1;
    public float characterSize = 0.8f;
    public float moveSpeed;
    public float instanceSpeed;

    readonly List<IDamageProcess> _damageModifiers = new();
    readonly List<IMoveProcess> _moveModifiers = new();

    private void Awake()
    {
        hp = Mhp;
        
    }

    private void Update()
    {
        instanceSpeed = moveSpeed;
        foreach (var m in _moveModifiers)
        {
            m.preprocess(ref instanceSpeed, this);
        }
        if (instanceSpeed <= 0)
            instanceSpeed = 0;
    }

    public void TakeDamage(float amount)
    {
       DamageBlock dmgB = new DamageBlock();
        dmgB.damage = amount;
        dmgB.finalDamage = 0;
        dmgB.blocked = false;
        dmgB.otherCase = false;
        dmgB.attacker = null;
        TakeDamage(dmgB);
    }

    public void assignModifier(IDamageProcess dmgProcess)
    {
        _damageModifiers.Add(dmgProcess);
        _damageModifiers.Sort((a,b)=>a.priority.CompareTo(b.priority));
    }

    public void assignModifier(IMoveProcess mvProcess)
    {
        _moveModifiers.Add(mvProcess);
        _moveModifiers.Sort((a, b)=>a.priority.CompareTo(b.priority));
    }

    public void unassignModifier(IDamageProcess dmgProcess)
    {
        _damageModifiers.Remove(dmgProcess);
    }

    public void unassignModifier(IMoveProcess mvProcess)
    {
        _moveModifiers.Remove(mvProcess);
    }

    public void TakeDamage(DamageBlock dmgB)
    {
        if(hp<= 0) return;

        foreach (var m in _damageModifiers)
        {
            m.preprocess( ref dmgB, this);
        }
        if (dmgB.damage <= 0)
            dmgB.finalDamage = 0;
        else
            dmgB.finalDamage = dmgB.damage;

        if(!dmgB.blocked)
            hp -= dmgB.finalDamage;
        Debug.Log($"{gameObject.name} took {dmgB.finalDamage} damage! HP = {hp}");

        if (hp <= 0)
        {
            Die();
        }
    }

    public void gainHealth(float amount)
    {
        if (hp <= 0) return;
        if(hp==Mhp)
        {
            amount = 0;
        }
        else if (hp + amount >= Mhp)
            amount = hp + amount - Mhp;
        hp += amount;
        Debug.Log($"{gameObject.name} gain {amount} heal. HP = {hp}");
    }

    public void Move(Vector3 dir)
    {
        //instanceSpeed = moveSpeed;
        //foreach(var  m in _moveModifiers)
        //{
        //    m.preprocess( ref instanceSpeed, this);
        //}
        //if(instanceSpeed<=0)
        //    instanceSpeed = 0;

        transform.position += dir*instanceSpeed*Time.deltaTime;
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} has died!");

        // 사망 이펙트 추가 가능
        // 예: Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject, 0.5f); // 약간 딜레이 후 제거
    }
}
