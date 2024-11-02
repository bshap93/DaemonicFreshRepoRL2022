using System;
using System.Collections.Generic;
using System.Linq;
using Project.Core.CharacterCreation;
using Project.UI.CharacterCreation.Classes.Scripts;
using Project.UI.CharacterCreation.Traits.Scripts;
using Project.UI.CharacterCreation.UIElements.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Project.UI.CharacterCreation.Scripts
{
    public class CharacterCreationUI : MonoBehaviour
    {
        [Header("Panels")] [SerializeField] GameObject attributePanel;
        [FormerlySerializedAs("traitsPanelGO")] [FormerlySerializedAs("traitsPanel")] [SerializeField]
        GameObject traitsPanelGo;
        [SerializeField] GameObject confirmationPanel;

        [Header("Class Selection")] [SerializeField]
        Transform classesContainer;
        [SerializeField] ClassSelectionButton classButtonPrefab;

        [Header("Attribute Elements")] [SerializeField]
        TextMeshProUGUI pointsRemainingText;
        [SerializeField] List<AttributeRowUI> attributeRows;

        [Header("Trait Elements")] [SerializeField]
        Transform traitContainer;
        [SerializeField] GameObject traitPrefab;
        [SerializeField] TextMeshProUGUI traitDescriptionText;

        [Header("Navigation")] [SerializeField]
        Button nextButton;
        [SerializeField] Button backButton;
        [SerializeField] Button confirmButton;

        [SerializeField] ClassSelectionPanel classPanel;
        [SerializeField] TraitsPanel traitsPanel;
        [SerializeField] int maxTraitSelections = 2;

        CharacterClass _selectedClass;
        CharacterCreationData currentConfig;

        CreationStep currentStep = CreationStep.Attributes;
        int remainingPoints;

        [Header("Traits")] [SerializeField] TraitsListUI traitsListUI;

        void Start()
        {
            var availableClasses = RunManager.Instance.GetAvailableClasses();
            classPanel.Initialize(availableClasses, OnClassSelected);
            currentConfig = new CharacterCreationData();
            InitializeUI();
            ShowCurrentStep();

            // Hide traits until class is selected
            traitsPanel.gameObject.SetActive(false);
        }

        void InitializeUI()
        {
            // Set up attribute rows
            foreach (var row in attributeRows) row.OnPointsChanged += UpdateRemainingPoints;

            // Set up navigation buttons
            nextButton.onClick.AddListener(OnNextClicked);
            backButton.onClick.AddListener(OnBackClicked);
            confirmButton.onClick.AddListener(OnConfirmClicked);

            UpdateRemainingPoints();
        }

        void ShowCurrentStep()
        {
            attributePanel.SetActive(currentStep == CreationStep.Attributes);
            traitsPanelGo.SetActive(currentStep == CreationStep.Traits);
            confirmationPanel.SetActive(currentStep == CreationStep.Confirmation);

            backButton.gameObject.SetActive(currentStep != CreationStep.Attributes);
            nextButton.gameObject.SetActive(currentStep != CreationStep.Confirmation);
            confirmButton.gameObject.SetActive(currentStep == CreationStep.Confirmation);
        }

        void UpdateRemainingPoints()
        {
            var used = attributeRows.Sum(row => row.CurrentPoints);
            var remaining = RunManager.Instance.StartingAttributePoints - used;
            pointsRemainingText.text = $"Points Remaining: {remaining}";
            nextButton.interactable = remaining == 0;
        }

        void OnNextClicked()
        {
            if (currentStep == CreationStep.Attributes)
            {
                currentStep = CreationStep.Traits;
                PopulateTraits();
            }
            else if (currentStep == CreationStep.Traits)
            {
                currentStep = CreationStep.Confirmation;
                ShowConfirmation();
            }

            ShowCurrentStep();
        }
        void ShowConfirmation()
        {
            throw new NotImplementedException();
        }
        void PopulateTraits()
        {
            throw new NotImplementedException();
        }

        void OnBackClicked()
        {
            if (currentStep == CreationStep.Traits)
                currentStep = CreationStep.Attributes;
            else if (currentStep == CreationStep.Confirmation)
                currentStep = CreationStep.Traits;

            ShowCurrentStep();
        }

        void OnConfirmClicked()
        {
            // Save character configuration
            RunManager.Instance.StartNewRun(currentConfig);
            // Transition to game
            // SceneManager.LoadScene("GameScene");
        }

        void OnClassSelected(CharacterClass characterClass)
        {
            _selectedClass = characterClass;

            // Show and initialize traits panel with class-specific traits
            traitsPanel.gameObject.SetActive(true);
            traitsPanel.Initialize(RunManager.Instance.GetAvailableTraits(), characterClass);
        }

        enum CreationStep
        {
            Attributes,
            Traits,
            Confirmation
        }
    }
}
