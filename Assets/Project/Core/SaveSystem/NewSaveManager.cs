using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Characters.Player.Scripts;
using Cysharp.Threading.Tasks;
using Project.Gameplay.DungeonGeneration.Spawning;
using UnityEngine;

namespace Project.Core.SaveSystem
{

// Helper class to implement the ISaveable interface
    public interface ISaveable
    {
        void SaveState(SaveData saveData);
        void LoadState(SaveData saveData);
    }

// Save Manager to handle saving/loading
    public class NewSaveManager : MonoBehaviour
    {
        // Track level transitions for save/load
        private Dictionary<string, LevelTransitionData> levelTransitions = new();
        
        private SpawnPointManager spawnPointManager;
        


        private SpawnPoint FindSpawnPoint(string spawnPointId)
        {
            return spawnPointManager.GetSpawnPointById(spawnPointId);
        }
        
        public void SetLastTransitionPoint(string levelId, string spawnPointId, SpawnDirection direction)
        {
            levelTransitions[levelId] = new LevelTransitionData
            {
                levelId = levelId,
                lastSpawnPointId = spawnPointId,
                direction = direction,
                playerPosition = PlayerCharacter.Instance.transform.position,
                playerRotation = PlayerCharacter.Instance.transform.rotation
            };
        }
        public LevelTransitionData GetLevelTransitionData(string levelId)
        {
            return levelTransitions.TryGetValue(levelId, out var data) ? data : null;
        }
        
        public async Task HandleLevelTransition(string targetLevelId, string targetSpawnPointId, SpawnDirection direction)
        {
            // Save current level state
            SaveGame();

            // Load new level
            // TODO: Implement level loading
        
            // Find target spawn point and position player
            var spawnPoint = FindSpawnPoint(targetSpawnPointId);
            if (spawnPoint != null)
            {
                PlayerCharacter.Instance.transform.position = spawnPoint.transform.position;
                PlayerCharacter.Instance.transform.rotation = spawnPoint.transform.rotation;
            
                // Notify spawn point of transition
                spawnPoint.OnLevelTransition(direction);
            }
        }
        public static NewSaveManager Instance { get; private set; }
        public SaveData CurrentSave { get; private set; }
    
        void Awake()
        {
            spawnPointManager = FindObjectOfType<SpawnPointManager>();
            if (spawnPointManager == null)
            {
                Debug.LogError("No SpawnPointManager found in scene!");
            }
            
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                CurrentSave = new SaveData();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async void SaveGame(string slot = "default")
        {
            try
            {
                // Collect save data from all ISaveable objects
                var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
                foreach (var saveable in saveables)
                {
                    saveable.SaveState(CurrentSave);
                }
            
                // Update timestamp
                CurrentSave.timestamp = DateTime.Now;
            
                // Save using Easy Save 3
                ES3.Save($"save_{slot}", CurrentSave);
            
                Debug.Log($"Game saved successfully to slot: {slot}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving game: {e.Message}");
            }
        }

        public bool LoadGame(string slot = "default")
        {
            try
            {
                if (!ES3.KeyExists($"save_{slot}"))
                {
                    Debug.LogWarning($"No save file found in slot: {slot}");
                    return false;
                }

                CurrentSave = ES3.Load<SaveData>($"save_{slot}");
            
                // Load data into all ISaveable objects
                var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
                foreach (var saveable in saveables)
                {
                    saveable.LoadState(CurrentSave);
                }
            
                Debug.Log($"Game loaded successfully from slot: {slot}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading game: {e.Message}");
                return false;
            }
        }

    }
}