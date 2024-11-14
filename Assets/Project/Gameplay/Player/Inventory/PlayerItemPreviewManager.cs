using System.Collections.Generic;
using Project.Gameplay.ItemManagement;
using Project.Prefabs.UI.PrefabRequiredScripts;
using UnityEngine;

namespace Project.Gameplay.Player.Inventory
{
    public class PlayerItemPreviewManager : MonoBehaviour
    {
        public TMPInventoryDetails InventoryDetails; // Reference to the preview UI script
        readonly List<ItemPreviewTrigger> nearbyItems = new();

        void Update()
        {
            // Continuously update the preview based on the nearest item
            DisplayNearestItem();
        }

        void DisplayNearestItem()
        {
            if (nearbyItems.Count == 0)
            {
                InventoryDetails.HidePreview();
                return;
            }

            // Sort items by distance from the player
            nearbyItems.Sort(
                (a, b) =>
                    Vector3.Distance(transform.position, a.transform.position)
                        .CompareTo(Vector3.Distance(transform.position, b.transform.position)));

            // Display the closest item's preview
            InventoryDetails.DisplayPreview(nearbyItems[0].Item);
        }

        public void RegisterItem(ItemPreviewTrigger item)
        {
            if (!nearbyItems.Contains(item)) nearbyItems.Add(item);
        }

        public void UnregisterItem(ItemPreviewTrigger item)
        {
            if (nearbyItems.Contains(item))
            {
                nearbyItems.Remove(item);
                DisplayNearestItem(); // Update preview after removing an item
            }
        }
    }
}
