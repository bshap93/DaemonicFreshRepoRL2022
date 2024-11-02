using System;
using System.Collections.Generic;
using System.Linq;
using Project.Core.CharacterCreation;
using Project.UI.CharacterCreation.UIElements.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.CharacterCreation
{
    public class CharacterCreationUI : MonoBehaviour
    {
        [Header("Panels")] [SerializeField] GameObject attributePanel;
        [SerializeField] GameObject traitsPanel;
        [SerializeField] GameObject confirmationPanel;

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
        CharacterCreationData currentConfig;

        CreationStep currentStep = CreationStep.Attributes;

        void Start()
        {
            currentConfig = new CharacterCreationData();
            InitializeUI();
            ShowCurrentStep();
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
            traitsPanel.SetActive(currentStep == CreationStep.Traits);
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

        enum CreationStep
        {
            Attributes,
            Traits,
            Confirmation
        }
    }
}
