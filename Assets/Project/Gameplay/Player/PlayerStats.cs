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
        // Base Stats
        [SerializeField] float maxHealth;
        [SerializeField] float currentHealth;
        [SerializeField] float moveSpeed;
        [SerializeField] float attackPower;
        [SerializeField] float defense;

        // Attributes assigned by the player
        [SerializeField] int strength;
        [SerializeField] int agility;
        [SerializeField] int endurance;
        [SerializeField] int intelligence;
        [SerializeField] int intuition;

        // Character creation data for reference
        [SerializeField] string playerClass; // Stores the class name
        [SerializeField] List<string> chosenTraits; // Stores the trait names

        // Runtime references to ScriptableObjects for class and traits
        StartingClass startingClass;
        List<CharacterTrait> traits = new();

        public void Initialize(CharacterCreationData creationData)
        {
            // Store names for reference
            playerClass = creationData.selectedClassName;
            chosenTraits = new List<string>(creationData.selectedTraitNames);

            // Load StartingClass and CharacterTrait ScriptableObjects
            LoadStartingClass(playerClass);
            LoadTraits(chosenTraits);

            // Assign attributes directly from creation data
            SetAttributes(creationData);

            // Set class and base stats
            ApplyBaseStatsFromClass();
            ApplyAttributesToBaseStats();

            // Initialize current health to max health
            currentHealth = maxHealth;

            Debug.Log(
                $"Initialized Player Stats: Class={playerClass}, MaxHealth={maxHealth}, MoveSpeed={moveSpeed}, AttackPower={attackPower}, Defense={defense}");
        }

        void LoadStartingClass(string className)
        {
            startingClass = Resources.Load<StartingClass>($"Classes/{className}");
            if (startingClass == null) Debug.LogError($"Class {className} not found in Resources.");
        }

        void LoadTraits(List<string> traitNames)
        {
            traits = new List<CharacterTrait>();
            foreach (var traitName in traitNames)
            {
                var trait = Resources.Load<CharacterTrait>($"Traits/{traitName}");
                if (trait != null)
                    traits.Add(trait);
                else
                    Debug.LogError($"Trait {traitName} not found in Resources.");
            }
        }

        void SetAttributes(CharacterCreationData creationData)
        {
            strength = creationData.attributes.strength;
            agility = creationData.attributes.agility;
            endurance = creationData.attributes.endurance;
            intelligence = creationData.attributes.intelligence;
            intuition = creationData.attributes.intuition;
        }

        void ApplyBaseStatsFromClass()
        {
            if (startingClass == null)
            {
                Debug.LogWarning("Starting class is null; cannot apply base stats.");
                return;
            }

            // Apply base stats from the class
            if (startingClass.baseStats.TryGetValue(StatType.Endurance, out var enduranceBase))
                maxHealth = enduranceBase * 10;

            if (startingClass.baseStats.TryGetValue(StatType.Agility, out var agilityBase))
                moveSpeed = agilityBase * 0.5f;

            if (startingClass.baseStats.TryGetValue(StatType.Strength, out var strengthBase))
                attackPower = strengthBase * 2;

            if (startingClass.baseStats.TryGetValue(StatType.Endurance, out var defenseBase))
                defense = defenseBase;
        }

        void ApplyAttributesToBaseStats()
        {
            // Modify base stats by adding attribute bonuses
            maxHealth += endurance * 10;
            moveSpeed += agility * 0.5f;
            attackPower += strength * 2;
            defense += endurance;

            Debug.Log(
                $"Attributes applied to base stats: MaxHealth={maxHealth}, MoveSpeed={moveSpeed}, AttackPower={attackPower}, Defense={defense}");
        }

        public void ApplyTraits()
        {
            foreach (var trait in traits)
            {
                foreach (var modifier in trait.statModifiers)
                    if (modifier.type == CharacterTrait.ModifierType.Additive)
                    {
                        if (modifier.statName == "moveSpeed") moveSpeed += modifier.value;
                        else if (modifier.statName == "defense") defense += modifier.value;
                    }

                Debug.Log($"Trait applied: {trait.traitName}");
            }
        }

        public void DisplayStats()
        {
            Debug.Log(
                $"Class: {playerClass}, Max Health: {maxHealth}, Current Health: {currentHealth}, Move Speed: {moveSpeed}, Attack Power: {attackPower}, Defense: {defense}");

            Debug.Log(
                $"Attributes - Strength: {strength}, Agility: {agility}, Endurance: {endurance}, Intelligence: {intelligence}, Intuition: {intuition}");

            Debug.Log($"Chosen Traits: {string.Join(", ", chosenTraits)}");
        }
    }
}
