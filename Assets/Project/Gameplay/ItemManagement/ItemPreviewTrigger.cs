using MoreMountains.InventoryEngine;
using Project.Gameplay.Player.Inventory;
using UnityEngine;

namespace Project.Gameplay.ItemManagement
{
    public class ItemPreviewTrigger : MonoBehaviour
    {
        public InventoryItem Item;  // Assign the InventoryItem to display

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerItemPreviewManager previewManager = other.GetComponent<PlayerItemPreviewManager>();
                previewManager.RegisterItem(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerItemPreviewManager previewManager = other.GetComponent<PlayerItemPreviewManager>();
                previewManager.UnregisterItem(this);
            }
        }
    }
}
