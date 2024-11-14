using System.Collections.Generic;
using Project.Gameplay.ItemManagement;
using Project.UI.HUD;
using UnityEngine;

public class PlayerItemPreviewManager : MonoBehaviour
{
    PreviewManager _previewManager;
    ItemPreviewTrigger currentItem;
    readonly List<ItemPreviewTrigger> nearbyItems = new();

    void Start()
    {
        _previewManager = FindObjectOfType<PreviewManager>();
        if (_previewManager == null) Debug.LogWarning("PreviewManager not found in the scene.");
    }

    void Update()
    {
        DisplayNearestItem();
    }

    void DisplayNearestItem()
    {
        if (nearbyItems.Count == 0 || _previewManager == null)
        {
            if (currentItem != null)
            {
                _previewManager.HidePreview();
                currentItem = null;
            }

            return;
        }

        // Sort to get the closest item
        nearbyItems.Sort(
            (a, b) =>
                Vector3.Distance(transform.position, a.transform.position)
                    .CompareTo(Vector3.Distance(transform.position, b.transform.position)));

        // Only update if the closest item has changed
        var closestItem = nearbyItems[0];
        if (closestItem != currentItem)
        {
            _previewManager.ShowPreview(closestItem.Item);
            currentItem = closestItem;
        }
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

            // Reset current item if it was removed
            if (currentItem == item)
            {
                _previewManager.HidePreview();
                currentItem = null;
            }
        }
    }
}
