using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngine.Tests
{
    public class ShieldInputTester : MonoBehaviour
    {
        protected InputManager _inputManager;

        void Start()
        {
            _inputManager = FindObjectOfType<InputManager>();
        }

        void Update()
        {
            // Test direct Unity input
            if (Input.GetButton("Player1_Shield")) Debug.Log("Raw Unity Input: Shield button is pressed");

            // Test TopDown Engine input system
            if (_inputManager != null && _inputManager.ShieldButton != null)
            {
                if (_inputManager.ShieldButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
                    Debug.Log("TopDown Engine: Shield button pressed");
                else if (_inputManager.ShieldButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
                    Debug.Log("TopDown Engine: Shield button released");
            }
        }

        void OnGUI()
        {
            GUI.Label(
                new Rect(10, 10, 300, 20),
                $"Shield Button State: {_inputManager?.ShieldButton?.State.CurrentState}");

            GUI.Label(
                new Rect(10, 30, 300, 20),
                $"Raw Input State: {(Input.GetButton("Player1_Shield") ? "Pressed" : "Not Pressed")}");
        }
    }
}
