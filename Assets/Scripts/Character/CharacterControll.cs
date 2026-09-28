using System.Collections.Generic;
using UnityEngine;

public class CharacterControll : MonoBehaviour
{
    public SkillExecutor skillExecutor;
    public CharacterStatistics chstats;
    public LayerMask ground;
    public bool isDashing;

    private CombatInputSource input;
    private CastController caster;

    public Dictionary<string, float> coolEnd => caster.coolEnd;

    private void Awake()
    {
        if (skillExecutor == null) skillExecutor = GetComponent<SkillExecutor>();
        if (chstats == null) chstats = GetComponent<CharacterStatistics>();
        input = GetComponent<CombatInputSource>();
        caster = GetComponent<CastController>();
        if (caster == null) caster = gameObject.AddComponent<CastController>();
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.gameEnd || chstats.hp <= 0f)
            return;
        if (input == null) input = GetComponent<CombatInputSource>();
        if (input == null) return;

        while (input.TryDequeue(out CombatCommand command))
            caster.TryCast(command);

        Vector3 move = input.MoveDirection;
        if (caster.CanTurn && !isDashing && move.sqrMagnitude > 0f)
            transform.rotation = Quaternion.LookRotation(move);
        if (caster.CanMove && !isDashing)
            chstats.Move(move);
    }

    public void coolDownEffect(string skillSlot, float amount)
    {
        caster.ReduceCooldown(skillSlot, amount);
    }

    public void ApplyStun(float duration)
    {
        caster.ApplyStun(duration);
    }
}
