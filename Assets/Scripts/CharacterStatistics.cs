using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;

public class CharacterStatistics : MonoBehaviour, IDamageable
{
    public float Mhp = 100;
    public float hp;
    public int basicAttack = 12;
    public float basicAttackSpeed = 1;
    public float characterSize = 0.8f;

    private void Awake()
    {
        hp = Mhp;
    }

    public void TakeDamage(float amount)
    {
        // 이미 죽었다면 무시
        if (hp <= 0) return;

        hp -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage! HP = {hp}");

        // 피격 피드백



        // HP가 0 이하 → 사망 처리
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} has died!");

        // 사망 이펙트 추가 가능
        // 예: Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject, 0.5f); // 약간 딜레이 후 제거
    }
}
