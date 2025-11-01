using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static Action GameSetup;
    public static Action PlayerSpawnEvent;
    public static Action<GameObject> PlayerSpawned;
    
    public static void GameSet()
    {
        GameSetup?.Invoke();
    }

    public static void SpawnPlayer()
    {
        PlayerSpawnEvent?.Invoke();
        
    }

    public static void SetCamera(GameObject player)
    {
        PlayerSpawned?.Invoke(player);
    }
}
