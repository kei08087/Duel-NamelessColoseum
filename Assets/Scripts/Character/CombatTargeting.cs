using UnityEngine;

// Team layers belong to the spawned combatant, never to a skill definition asset.
public static class CombatTargeting
{
    public static int SideLayer(bool isPlayer) => LayerMask.NameToLayer(isPlayer ? "Player" : "Enemy");

    public static LayerMask OpponentMask(bool isPlayer) =>
        LayerMask.GetMask(isPlayer ? "Enemy" : "Player");

    public static void AssignSide(GameObject combatant, bool isPlayer)
    {
        if (combatant == null)
            return;

        int layer = SideLayer(isPlayer);
        if (layer < 0)
        {
            Debug.LogError("Player and Enemy layers must exist before spawning combatants.");
            return;
        }
        combatant.layer = layer;
        // Physics masks inspect the collider GameObject's layer, not its root.
        // Only existing collider owners are assigned; later skill effects keep
        // their own projectile and obstruction layers.
        foreach (Collider collider in combatant.GetComponentsInChildren<Collider>(true))
            collider.gameObject.layer = layer;
    }
}
