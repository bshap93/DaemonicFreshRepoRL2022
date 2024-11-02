using System.Collections.Generic;
using System.Linq;
using Project.Core.CharacterCreation;
using TMPro;
using UnityEngine;

namespace Project.UI.CharacterCreation.Traits.Scripts
{
    public class TraitsPanel : MonoBehaviour
    {
        [SerializeField] Transform traitContainer; // Parent that holds trait list
        [SerializeField] GameObject traitPrefab; // Single trait UI prefab
        [SerializeField] TextMeshProUGUI descriptionText; // Description panel text
        [SerializeField] int maxTraitSelections = 3; // How many traits can be selected

        readonly List<CharacterCreation.TraitUI> traitUIs = new();
        CharacterClass currentClass;
        readonly List<CharacterTrait> selectedTraits = new();

        public void Initialize(List<CharacterTrait> availableTraits, CharacterClass characterClass)
        {
            currentClass = characterClass;
            ClearTraits();

            // Only show traits available for this class
            var validTraits = availableTraits.Where(t => t.IsAvailableForClass(characterClass)).ToList();

            foreach (var trait in validTraits)
            {
                var traitGO = Instantiate(traitPrefab, traitContainer);
                var traitUI = traitGO.GetComponent<TraitUI>();

                // Add visual indicator for class-specific traits
                var isClassSpecific = trait.isClassSpecific;

                traitUI.Initialize(
                    trait,
                    OnTraitSelected,
                    OnTraitInfoRequested,
                    isClassSpecific
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
            else if (selectedTraits.Count < maxTraitSelections)
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
            // Update description panel with trait info
            descriptionText.text = $"{trait.traitName}\n\n{trait.description}";

            // Could also show stat modifications, requirements, etc.
            var statChanges = GetStatModifierText(trait);
            descriptionText.text += $"\n\nEffects:\n{statChanges}";
        }

        string GetStatModifierText(CharacterTrait trait)
        {
            // Format the trait's stat modifications for display
            var text = "";
            foreach (var mod in trait.statModifiers)
                text += $"{mod.statName}: {(mod.value >= 0 ? "+" : "")}{mod.value}\n";

            return text;
        }
    }
}
