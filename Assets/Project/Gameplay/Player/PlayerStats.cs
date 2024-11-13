// PlayerStats.cs

using System;
using System.Collections.Generic;
using Project.Core.CharacterCreation;
using UnityEngine;

namespace Project.Gameplay.Player
{
    [Serializable]
    public class PlayerStats : MonoBehaviour
    {
        // Final stats after applying attributes and traits
        public float maxHealth;
        public float currentHealth;
        public float moveSpeed;
        public float attackPower;
        public float defense;
        // Base stats directly from the selected class
        Dictionary<StatType, float> baseStats = new();

        // Reference to character creation data for easier tracking
        CharacterCreationData characterCreationData;

        // Initialization with character creation data
        public void Initialize(CharacterCreationData creationData)
        {
            characterCreationData = creationData;

            // Apply base stats from class
            SetClass(characterCreationData.selectedClass);

            // Apply allocated attributes
            ApplyAttributes(characterCreationData.attributes);

            // Apply selected traits
            SetTraits(characterCreationData.selectedTraits);

            // Set current health to max health at start
            currentHealth = maxHealth;
        }

        public void SetClass(StartingClass selectedClass)
        {
            if (selectedClass == null)
            {
                Debug.LogWarning("Selected class is null. Cannot set base stats.");
                return;
            }

            // Populate baseStats dictionary
            foreach (var stat in selectedClass.baseStats) baseStats[stat.Key] = stat.Value;

            // Calculate stats based on base values
            maxHealth = baseStats.ContainsKey(StatType.Endurance) ? baseStats[StatType.Endurance] * 10 : 100;
            moveSpeed = baseStats.ContainsKey(StatType.Agility) ? baseStats[StatType.Agility] * 0.5f : 5;
            attackPower = baseStats.ContainsKey(StatType.Strength) ? baseStats[StatType.Strength] * 2 : 10;
            defense = baseStats.ContainsKey(StatType.Endurance) ? baseStats[StatType.Endurance] : 5;

            Debug.Log($"Class set to: {selectedClass.className} with base stats applied.");
        }

        public void ApplyAttributes(CharacterStats attributes)
        {
            // Apply attribute values on top of base stats
            maxHealth += attributes.endurance * 10;
            moveSpeed += attributes.agility * 0.5f;
            attackPower += attributes.strength * 2;
            defense += attributes.endurance;

            Debug.Log("Attributes applied to player stats");
        }

        public void SetTraits(List<CharacterTrait> selectedTraits)
        {
            foreach (var trait in selectedTraits)
            {
                foreach (var modifier in trait.statModifiers)
                    if (modifier.type == CharacterTrait.ModifierType.Additive)
                    {
                        if (modifier.statName == "moveSpeed") moveSpeed += modifier.value;
                        else if (modifier.statName == "defense") defense += modifier.value;
                    }

                // Handle other types of modifiers here if needed
                Debug.Log($"Trait applied: {trait.traitName}");
            }
        }

        public void DisplayStats()
        {
            Debug.Log(
                $"Character Stats - MaxHealth: {maxHealth}, CurrentHealth: {currentHealth}, MoveSpeed: {moveSpeed}, AttackPower: {attackPower}, Defense: {defense}");
        }

        public Dictionary<string, float> GetAllStats()
        {
            return new Dictionary<string, float>
            {
                { "Max Health", maxHealth },
                { "Current Health", currentHealth },
                { "Move Speed", moveSpeed },
                { "Attack Power", attackPower },
                { "Defense", defense }
            };
        }
    }
}
