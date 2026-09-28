using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject character;
    public GameObject wrapper;
    public bool isPlayer = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        EventManager.PlayerSpawnEvent += playerSpawn;
    }

    private void OnDisable()
    {
        EventManager.PlayerSpawnEvent -= playerSpawn;
    }

    void playerSpawn(SkillsetBase skillset)
    {
        if (character == null || wrapper == null)
        {
            Debug.LogError($"Spawner {name} is missing a character or wrapper prefab.");
            return;
        }

        GameObject wrap = Instantiate(wrapper);
        GameObject spawnedCharacter = Instantiate(character, transform.position, transform.rotation);
        spawnedCharacter.transform.SetParent(wrap.transform, true);

        CharacterStatistics stats = spawnedCharacter.GetComponent<CharacterStatistics>();
        if (stats == null)
        {
            Debug.LogError($"Spawned character {spawnedCharacter.name} has no CharacterStatistics.");
            Destroy(spawnedCharacter);
            Destroy(wrap);
            return;
        }

        if (isPlayer)
        {
            if (spawnedCharacter.GetComponent<CombatInputSource>() == null)
                spawnedCharacter.AddComponent<CombatInputSource>();
            skillset.init();
            stats.setSkillset(skillset);
            GameManager.Instance.RegisterPlayer(spawnedCharacter);
        }
        else
        {
            stats.setSkillset(null);
            GameManager.Instance.RegisterEnemy(spawnedCharacter);
        }

        EventManager.PlayerUIConnection(spawnedCharacter, isPlayer);
    }


}
