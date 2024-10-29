using Project.Core.SaveSystem;
using UnityEngine;
// TODO: Create InputActions asset and class
// using Project.Core.Input;
// using Project.Core.Stats;
// using Project.Core.Inventory;

public class PlayerController : MonoBehaviour, ISaveable
{
    // TODO: Implement custom input system
    // private InputActions inputActions;
    private CharacterController controller;
    
    // TODO: Create PlayerStats class with health, speed, etc
    // private PlayerStats stats;
    
    // TODO: Create Inventory system
    // private Inventory inventory;
    
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        // TODO: Initialize input system
        // TODO: Initialize stats
        // TODO: Initialize inventory
    }
    
    public void SaveState(SaveData saveData)
    {
        saveData.playerData.position = transform.position;
        saveData.playerData.rotation = transform.rotation;
        // TODO: Save stats once PlayerStats is implemented
        // TODO: Save inventory once Inventory is implemented
    }
    
    public void LoadState(SaveData saveData)
    {
        transform.position = saveData.playerData.position;
        transform.rotation = saveData.playerData.rotation;
        // TODO: Load stats once PlayerStats is implemented
        // TODO: Load inventory once Inventory is implemented
    }
}
