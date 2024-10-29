using System.Collections.Generic;

namespace Project.Gameplay.Player.Inventory.Models
{
// Inventory System
    [System.Serializable]
    public class InventoryItem
    {
        public string id;
        public string name;
        public int quantity;
        public Dictionary<string, object> properties;
    }
}
