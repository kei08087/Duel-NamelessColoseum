using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject wrapper;
    public bool isPlayer = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        EventManager.CombatantsSpawnEvent += SpawnCombatant;
    }

    private void OnDisable()
    {
        EventManager.CombatantsSpawnEvent -= SpawnCombatant;
    }

    private void SpawnCombatant(SkillsetBase playerSkillset, SkillsetBase enemySkillset)
    {
        SkillsetBase skillset = isPlayer ? playerSkillset : enemySkillset;
        if (skillset == null || skillset.characterPrefab == null || wrapper == null)
        {
            Debug.LogError($"Spawner {name} is missing a skillset, character, or wrapper prefab.");
            return;
        }

        GameObject wrap = Instantiate(wrapper);
        GameObject spawnedCharacter = Instantiate(skillset.characterPrefab, transform.position, transform.rotation);
        spawnedCharacter.transform.SetParent(wrap.transform, true);

        CharacterStatistics stats = spawnedCharacter.GetComponent<CharacterStatistics>();
        if (stats == null)
        {
            Debug.LogError($"Spawned character {spawnedCharacter.name} has no CharacterStatistics.");
            Destroy(spawnedCharacter);
            Destroy(wrap);
            return;
        }

        CombatTargeting.AssignSide(spawnedCharacter, isPlayer);
        CombatInputSource input = spawnedCharacter.GetComponent<CombatInputSource>();
        if (input == null)
            input = spawnedCharacter.AddComponent<CombatInputSource>();
        input.readDesktopInput = true;
        input.desktopProfile = isPlayer ? DesktopCombatProfile.PlayerOne : DesktopCombatProfile.PlayerTwo;
        skillset.init(CombatTargeting.OpponentMask(isPlayer));
        stats.setSkillset(skillset);
        if (isPlayer)
            GameManager.Instance.RegisterPlayer(spawnedCharacter);
        else
            GameManager.Instance.RegisterEnemy(spawnedCharacter);

        EventManager.ConnectUI(spawnedCharacter, isPlayer);
    }


}
