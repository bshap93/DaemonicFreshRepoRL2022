// 4. Debug helper to verify combat setup

using MoreMountains.TopDownEngine;
using UnityEngine;

public class CombatDebugHelper : MonoBehaviour
{
    void VerifyCombatSetup()
    {
        // Check player setup
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log($"Player Layer: {LayerMask.LayerToName(player.layer)}");
            Debug.Log($"Player Health Component: {player.GetComponent<Health>() != null}");

            var weapon = player.GetComponentInChildren<MeleeWeapon>();
            if (weapon != null) Debug.Log($"Player Weapon TargetMask: {weapon.TargetLayerMask.value}");
        }

        // Check enemy setup
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            Debug.Log($"Enemy {enemy.name} Layer: {LayerMask.LayerToName(enemy.layer)}");
            Debug.Log($"Enemy Health Component: {enemy.GetComponent<Health>() != null}");
        }
    }
}
