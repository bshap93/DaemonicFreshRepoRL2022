using System;
using System.Collections.Generic;
using Project.Core.CharacterCreation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.CharacterCreation.Scripts
{
    public class CharacterCreationUI : MonoBehaviour
    {
        [SerializeField] GameObject classSelectionPanel; // Add this
        [Header("Panels")] [SerializeField] GameObject attributePanel;
        [SerializeField] GameObject traitsPanel;
        [SerializeField] GameObject confirmationPanel;

        // Add class selection specific fields
        [Header("Class Selection")] [SerializeField]
        Transform classContainer;
        [SerializeField] ClassSelectionButton classButtonPrefab;
        [SerializeField] TextMeshProUGUI classDescriptionText;

        [Header("Attribute Elements")] [SerializeField]
        TextMeshProUGUI pointsRemainingText;
        [SerializeField] List<AttributeRowUI> attributeRows;
        [SerializeField] int startingPoints = 10;


        [Header("Navigation")] [SerializeField]
        Button nextButton;
        [SerializeField] Button backButton;
        [SerializeField] Button confirmButton;

        CharacterCreationData _currentConfig;
        CreationStep _currentStep = CreationStep.ClassSelection; // Change initial step

        int _remainingPoints;

        CharacterClass? _selectedClass;

        void Start()
        {
            // Initialize with available classes
            var availableClasses = RunManager.Instance.GetAvailableClasses();
            InitializeClassSelection(availableClasses);


            // Validate required references
            if (pointsRemainingText == null)
                Debug.LogError("Points Remaining Text is not assigned in CharacterCreationUI");

            if (attributeRows == null || attributeRows.Count == 0)
                Debug.LogError("Attribute Rows are not assigned in CharacterCreationUI");

            _currentConfig = new CharacterCreationData();
            _remainingPoints = startingPoints;
            InitializeUI();
            ShowCurrentStep();
        }

        void InitializeUI()
        {
            // Set up navigation buttons
            nextButton.onClick.AddListener(OnNextClicked);
            backButton.onClick.AddListener(OnBackClicked);
            confirmButton.onClick.AddListener(OnConfirmClicked);

            // Set up initial state
            // Set up attribute rows
            foreach (var row in attributeRows) row.Initialize(OnAttributePointChanged);

            UpdatePointsDisplay();
        }

        void InitializeClassSelection(List<StartingClass> availableClasses)
        {
            if (availableClasses == null || availableClasses.Count == 0)
            {
                Debug.LogError("No available classes found!");
                return;
            }

            if (classContainer == null)
            {
                Debug.LogError("Class container not assigned!");
                return;
            }

            if (classButtonPrefab == null)
            {
                Debug.LogError("Class button prefab not assigned!");
                return;
            }

            // Clear any existing buttons
            foreach (Transform child in classContainer) Destroy(child.gameObject);

            // Create new buttons
            foreach (var classData in availableClasses)
                if (classData != null)
                {
                    var buttonGo = Instantiate(classButtonPrefab, classContainer);
                    var classButton = buttonGo.GetComponent<ClassSelectionButton>();
                    if (classButton != null) classButton.Setup(classData, OnClassButtonClicked);
                }
        }

        void OnClassButtonClicked(StartingClass classData)
        {
            _selectedClass = (CharacterClass)Enum.Parse(typeof(CharacterClass), classData.className);
            classDescriptionText.text = $"{classData.className}\n\n{classData.description}";
            nextButton.interactable = true; // Enable next button once class is selected
        }

        void OnAttributePointChanged(int pointChange)
        {
            _remainingPoints -= pointChange;
            UpdatePointsDisplay();

            // Enable/disable next button based on points remaining
            nextButton.interactable = _remainingPoints == 0;
        }

        void UpdatePointsDisplay()
        {
            if (pointsRemainingText != null)
                pointsRemainingText.text = $"Points Remaining: {_remainingPoints}";
            else
                Debug.LogError("Points remaining text not assigned!", this);

            if (attributeRows != null)
            {
                foreach (var row in attributeRows)
                    if (row != null)
                        row.SetIncrementButtonState(_remainingPoints > 0);
            }
            else
            {
                Debug.LogError("Attribute rows list not assigned!", this);
            }
        }

        void OnNextClicked()
        {
            _currentStep = _currentStep switch
            {
                CreationStep.ClassSelection => CreationStep.Attributes,
                CreationStep.Attributes => CreationStep.Traits,
                CreationStep.Traits => CreationStep.Confirmation,
                _ => _currentStep
            };

            ShowCurrentStep();
        }

        void OnBackClicked()
        {
            _currentStep = _currentStep switch
            {
                CreationStep.Attributes => CreationStep.ClassSelection,
                CreationStep.Traits => CreationStep.Attributes,
                CreationStep.Confirmation => CreationStep.Traits,
                _ => _currentStep
            };

            ShowCurrentStep();
        }

        void OnConfirmClicked()
        {
            // Save the final character configuration
            SaveCharacterStats();

            // Could load the game scene here
            // SceneManager.LoadScene("GameScene");
            Debug.Log("Character Creation Completed!");
        }

        void SaveCharacterStats()
        {
            // Save the attribute allocations
            var stats = new CharacterStats();

            foreach (var row in attributeRows)
                switch (row.statNameText.text)
                {
                    case "Strength":
                        stats.strength = row.CurrentPoints;
                        break;
                    case "Agility":
                        stats.agility = row.CurrentPoints;
                        break;
                    case "Endurance":
                        stats.endurance = row.CurrentPoints;
                        break;
                    case "Intelligence":
                        stats.intelligence = row.CurrentPoints;
                        break;
                    case "Intuition":
                        stats.intuition = row.CurrentPoints;
                        break;
                }

            _currentConfig.attributes = stats;
        }

        void ShowCurrentStep()
        {
            // First, deactivate all panels
            classSelectionPanel.SetActive(false);
            attributePanel.SetActive(false);
            traitsPanel.SetActive(false);
            confirmationPanel.SetActive(false);

            // Then activate the current panel
            switch (_currentStep)
            {
                case CreationStep.ClassSelection:
                    classSelectionPanel.SetActive(true);
                    break;
                case CreationStep.Attributes:
                    attributePanel.SetActive(true);
                    break;
                case CreationStep.Traits:
                    traitsPanel.SetActive(true);
                    break;
                case CreationStep.Confirmation:
                    confirmationPanel.SetActive(true);
                    break;
            }

            // Update navigation buttons
            backButton.gameObject.SetActive(_currentStep != CreationStep.ClassSelection);
            nextButton.gameObject.SetActive(_currentStep != CreationStep.Confirmation);
            confirmButton.gameObject.SetActive(_currentStep == CreationStep.Confirmation);

            // Set next button interactable state
            nextButton.interactable = _currentStep switch
            {
                CreationStep.ClassSelection => _selectedClass.HasValue,
                CreationStep.Attributes => _remainingPoints == 0,
                CreationStep.Traits => true, // You might want to add validation here later
                _ => false
            };
        }


        enum CreationStep
        {
            ClassSelection,
            Attributes,
            Traits,
            Confirmation
        }
    }
}
