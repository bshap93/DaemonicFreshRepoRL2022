using System.Collections.Generic;
using System.Linq;
using Project.Core.CharacterCreation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationUI : MonoBehaviour
{
    [Header("Panels")] [SerializeField] GameObject attributePanel;
    [SerializeField] GameObject traitsPanel;
    [SerializeField] GameObject confirmationPanel;

    [Header("Attribute Elements")]
    [SerializeField] private TextMeshProUGUI pointsRemainingText;
    [SerializeField] private List<AttributeRowUI> attributeRows;
    [SerializeField] private int startingPoints = 10;
    
    private int remainingPoints;


    [Header("Navigation")] [SerializeField]
    Button nextButton;
    [SerializeField] Button backButton;
    [SerializeField] Button confirmButton;

    CharacterCreationData currentConfig;
    CreationStep currentStep = CreationStep.Attributes;

    void Start()
    {
        currentConfig = new CharacterCreationData();
        remainingPoints = startingPoints;
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
        foreach (var row in attributeRows)
        {
            row.Initialize(OnAttributePointChanged);
        }

        UpdatePointsDisplay();
    }
    
    private void OnAttributePointChanged(int pointChange)
    {
        remainingPoints -= pointChange;
        UpdatePointsDisplay();
        
        // Enable/disable next button based on points remaining
        nextButton.interactable = remainingPoints == 0;
    }

    private void UpdatePointsDisplay()
    {
        pointsRemainingText.text = $"Points Remaining: {remainingPoints}";
        
        // Update increment buttons based on remaining points
        foreach (var row in attributeRows)
        {
            row.GetComponent<Button>().interactable = remainingPoints > 0;
        }
    }

    void OnNextClicked()
    {
        if (currentStep == CreationStep.Attributes)
        {
            // Only proceed if all points are allocated
            var usedPoints = attributeRows.Sum(row => row.CurrentPoints);
            if (usedPoints == RunManager.Instance.StartingAttributePoints) currentStep = CreationStep.Traits;
        }
        else if (currentStep == CreationStep.Traits)
        {
            currentStep = CreationStep.Confirmation;
        }

        ShowCurrentStep();
    }

    void OnBackClicked()
    {
        if (currentStep == CreationStep.Traits)
            currentStep = CreationStep.Attributes;
        else if (currentStep == CreationStep.Confirmation) currentStep = CreationStep.Traits;

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

        currentConfig.attributes = stats;
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


    enum CreationStep
    {
        Attributes,
        Traits,
        Confirmation
    }
}
