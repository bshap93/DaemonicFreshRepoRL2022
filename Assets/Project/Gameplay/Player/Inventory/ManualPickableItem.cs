using MoreMountains.TopDownEngine;
using Project.Gameplay.ItemManagement;
using UnityEngine;

namespace Project.Gameplay.Player.Inventory
{
    public class ManualPickableItem : PickableItem
    {
        public bool DestroyOnPick = true;
        bool _isPlayerInRange;
        PickupPromptManager _pickupPromptManager;

        protected override void Start()
        {
            base.Start();
            _pickupPromptManager = FindObjectOfType<PickupPromptManager>();
        }

        void Update()
        {
            if (_isPlayerInRange && UnityEngine.Input.GetKeyDown(KeyCode.F)) // Replace "F" with your preferred key
                PickItem(_collidingObject); // Use _collidingObject to specify the picker
        }

        public override void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                _isPlayerInRange = true;
                _collidingObject = collider.gameObject; // Set _collidingObject to avoid null reference
                _pickupPromptManager?.ShowPickupPrompt();
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                _isPlayerInRange = false;
                _pickupPromptManager?.HidePickupPrompt();
                _collidingObject = null; // Clear _collidingObject when player exits
            }
        }

        public override void PickItem(GameObject picker)
        {
            if (CheckIfPickable())
            {
                base.PickItem(picker);

                // Unregister from the preview manager before destroying
                var previewManager = FindObjectOfType<PlayerItemPreviewManager>();
                if (previewManager != null) previewManager.UnregisterItem(GetComponent<ItemPreviewTrigger>());

                if (_pickupPromptManager != null) _pickupPromptManager.HidePickupPrompt();

                // Destroy the item to fully remove it
                Destroy(gameObject);
            }
            else
            {
                ShowInventoryFullMessage();
            }
        }

        void ShowInventoryFullMessage()
        {
            Debug.Log("Inventory is full or item cannot be picked up.");
            // Additional UI feedback for full inventory, if needed
        }
    }
}
