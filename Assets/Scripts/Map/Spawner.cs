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
        GameObject wrap = Instantiate(wrapper);
        GameObject player = Instantiate(character, transform.position, transform.rotation);
        if(isPlayer) 
            GameManager.Instance.RegisterPlayer(player);
        else
            GameManager.Instance.RegisterEnemy(player);
        player.transform.SetParent(wrap.transform, true);

        if(skillset != null&&isPlayer)
            skillset.init();
        player.GetComponent<CharacterStatistics>().setSkillset(skillset);
        EventManager.PlayerUIConnection(player, isPlayer);
    }


}
