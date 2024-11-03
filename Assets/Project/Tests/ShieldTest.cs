using System;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.ItemManagement.ItemClasses;
using Project.Gameplay.ItemManagement.ItemUseAbilities;
using TopDownEngine.Common.Scripts.Characters.Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace Project.Tests
{
    public class ShieldTest : MonoBehaviour
    {
        protected CharacterHandleShield _handleShield;
        protected InputManager _inputManager;
        protected Shield _shield;
        protected bool _testsPassed;
        protected bool _testsStarted;

        void Start()
        {
            // Start coroutine to wait for player initialization
            StartCoroutine(WaitForPlayerInitialization());
        }

        void OnGUI()
        {
            if (!_testsStarted)
                GUI.Label(new Rect(10, 10, 300, 20), "Shield Tests: Waiting for player initialization...");
            else if (_testsPassed)
                GUI.Label(new Rect(10, 10, 300, 20), "Shield Implementation Tests: PASSED");
            else
                GUI.Label(new Rect(10, 10, 300, 20), "Shield Implementation Tests: FAILED (Check Console)");
        }

        IEnumerator WaitForPlayerInitialization()
        {
            Debug.Log("Waiting for player initialization...");

            // Wait a frame to let initial setup happen
            yield return null;

            // Wait until we find the InputManager
            var timeout = Time.time + 5f; // 5 second timeout
            while (InputManager.Instance == null && Time.time < timeout)
            {
                Debug.Log("Waiting for InputManager...");
                yield return new WaitForSeconds(0.1f);
            }

            // Wait an additional frame for components to initialize
            yield return null;

            if (InputManager.Instance == null)
            {
                Debug.LogError("InputManager not found after timeout! Make sure the Player prefab is being spawned.");
                _testsPassed = false;
            }
            else
            {
                Debug.Log("Player initialized. Starting tests...");
                _inputManager = InputManager.Instance;
                RunTests();
            }
        }

        void RunTests()
        {
            if (_testsStarted) return;
            _testsStarted = true;

            Debug.Log("Starting Shield Implementation Tests...");

            try
            {
                // Test 1: Component Setup
                TestComponentSetup();

                // Test 2: Input Handling
                TestInputHandling();

                // Test 3: Shield Mechanics
                TestShieldMechanics();

                _testsPassed = true;
                Debug.Log("All shield tests passed!");
            }
            catch (Exception e)
            {
                Debug.LogError($"Tests failed: {e.Message}");
                _testsPassed = false;
            }
        }

        void TestComponentSetup()
        {
            Debug.Log("Testing Component Setup...");

            // Test Input Manager
            Assert.IsNotNull(InputManager.Instance, "InputManager singleton should be available");
            Assert.IsNotNull(_inputManager.ShieldButton, "Shield button should be initialized in InputManager");

            // Find the player character
            var player = FindObjectOfType<Character>();
            Assert.IsNotNull(player, "Player Character should be present in scene");

            // Test CharacterHandleShield
            _handleShield = player.GetComponent<CharacterHandleShield>();
            Assert.IsNotNull(_handleShield, "CharacterHandleShield should be present on character");

            // Test Shield
            _shield = _handleShield.CurrentShield;
            Assert.IsNotNull(_shield, "Shield component should be present and initialized");
            Assert.IsTrue(_shield.CurrentShieldHealth > 0, "Shield should have health");

            Debug.Log("Component Setup Tests Passed!");
        }

        void TestInputHandling()
        {
            Debug.Log("Testing Input Handling...");

            // Simulate shield button press
            _inputManager.ShieldButtonDown();
            Assert.AreEqual(
                MMInput.ButtonStates.ButtonDown, _inputManager.ShieldButton.State.CurrentState,
                "Shield button state should be ButtonDown");

            // Check if shield raises on button press
            Assert.AreEqual(
                Shield.ShieldStates.ShieldActive, _shield.ShieldState.CurrentState,
                "Shield should be active when button is pressed");

            // Let a frame pass
            StartCoroutine(TestInputRelease());
        }

        IEnumerator TestInputRelease()
        {
            yield return null;

            // Simulate shield button release
            _inputManager.ShieldButtonUp();
            Assert.AreEqual(
                MMInput.ButtonStates.ButtonUp, _inputManager.ShieldButton.State.CurrentState,
                "Shield button state should be ButtonUp");

            Debug.Log("Input Handling Tests Passed!");
        }

        void TestShieldMechanics()
        {
            Debug.Log("Testing Shield Mechanics...");

            // Test damage blocking
            var initialHealth = _shield.CurrentShieldHealth;
            var blocked = _shield.ProcessDamage(10f);

            Assert.IsTrue(blocked, "Shield should block damage when active");
            Assert.IsTrue(
                _shield.CurrentShieldHealth < initialHealth, "Shield health should decrease when blocking damage");

            // Test shield break
            _shield.ProcessDamage(_shield.MaxShieldHealth);
            Assert.AreEqual(
                Shield.ShieldStates.ShieldBreak, _shield.ShieldState.CurrentState,
                "Shield should break when health depleted");

            Debug.Log("Shield Mechanics Tests Passed!");
        }
    }
}
