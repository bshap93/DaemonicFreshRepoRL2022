using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Project.Gameplay.Player.Inventory
{
    public class ManualPickableItem : MonoBehaviour
    {
        public InventoryItem Item; // The item to be picked up
        public int Quantity = 1;
        GameObject _collidingObject; // Reference to the player in range
        bool _isPlayerInRange;
        PickupPromptManager _pickupPromptManager;
        MoreMountains.InventoryEngine.Inventory _targetInventory; // Reference to the inventory in PortableSystems

        void Start()
        {
            _pickupPromptManager = FindObjectOfType<PickupPromptManager>();

            // Locate PortableSystems and retrieve the appropriate inventory
            var portableSystems = GameObject.Find("PortableSystems");
            if (portableSystems != null)
                // Assuming the first Inventory component in PortableSystems is the target.
                _targetInventory = portableSystems.GetComponentInChildren<MoreMountains.InventoryEngine.Inventory>();

            if (_targetInventory == null) Debug.LogWarning("Target inventory not found in PortableSystems.");
        }

        void Update()
        {
            if (_isPlayerInRange && UnityEngine.Input.GetKeyDown(KeyCode.F)) PickItem();
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                _isPlayerInRange = true;
                _collidingObject = collider.gameObject; // Store the player reference
                _pickupPromptManager?.ShowPickupPrompt();
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                _isPlayerInRange = false;
                _collidingObject = null; // Clear the player reference
                _pickupPromptManager?.HidePickupPrompt();
            }
        }

        void PickItem()
        {
            if (Item == null || _targetInventory == null)
            {
                Debug.LogWarning("Item or target inventory is null. Cannot pick up the item.");
                return;
            }

            if (_targetInventory.AddItem(Item, Quantity)) // Assume AddItem adds item and returns success
            {
                _pickupPromptManager?.HidePickupPrompt();
                Destroy(gameObject); // Remove the item from the scene after pickup
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
