using System;
using System.Linq;
using System.Threading.Tasks;
using Characters.Player.Scripts;
using DunGen;
using Project.Gameplay.DungeonGeneration.Generators;
using Project.Gameplay.DungeonGeneration.Spawning;
using UnityEngine;
using UnityEngine.Serialization;

// DunGen's namespace

namespace Project.Gameplay.DungeonGeneration
{
    public class NewDungeonManager : MonoBehaviour
    {
        [FormerlySerializedAs("_runtimeDungeon")] 
        [SerializeField] RuntimeDungeon runtimeDungeon;
        private TaskCompletionSource<bool> _generationTaskSource;
        private SpawnPointManager _spawnPointManager;
        [SerializeField] private GameObject playerPrefab;  // Assign your player prefab in inspector
        private GameObject _playerInstance;



        private void Awake()
        {
            _spawnPointManager = gameObject.GetComponent<SpawnPointManager>();
            if (_spawnPointManager == null)
            {
                _spawnPointManager = gameObject.AddComponent<SpawnPointManager>();
            }
            // runtimeDungeon = gameObject.GetComponent<RuntimeDungeon>();
            if (runtimeDungeon == null)
            {
                runtimeDungeon = gameObject.AddComponent<RuntimeDungeon>();
                Debug.LogWarning("DungeonFlow asset needs to be assigned to RuntimeDungeon!");
            }

            // Subscribe to DunGen's generation complete event
            runtimeDungeon.Generator.OnGenerationComplete += OnDungeonGenerationComplete;
        }

        private void OnDestroy()
        {
            if (runtimeDungeon != null && runtimeDungeon.Generator != null)
            {
                runtimeDungeon.Generator.OnGenerationComplete -= OnDungeonGenerationComplete;
            }
        }

        private void OnDungeonGenerationComplete(DungeonGenerator generator)
        {
            _generationTaskSource?.TrySetResult(true);
            // Initialize spawn points after dungeon is generated
            _spawnPointManager.InitializeSpawnPoints(generator.CurrentDungeon);
            
            // Create player if it doesn't exist
            if (_playerInstance == null)
            {
                _playerInstance = Instantiate(playerPrefab);
            }


            // Example: Spawn player at start point
            var startPoint = _spawnPointManager.GetAvailableSpawnPoints(SpawnPointType.Player)
                .OfType<PlayerSpawnPoint>()
                .FirstOrDefault(p => p.SpawnType == PlayerSpawnPoint.PlayerSpawnType.LevelStart);

            if (startPoint != null)
            {
                // Spawn player
                PlayerCharacter.Instance.transform.position = startPoint.transform.position;
                PlayerCharacter.Instance.transform.rotation = startPoint.transform.rotation;
                startPoint.MarkOccupied();
            }

            // Example: Spawn enemies based on difficulty
            float currentDifficulty = 1.0f; // Example difficulty value
            var enemySpawns = _spawnPointManager.GetAvailableSpawnPoints(SpawnPointType.Enemy);
            foreach (var spawn in enemySpawns)
            {
                // TODO: Spawn appropriate enemy based on difficulty
                spawn.MarkOccupied();
            }
            
        }

        public async Task GenerateNewDungeon(int seed)
        {
            try 
            {
                _generationTaskSource = new TaskCompletionSource<bool>();
            
                runtimeDungeon.Generator.Seed = seed;
                runtimeDungeon.Generate();
            
                // Wait for generation complete event
                await _generationTaskSource.Task;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to generate dungeon: {e.Message}");
                throw;
            }
        }

        public void LoadDungeon(DungeonData data)
        {
            // TODO: Implement using DunGen's systems
            throw new NotImplementedException();
        }
    }
}
