using System;
using System.Threading.Tasks;
using DunGen;
using Project.Gameplay.DungeonGeneration.Generators;
using UnityEngine;
// DunGen's namespace

namespace Project.Gameplay.DungeonGeneration
{
    public class NewDungeonManager : MonoBehaviour
    {
        private RuntimeDungeon _runtimeDungeon;
        private TaskCompletionSource<bool> _generationTaskSource;

        private void Awake()
        {
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
