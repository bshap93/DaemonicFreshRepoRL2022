using System.Collections.Generic;
using UnityEngine;

namespace Project.Core.CharacterCreation
{
    public class RunManager : MonoBehaviour
    {
        [Header("Configuration")] [SerializeField]
        int startingAttributePoints = 10;
        [SerializeField] List<CharacterTrait> availableTraits;
        [SerializeField] List<StartingClass> availableClasses;
        public static RunManager Instance { get; private set; }

        public RunConfig CurrentRun { get; private set; }
        public int StartingAttributePoints => startingAttributePoints;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadTraitsAndClasses();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void LoadTraitsAndClasses()
        {
            // Load trait and class definitions from ScriptableObjects
            availableTraits = new List<CharacterTrait>(Resources.LoadAll<CharacterTrait>("Traits"));
            availableClasses = new List<StartingClass>(Resources.LoadAll<StartingClass>("Classes"));
        }

        public void StartNewRun(CharacterCreationData creationData)
        {
            CurrentRun = new RunConfig
            {
                seed = Random.Range(int.MinValue, int.MaxValue),
                baseStats = creationData.attributes,
                traits = creationData.selectedTraits,
                startingClass = creationData.selectedClass
            };

            SaveRun();
            // Start the actual game/level
            // SceneManager.LoadScene("GameScene");
        }

        public void SaveRun()
        {
            ES3.Save("currentRun", CurrentRun);
        }

        public bool LoadRun()
        {
            if (ES3.KeyExists("currentRun"))
            {
                CurrentRun = ES3.Load<RunConfig>("currentRun");
                return true;
            }

            return false;
        }

        public List<CharacterTrait> GetAvailableTraits()
        {
            return availableTraits;
        }

        public List<StartingClass> GetAvailableClasses()
        {
            return availableClasses;
        }
    }
}
