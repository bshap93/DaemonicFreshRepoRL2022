using System;
using System.Collections.Generic;
using Project.Core.CharacterCreation;
using Project.UI.CharacterCreation.UIElements.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.CharacterCreation.Scripts
{
    public class CharacterCreationUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject classSelectionPanel;
        [SerializeField] private GameObject attributePanel;
        [SerializeField] private GameObject traitsPanel;
        [SerializeField] private TraitsPanel traitsPanelScript;
        [SerializeField] private ConfirmationPanel confirmationPanel;

        [Header("Class Selection")]
        [SerializeField] private Transform classContainer;
        [SerializeField] private ClassSelectionButton classButtonPrefab;
        [SerializeField] private VerticalLayoutGroup detailsPanel;
        [SerializeField] private TextMeshProUGUI classNameText;
        [SerializeField] private TextMeshProUGUI classDescriptionText;
        [SerializeField] private Image selectedClassIcon;

        [Header("Attribute Elements")]
        [SerializeField] private TextMeshProUGUI pointsRemainingText;
        [SerializeField] private List<AttributeRowUI> attributeRows;
        [SerializeField] private int startingPoints = 10;

        [Header("Navigation")]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button confirmButton;

        private CharacterCreationData _currentConfig;
        private CreationStep _currentStep = CreationStep.ClassSelection;
        private int _remainingPoints;
        private StartingClass _selectedClass;  // Change from CharacterClass? to StartingClass
        private readonly List<ClassSelectionButton> classButtons = new();




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

            if (traitsPanelScript == null)
                Debug.LogError("TraitsPanel script not assigned!");

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

            // Clear existing buttons
            foreach (var button in classButtons)
                if (button != null)
                    Destroy(button.gameObject);

            classButtons.Clear();

            // Create new buttons
            foreach (var classData in availableClasses)
                if (classData != null)
                {
                    var buttonGO = Instantiate(classButtonPrefab, classContainer);
                    var classButton = buttonGO.GetComponent<ClassSelectionButton>();
                    if (classButton != null)
                    {
                        classButton.Setup(classData, OnClassButtonClicked);
                        classButtons.Add(classButton);
                    }
                }

            // Hide details panel initially
            if (detailsPanel != null) detailsPanel.gameObject.SetActive(false);
        }

        private void OnClassButtonClicked(StartingClass classData)
        {
            if (classData == null) return;

            // Update selected button visuals
            foreach (var button in classButtons) button.SetSelected(false);

            _selectedClass = classData;  // Store the entire StartingClass

            // Update UI
            if (detailsPanel != null) detailsPanel.gameObject.SetActive(true);

            if (classNameText != null) classNameText.text = classData.className.ToString();

            if (classDescriptionText != null) classDescriptionText.text = classData.description;

            if (selectedClassIcon != null)
            {
                selectedClassIcon.sprite = classData.classIcon;
                selectedClassIcon.preserveAspect = true;
            }

            if (nextButton != null) nextButton.interactable = true;
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

        private void OnNextClicked()
        {
            switch (_currentStep)
            {
                case CreationStep.ClassSelection:
                    if (_selectedClass != null)
                    {
                        _currentStep = CreationStep.Attributes;
                    }
                    break;

                case CreationStep.Attributes:
                    if (_remainingPoints == 0)
                    {
                        _currentStep = CreationStep.Traits;
                        traitsPanelScript.Initialize(RunManager.Instance.GetAvailableTraits(), _selectedClass.ClassType);
                    }
                    break;

                case CreationStep.Traits:
                    if (traitsPanelScript.HasRequiredTraits())
                    {
                        _currentStep = CreationStep.Confirmation;
                        ShowCharacterSummary();
                    }
                    else
                    {
                        Debug.Log("Please select exactly 2 traits before continuing.");
                    }
                    break;
            }

            ShowCurrentStep();
        }

        private void ShowCharacterSummary()
        {
            _currentConfig.selectedClass = _selectedClass;   // Store the enum
            _currentConfig.attributes = GatherAttributeData();
            _currentConfig.selectedTraits = traitsPanelScript.GetSelectedTraits();

            confirmationPanel.DisplayCharacterSummary(_currentConfig);
        }

        CharacterStats GatherAttributeData()
        {
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

            return stats;
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
            confirmationPanel.gameObject.SetActive(false);  // Access the GameObject

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
                    confirmationPanel.gameObject.SetActive(true);  // Access the GameObject
                    break;
            }

            // Update navigation buttons
            backButton.gameObject.SetActive(_currentStep != CreationStep.ClassSelection);
            nextButton.gameObject.SetActive(_currentStep != CreationStep.Confirmation);
            confirmButton.gameObject.SetActive(_currentStep == CreationStep.Confirmation);

            // Set next button interactable state
            nextButton.interactable = _currentStep switch
            {
                CreationStep.ClassSelection => _selectedClass != null,  // Check if StartingClass is assigned
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
