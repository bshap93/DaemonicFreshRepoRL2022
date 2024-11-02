using System.Collections.Generic;
using System.Linq;
using Project.Core.CharacterCreation;
using Project.UI.CharacterCreation.Traits.Scripts;
using TMPro;
using UnityEngine;

public class TraitsListUI : MonoBehaviour
{
    [SerializeField] Transform traitContainer;
    [SerializeField] GameObject traitPrefab;
    [SerializeField] TextMeshProUGUI descriptionText;
    int maxSelections;
    readonly List<CharacterTrait> selectedTraits = new();

    readonly List<TraitUI> traitUIs = new();

    public void Initialize(CharacterClass characterClass, int maxTraitSelections)
    {
        maxSelections = maxTraitSelections;
        ClearTraits();

        // Get available traits for this class from RunManager
        var availableTraits = RunManager.Instance.GetAvailableTraits()
            .Where(t => t.IsAvailableForClass(characterClass))
            .ToList();

        foreach (var trait in availableTraits)
        {
            var traitGO = Instantiate(traitPrefab, traitContainer);
            var traitUI = traitGO.GetComponent<TraitUI>();

            traitUI.Initialize(
                trait,
                OnTraitSelected,
                OnTraitInfoRequested,
                trait.isClassSpecific
            );

            traitUIs.Add(traitUI);
        }
    }

    void ClearTraits()
    {
        foreach (var traitUI in traitUIs) Destroy(traitUI.gameObject);
        traitUIs.Clear();
        selectedTraits.Clear();
    }

    void OnTraitSelected(CharacterTrait trait)
    {
        if (selectedTraits.Contains(trait))
        {
            selectedTraits.Remove(trait);
        }
        else if (selectedTraits.Count < maxSelections)
        {
            selectedTraits.Add(trait);
        }
        else
        {
            // Too many traits selected, uncheck the toggle
            var traitUI = traitUIs.Find(ui => ui.Trait == trait);
            traitUI.SetToggleWithoutNotify(false);
        }
    }

    void OnTraitInfoRequested(CharacterTrait trait)
    {
        if (descriptionText != null)
        {
            descriptionText.text = $"{trait.traitName}\n\n{trait.description}";

            // Add stat modifications to description if any exist
            if (trait.statModifiers.Any())
            {
                descriptionText.text += "\n\nModifies:";
                foreach (var mod in trait.statModifiers)
                {
                    var prefix = mod.value >= 0 ? "+" : "";
                    descriptionText.text += $"\n{mod.statName}: {prefix}{mod.value}";
                }
            }
        }
    }

    public List<CharacterTrait> GetSelectedTraits()
    {
        return new List<CharacterTrait>(selectedTraits);
    }
}
