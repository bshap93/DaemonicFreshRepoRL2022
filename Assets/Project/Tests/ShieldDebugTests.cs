using UnityEngine;
using MoreMountains.TopDownEngine;
using Project.Gameplay.ItemManagement.ItemUseAbilities;
using TopDownEngine.Common.Scripts.Characters.Core;

namespace Project.Tests
{
    public class ShieldDebugTest : MonoBehaviour
    {
        protected CharacterHandleShield _handleShield;
        protected InputManager _inputManager;

        void Start()
        {
            StartCoroutine(WaitAndTest());
        }

        System.Collections.IEnumerator WaitAndTest()
        {
            // Wait for player spawn
            yield return new WaitForSeconds(0.5f);

            var player = FindObjectOfType<Character>();
            if (player != null)
            {
                _handleShield = player.GetComponent<CharacterHandleShield>();
                Debug.Log($"Found CharacterHandleShield: {_handleShield != null}");
                if (_handleShield != null)
                {
                    Debug.Log($"Shield Equipment State:");
                    Debug.Log($"- Initial Shield assigned: {_handleShield.InitialShield != null}");
                    Debug.Log($"- Current Shield exists: {_handleShield.CurrentShield != null}");
                    Debug.Log($"- Shield Attachment exists: {_handleShield.ShieldAttachment != null}");
                }
            }
            else
            {
                Debug.LogError("No player character found!");
            }

            _inputManager = InputManager.Instance;
            Debug.Log($"Input Manager exists: {_inputManager != null}");
        }

        void OnGUI()
        {
            if (_handleShield != null && _handleShield.CurrentShield != null)
            {
                GUI.Label(new Rect(10, 10, 300, 20), 
                    $"Shield State: {_handleShield.CurrentShield.ShieldState?.CurrentState}");
            }
        }
    }
}
