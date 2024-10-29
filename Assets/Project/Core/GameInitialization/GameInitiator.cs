using System.Threading.Tasks;
using Project.Core.SaveSystem;
using UnityEngine;

public class GameInitiator : MonoBehaviour
{
    void Start()
    {
        
    }
    
    private async Task InitializeCore()
    {
        // Initialize SaveManager first
        var saveManager = gameObject.AddComponent<NewSaveManager>();
    
        // Try to load last save
        if (!saveManager.LoadGame())
        {
            // No save found, initialize new game
            Debug.Log("No save found, starting new game");
        }
    }

    void Update()
    {
        
    }
}
