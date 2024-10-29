using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        public SaveData CurrentSave { get; private set; }
    
        void Awake()
        {
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