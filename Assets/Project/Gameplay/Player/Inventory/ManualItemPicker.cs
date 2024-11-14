using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Project.Gameplay.Player.Inventory
{
    public class ManualItemPicker : ItemPicker
    {
        bool _isInRange;
        public GameObject PickupPrompt;


        // Called by ProximityDetector when the player enters the pickup range
        public void OnPlayerEnter()
        {
            _isInRange = true;
            ShowPickupPrompt();
        }

        // Called by ProximityDetector when the player exits the pickup range
        public void OnPlayerExit()
        {
            _isInRange = false;
            HidePickupPrompt();
        }

        void Update()
        {
            if (_isInRange && UnityEngine.Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("Attempting to pick up item...");

                if (Pickable())
                {
                    Pick(Item.TargetInventoryName);
                    Debug.Log("Item picked up successfully.");
                }
                else
                {
                    ShowInventoryFullMessage();
                    Debug.Log("Inventory is full.");
                }

                HidePickupPrompt();
            }
        }




        void ShowInventoryFullMessage()
        {
            // Show a message if the item can't be picked up
        }
        
        void ShowPickupPrompt()
        {
            if (PickupPrompt != null)
            {
                PickupPrompt.SetActive(true);
            }
        }

        void HidePickupPrompt()
        {
            if (PickupPrompt != null)
            {
                PickupPrompt.SetActive(false);
            }
        }

    }
}
