using System.Threading.Tasks;
using Core.SaveSystem.Scripts;
using DunGen;
using Project.Core.SaveSystem;
using Project.Gameplay.DungeonGeneration;
using Unity.AI.Navigation.Samples;
using UnityEngine;

namespace Project.Core.GameInitialization
{
    public class GameInitiator : MonoBehaviour
    {
        private static GameInitiator _instance;
        private NewSaveManager _saveManager;
        private NewDungeonManager _dungeonManager;
        private RuntimeDungeon _runtimeDungeon;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
        
            _instance = this;
        
            // Find references in our prefab structure
            _dungeonManager = GetComponentInChildren<NewDungeonManager>();
            _runtimeDungeon = GetComponentInChildren<RuntimeDungeon>();
        
            if (_dungeonManager == null || _runtimeDungeon == null)
            {
                Debug.LogError("Missing required components in PortableSystems prefab!");
            }
        }

        private async void Start()
        {
            await InitializeCore();
        }

        private async Task InitializeCore()
        {
            // Add SaveManager dynamically
            _saveManager = gameObject.AddComponent<NewSaveManager>();

            // Try to load previous save, or start new game
            if (!await LoadLastGame())
            {
                await StartNewGame();
            }
        }

        private async Task<bool> LoadLastGame()
        {
            return await Task.FromResult(_saveManager.LoadGame());
        }

        private async Task StartNewGame()
        {
            int seed = Random.Range(0, int.MaxValue);
            await _dungeonManager.GenerateNewDungeon(seed);
        }
    }
}
