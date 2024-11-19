// GameInitiator.cs

using System.Threading.Tasks;
using DunGen;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Core.SaveSystem;
using Project.Gameplay.DungeonGeneration;
using Project.Gameplay.Player;
using UnityEngine;

namespace Project.Core.GameInitialization
{
    public class GameInitiator : MonoBehaviour, MMEventListener<MMCameraEvent>
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

        void OnEnable()
        {
            // Listen for CharacterSwitch events
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }
        public void OnMMEvent(TopDownEngineEvent engineEvent)
        {
            if (engineEvent.EventType == TopDownEngineEventTypes.CharacterSwitch)
            {
                // Apply character creation data to the player
                NewSaveManager.Instance.ApplyCharacterCreationDataToPlayer();
                Debug.Log("CharacterSwitch event received. Applied CharacterCreationData to PlayerStats.");
            }
        }

        async Task InitializeCore()
        {
            // Load previous save or start a new game
            // if (!await LoadLastGame()) await StartNewGame();
            await StartNewGame();
        }

        async Task<bool> LoadLastGame()
        {
            return await Task.FromResult(_saveManager.LoadGame());
        }

        async Task StartNewGame()
        {
            var seed = Random.Range(0, int.MaxValue);
            await dungeonManager.GenerateNewDungeon(seed);

            // Spawn the player
            var initialSpawnPoint = FindObjectOfType<CheckPoint>();
            if (initialSpawnPoint == null)
            {
                Debug.LogError("No CheckPoint found for initial spawn!");
            }

            // var playerGameObject = GameObject.FindGameObjectWithTag("Player");
            // if (playerGameObject == null)
            // {
            //     Debug.LogError("No TopDownController found in scene!");
            //     return;
            // }
            //
            // // Apply CharacterCreationData to the player after it spawns
            // NewSaveManager.Instance.ApplyCharacterCreationDataToPlayer();
        }

        public void OnMMEvent(MMCameraEvent eventType)
        {
            if (eventType.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                Debug.Log("SetTargetCharacter event received. Applying CharacterCreationData...");
                ApplyCharacterCreationDataToPlayer(eventType.TargetCharacter.gameObject);
            }
            

        }
        
        private void ApplyCharacterCreationDataToPlayer(GameObject playerGameObject)
        {
            if (playerGameObject != null)
            {
                var playerStats = playerGameObject.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.Initialize(NewSaveManager.Instance.CurrentSave.characterCreationData);
                    Debug.Log("CharacterCreationData applied to PlayerStats.");
                }
                else
                {
                    Debug.LogError("PlayerStats component not found on Player GameObject.");
                }
            }
            else
            {
                Debug.LogError("Player GameObject not found.");
            }
        }
    }
}
