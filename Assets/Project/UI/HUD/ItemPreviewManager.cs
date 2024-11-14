using MoreMountains.InventoryEngine;
using Project.Prefabs.UI.PrefabRequiredScripts;
using UnityEngine;

namespace Project.UI.HUD
{
    public class PreviewManager : MonoBehaviour
    {
        public TMPInventoryDetails InventoryDetails;

        public void ShowPreview(InventoryItem item)
        {
            if (InventoryDetails != null)
            {
                Debug.Log("Showing preview for item: " + item.ItemName);
                InventoryDetails.DisplayPreview(item);

                // Make sure CanvasGroup is visible
                var canvasGroup = InventoryDetails.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }

        public void HidePreview()
        {
            if (InventoryDetails != null)
            {
                var canvasGroup = InventoryDetails.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }
    }
}
