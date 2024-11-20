using System;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Items
{
    public class Torch : MonoBehaviour
    {
        public enum TorchStates
        {
            Inactive,
            Active,
            BurnedOut
        }

        [Header("Torch Properties")] [Tooltip("The maximum burn time of the torch in seconds.")]
        public float MaxBurnTime = 60f;

        [Tooltip("Feedback when the torch is lit.")]
        public MMFeedbacks TorchLitFeedback;

        [Tooltip("Feedback when the torch burns out.")]
        public MMFeedbacks TorchBurnedOutFeedback;

        [Tooltip("Feedback when the torch is extinguished manually.")]
        public MMFeedbacks TorchExtinguishedFeedback;
        float _burnTimer;

        Character _owner;

        public TorchStates CurrentState { get; private set; } = TorchStates.Inactive;

        void Update()
        {
            if (CurrentState == TorchStates.Active) HandleBurning();
        }

        public event Action OnTorchBurnedOut;

        public void EquipTorch(Character owner)
        {
            _owner = owner;
            InitializeTorch();
        }

        void InitializeTorch()
        {
            _burnTimer = MaxBurnTime;
            CurrentState = TorchStates.Inactive;
        }

        public void LightTorch()
        {
            if (CurrentState != TorchStates.Inactive) return;

            CurrentState = TorchStates.Active;
            _burnTimer = MaxBurnTime;

            TorchLitFeedback?.PlayFeedbacks();
            Debug.Log("Torch lit!");
        }

        public void ExtinguishTorch()
        {
            if (CurrentState != TorchStates.Active) return;

            CurrentState = TorchStates.Inactive;
            TorchExtinguishedFeedback?.PlayFeedbacks();
            Debug.Log("Torch extinguished manually.");
        }

        void HandleBurning()
        {
            _burnTimer -= Time.deltaTime;

            if (_burnTimer <= 0) BurnOutTorch();
        }

        void BurnOutTorch()
        {
            CurrentState = TorchStates.BurnedOut;

            TorchBurnedOutFeedback?.PlayFeedbacks();
            Debug.Log("Torch burned out!");

            OnTorchBurnedOut?.Invoke();
        }
    }
}
