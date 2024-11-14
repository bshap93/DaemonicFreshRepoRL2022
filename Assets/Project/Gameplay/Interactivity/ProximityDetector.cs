using Project.Gameplay.Player.Inventory;
using UnityEngine;

namespace Project.Gameplay.Interactivity
{
    public class ProximityDetector : MonoBehaviour
    {
        // Reference to ManualItemPicker, set via the Inspector
        public ManualItemPicker ManualPicker;

        void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player"))
                // Notify the ManualItemPicker that the player is within range
                ManualPicker.OnPlayerEnter();
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Player"))
                // Notify the ManualItemPicker that the player has exited the range
                ManualPicker.OnPlayerExit();
        }
    }
}
