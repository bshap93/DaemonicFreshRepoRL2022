using System.Threading.Tasks;
using DunGen;
using Project.Core.SaveSystem;
using Project.Gameplay.DungeonGeneration;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Core.GameInitialization
{
    public class GameInitiator : MonoBehaviour
    {
        static GameInitiator _instance;
        [FormerlySerializedAs("_dungeonManager")] [SerializeField]
        NewDungeonManager dungeonManager;
        RuntimeDungeon _runtimeDungeon;
        NewSaveManager _saveManager;

        void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            // Find references in our prefab structure
            // dungeonManager = GetComponentInChildren<NewDungeonManager>();
            _runtimeDungeon = GetComponentInChildren<RuntimeDungeon>();

            if (dungeonManager == null || _runtimeDungeon == null)
                Debug.LogError("Missing required components in PortableSystems prefab!");
        }

        async void Start()
        {
            await InitializeCore();
        }

        async Task InitializeCore()
        {
            // Add SaveManager dynamically
            _saveManager = gameObject.AddComponent<NewSaveManager>();

            // Try to load previous save, or start new game
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
        }
    }
}
