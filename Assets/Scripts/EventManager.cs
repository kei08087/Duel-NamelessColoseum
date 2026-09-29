using UnityEngine;
using System;

public static class EventManager
{
    public static Action GameSetup;
    public static Action<SkillsetBase, SkillsetBase> CombatantsSpawnEvent;
    public static Action<GameObject> PlayerSpawned;
    public static Action<GameObject, bool> PlayerUIConnection;
    public static Action GameEnd;
    
    public static void GameSet()
    {
        GameSetup?.Invoke();
    }

    public static void SpawnCombatants(SkillsetBase playerSkillset, SkillsetBase enemySkillset)
    {
        CombatantsSpawnEvent?.Invoke(playerSkillset, enemySkillset);
    }

    public static void SetCamera(GameObject player)
    {
        PlayerSpawned?.Invoke(player);
    }

    public static void ConnectUI(GameObject character, bool isPlayer)
    {
        PlayerUIConnection?.Invoke(character, isPlayer);
    }

    public static void EndTheGame()
    {
        GameEnd?.Invoke();
    }
}
