// GameInitiator.cs

using System.Threading.Tasks;
using DunGen;
using MoreMountains.TopDownEngine;
using Project.Core.SaveSystem;
using Project.Gameplay.DungeonGeneration;
using UnityEngine;

namespace Project.Core.GameInitialization
{
    public class GameInitiator : MonoBehaviour
    {
        static GameInitiator _instance;
        RuntimeDungeon _runtimeDungeon;
        NewSaveManager _saveManager;
        NewDungeonManager dungeonManager;

        void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            // Find references in our prefab structure
            dungeonManager = GetComponentInChildren<NewDungeonManager>();
            _runtimeDungeon = GetComponentInChildren<RuntimeDungeon>();

            if (dungeonManager == null || _runtimeDungeon == null)
                Debug.LogError("Missing required components in PortableSystems prefab!");

            // Check if NewSaveManager is already in the scene
            _saveManager = NewSaveManager.Instance;
            if (_saveManager == null) _saveManager = gameObject.AddComponent<NewSaveManager>();
        }

        async void Start()
        {
            await InitializeCore();
        }

        async Task InitializeCore()
        {
            // Load previous save or start a new game
            if (!await LoadLastGame()) await StartNewGame();
        }

        async Task<bool> LoadLastGame()
        {
            return await Task.FromResult(_saveManager.LoadGame());
        }

        async Task StartNewGame()
        {
            var seed = Random.Range(0, int.MaxValue);
            await dungeonManager.GenerateNewDungeon(seed);

            if (NewSaveManager.Instance.LoadGame())
                Debug.Log("Save data loaded successfully");
            else
                Debug.LogError("Failed to load save data");

            var initialSpawnPoint = FindObjectOfType<CheckPoint>();
            if (initialSpawnPoint == null) Debug.LogError("No Checkpoint found for initial spawn!");
        }
    }
}
