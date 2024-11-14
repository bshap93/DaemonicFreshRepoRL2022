using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Project.Gameplay.Player.Inventory
{
    public class ManualItemPicker : MonoBehaviour
    {
        public InventoryItem Item; // The item to be picked up
        public int Quantity = 1;

        [Header("Feedbacks")] [Tooltip("Feedbacks to play when the item is picked up")]
        public MMFeedbacks PickedMMFeedbacks; // Feedbacks to play when the item is picked up
        GameObject _collidingObject; // Reference to the player in range
        bool _isInRange;
        PickupPromptManager _pickupPromptManager;
        MoreMountains.InventoryEngine.Inventory _targetInventory;


        void Start()
        {
            _pickupPromptManager = FindObjectOfType<PickupPromptManager>();

            // Locate PortableSystems and retrieve the appropriate inventory
            var portableSystems = GameObject.Find("PortableSystems");
            if (portableSystems != null)
                _targetInventory = portableSystems.GetComponentInChildren<MoreMountains.InventoryEngine.Inventory>();

            if (_targetInventory == null) Debug.LogWarning("Target inventory not found in PortableSystems.");

            // Initialize feedbacks
            if (PickedMMFeedbacks != null) PickedMMFeedbacks.Initialization(gameObject);
        }

        void Update()
        {
            if (_isInRange && UnityEngine.Input.GetKeyDown(KeyCode.F)) PickItem();
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                _isInRange = true;
                _collidingObject = collider.gameObject;
                _pickupPromptManager?.ShowPickupPrompt();
                _pickupPromptManager?.ShowPreviewPanel(Item); // Show preview when entering
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                _isInRange = false;
                _collidingObject = null;
                _pickupPromptManager?.HidePickupPrompt();
                _pickupPromptManager?.HidePreviewPanel(); // Hide preview when exiting
            }
        }

        void PickItem()
        {
            if (Item == null || _targetInventory == null)
            {
                Debug.LogWarning("Item or target inventory is null. Cannot pick up the item.");
                return;
            }

            if (_targetInventory.AddItem(Item, Quantity))
            {
                _pickupPromptManager?.HidePickupPrompt();
                _pickupPromptManager?.HidePreviewPanel(); // Hide preview on successful pickup

                // Play feedbacks on successful pickup
                if (PickedMMFeedbacks != null) PickedMMFeedbacks.PlayFeedbacks();

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
