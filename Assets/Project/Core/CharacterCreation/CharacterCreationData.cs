using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Core.CharacterCreation
{
    [Serializable]
    public class CharacterCreationData
    {
        public string characterName;
        public List<CharacterTrait> selectedTraits = new();
        public int remainingPoints;
        public StartingClass selectedClass;
        public CharacterStats attributes = new();
    }

    public enum StatType
    {
        Strength,
        Agility,
        Endurance,
        Intelligence,
        Intuition
    }

    [CreateAssetMenu(fileName = "New Starting Class", menuName = "Roguelike/Starting Class")]
    public class StartingClass : ScriptableObject
    {
        public string className;
        public string description;
        public Sprite classIcon;
        public List<CharacterTrait> defaultTraits = new();
        public Dictionary<StatType, int> baseStats = new();
    }

    public enum CharacterClass
    {
        Automaton,
        Luddite
    }


    [CreateAssetMenu(fileName = "New Trait", menuName = "Roguelike/Character Trait")]
    public class CharacterTrait : ScriptableObject
    {
        public enum ModifierType
        {
            Additive,
            Multiplicative
        }

        public string traitName;
        public string description;
        public Sprite icon;
        public List<StatModifier> statModifiers = new();
        public List<string> specialEffects = new();

        [Header("Class Restrictions")] public bool isClassSpecific;
        [Tooltip("If class specific, which classes can use this trait")]
        public List<CharacterClass> availableForClasses = new();

        public bool IsAvailableForClass(CharacterClass characterClass)
        {
            return !isClassSpecific || availableForClasses.Contains(characterClass);
        }

        [Serializable]
        public class StatModifier
        {
            public string statName;
            public float value;
            public ModifierType type;
        }
    }


    [Serializable]
    public class StatModifier
    {
        public string statName;
        public int value;
    }
}
