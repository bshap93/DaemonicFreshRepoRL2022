using System;
using System.Linq;
using System.Threading.Tasks;
using Characters.Player.Scripts;
using DunGen;
using Project.Gameplay.DungeonGeneration.Generators;
using Project.Gameplay.DungeonGeneration.Spawning;
using UnityEngine;
// DunGen's namespace

namespace Project.Gameplay.DungeonGeneration
{
    public class NewDungeonManager : MonoBehaviour
    {
        private RuntimeDungeon _runtimeDungeon;
        private TaskCompletionSource<bool> _generationTaskSource;
        private SpawnPointManager _spawnPointManager;


        private void Awake()
        {
            _spawnPointManager = gameObject.AddComponent<SpawnPointManager>();
            _runtimeDungeon = gameObject.GetComponent<RuntimeDungeon>();
            if (_runtimeDungeon == null)
            {
                _runtimeDungeon = gameObject.AddComponent<RuntimeDungeon>();
                Debug.LogWarning("DungeonFlow asset needs to be assigned to RuntimeDungeon!");
            }

            // Subscribe to DunGen's generation complete event
            _runtimeDungeon.Generator.OnGenerationComplete += OnDungeonGenerationComplete;
        }

        private void OnDestroy()
        {
            if (_runtimeDungeon != null && _runtimeDungeon.Generator != null)
            {
                _runtimeDungeon.Generator.OnGenerationComplete -= OnDungeonGenerationComplete;
            }
        }

        private void OnDungeonGenerationComplete(DungeonGenerator generator)
        {
            _generationTaskSource?.TrySetResult(true);
            // Initialize spawn points after dungeon is generated
            _spawnPointManager.InitializeSpawnPoints(generator.CurrentDungeon);

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
            
                _runtimeDungeon.Generator.Seed = seed;
                _runtimeDungeon.Generate();
            
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
