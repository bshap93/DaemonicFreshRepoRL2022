using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Core.CharacterCreation;
using Project.Gameplay.DungeonGeneration.Spawning;
using Project.Gameplay.Player;
using UnityEngine;

namespace Project.Core.SaveSystem
{
// Helper class to implement the ISaveable interface


// Save Manager to handle saving/loading
    public class NewSaveManager : MonoBehaviour
    {
        // Track level transitions for save/load
        readonly Dictionary<string, LevelTransitionData> levelTransitions = new();

        SpawnPointManager spawnPointManager;
        public static NewSaveManager Instance { get; private set; }
        public SaveData CurrentSave { get; private set; }

        void Awake()
        {
            spawnPointManager = FindObjectOfType<SpawnPointManager>();
            if (spawnPointManager == null) Debug.LogWarning("No SpawnPointManager found in scene!");

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


        SpawnPoint FindSpawnPoint(string spawnPointId)
        {
            return spawnPointManager.GetSpawnPointById(spawnPointId);
        }

        public void SetLastTransitionPoint(string levelId, string spawnPointId, SpawnDirection direction)
        {
            var playerGameObject = GameObject.FindGameObjectWithTag("Player");
            levelTransitions[levelId] = new LevelTransitionData
            {
                levelId = levelId,
                lastSpawnPointId = spawnPointId,
                direction = direction,
                playerPosition = playerGameObject.transform.position,
                playerRotation = playerGameObject.transform.rotation
            };
        }
        public LevelTransitionData GetLevelTransitionData(string levelId)
        {
            return levelTransitions.TryGetValue(levelId, out var data) ? data : null;
        }

        public Task HandleLevelTransition(string targetLevelId, string targetSpawnPointId, SpawnDirection direction)
        {
            // Save current level state
            SaveGame();

            // Load new level
            // TODO: Implement level loading

            var playerGameObject = GameObject.FindGameObjectWithTag("Player");

            // Find target spawn point and position player
            var spawnPoint = FindSpawnPoint(targetSpawnPointId);
            if (spawnPoint != null)
            {
                playerGameObject.transform.position = spawnPoint.transform.position;
                playerGameObject.transform.rotation = spawnPoint.transform.rotation;

                // Notify spawn point of transition
                spawnPoint.OnLevelTransition(direction);
            }

            return Task.CompletedTask;
        }

        public void SaveGame(string slot = "default")
        {
            try
            {
                // Collect save data from all ISaveable objects
                var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
                foreach (var saveable in saveables) saveable.SaveState(CurrentSave);


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

        // Check if a save exists in the specified slot
        public bool HasSave(int slot = 0)
        {
            return ES3.KeyExists($"save_slot_{slot}");
        }


        public bool LoadGame(int slot = 0)
        {
            try
            {
                if (HasSave(slot))
                {
                    Debug.LogWarning($"No save file found in slot: {slot}");
                    return false;
                }

                // Load the complete save file
                CurrentSave = ES3.Load<SaveData>($"save_{slot}");

                Debug.Log($"CurrentSave: {CurrentSave}");
                Debug.Log($"CurrentSave.timestamp: {CurrentSave.timestamp}");
                Debug.Log($"CurrentSave.characterCreationData: {CurrentSave.characterCreationData}");
                Debug.Log(
                    $"CurrentSave.characterCreationData.selectedClass: {CurrentSave.characterCreationData.selectedClassName}");

                Debug.Log(
                    $"CurrentSave.characterCreationData.attributes: {CurrentSave.characterCreationData.attributes}");

                Debug.Log(
                    $"CurrentSave.characterCreationData.selectedTraits: {CurrentSave.characterCreationData.selectedTraitNames}");

                // Load character creation data into the gameplay scene if applicable
                if (CurrentSave.characterCreationData != null)
                    // Use characterCreationData to set initial player state
                    ApplyCharacterCreationData(CurrentSave.characterCreationData);

                // Load data into all ISaveable objects
                var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
                foreach (var saveable in saveables) saveable.LoadState(CurrentSave);

                Debug.Log($"Game loaded successfully from slot: {slot}");
                if (CurrentSave.characterCreationData != null)
                    Debug.Log($"Loaded Character: Class - {CurrentSave.characterCreationData.selectedClassName}");
                else
                    Debug.Log("No character creation data found in save file.");

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading game: {e.Message}");
                return false;
            }
        }

        // Clear current save data (used for starting new game)
        public void ClearCurrentSave()
        {
            CurrentSave = new SaveData(); // Reset save data to default
        }


        public void ApplyCharacterCreationData(CharacterCreationData creationData)
        {
            var playerGameObject = GameObject.FindGameObjectWithTag("Player");
            if (playerGameObject != null)
            {
                var playerStats = playerGameObject.GetComponent<PlayerStats>();

                // Call Initialize to load and apply character data
                playerStats.Initialize(creationData);

                Debug.Log("Character data applied in PlayerStats");
            }
            else
            {
                Debug.LogError("Player GameObject not found in Gameplay Scene.");
            }
        }
    }
}
