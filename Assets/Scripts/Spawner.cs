using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject character;
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

    void playerSpawn()
    {
        GameObject player = Instantiate(character, transform.position, transform.rotation);
        if(isPlayer) 
            GameManager.Instance.RegisterPlayer(player);
    }


}
