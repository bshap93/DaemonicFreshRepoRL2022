using System.Collections.Generic;

namespace Project.Gameplay.Player
{
// Player Stats System
    [System.Serializable]
    public class PlayerStats
    {
        public float maxHealth;
        public float currentHealth;
        public float moveSpeed;
        public float attackPower;
        public float defense;
    
        public Dictionary<string, float> GetCurrentStats()
        {
            return new Dictionary<string, float>
            {
                { "maxHealth", maxHealth },
                { "currentHealth", currentHealth },
                { "moveSpeed", moveSpeed },
                { "attackPower", attackPower },
                { "defense", defense }
            };
        }
    }
}
