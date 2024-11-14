using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Project.Gameplay.Player.Inventory
{
    public class ManualItemPicker : ItemPicker
    {
        public PickupPromptManager pickupPromptManager;
        bool _isInRange;

        protected override void Start()
        {
            base.Start();
            if (pickupPromptManager == null)
                Debug.LogWarning("PickupPromptManager not found in the scene.");

            pickupPromptManager?.HidePickupPrompt();
        }

        void Update()
        {
            if (_isInRange && UnityEngine.Input.GetKeyDown(KeyCode.F))
            {
                if (Pickable())
                    Pick(Item.TargetInventoryName);
                else
                    ShowInventoryFullMessage();

                pickupPromptManager?.HidePickupPrompt();
            }
        }

        // Called by ProximityDetector when the player enters the pickup range
        public void OnPlayerEnter()
        {
            _isInRange = true;
            pickupPromptManager?.ShowPickupPrompt();
        }

        // Called by ProximityDetector when the player exits the pickup range
        public void OnPlayerExit()
        {
            _isInRange = false;
            pickupPromptManager?.HidePickupPrompt();
        }

        void ShowInventoryFullMessage()
        {
            Debug.Log("Inventory is full.");
        }
    }
}
