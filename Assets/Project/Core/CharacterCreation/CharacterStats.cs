using System;
using System.Collections.Generic;

namespace Project.Core.CharacterCreation
{
    [Serializable]
    public class CharacterStats
    {
        public float health;
        public float stamina;
        public float strength;
        public float agility;
        public float endurance;
        public float intelligence;
        public float intuition;


        // Add other base stats as needed

        public Dictionary<string, float> GetModifiedStats(List<CharacterTrait> traits)
        {
            var modified = new Dictionary<string, float>();
            // Apply trait modifications
            return modified;
        }
    }
}
