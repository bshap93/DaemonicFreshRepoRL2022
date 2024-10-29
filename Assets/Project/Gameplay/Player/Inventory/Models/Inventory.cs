using System.Collections.Generic;

namespace Project.Gameplay.Player.Inventory.Models
{
    [System.Serializable]
    public class Inventory
    {
        public List<InventoryItem> items = new List<InventoryItem>();
        public int maxSlots = 20;
    
        public List<InventoryItem> GetContents()
        {
            return new List<InventoryItem>(items);
        }
    }
}
