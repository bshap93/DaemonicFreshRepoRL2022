using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.ItemManagement.ItemClasses;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.ItemUseAbilities
{
    public class CharacterHandleShield : CharacterAbility
    {
        [MMInspectorGroup("Shield", true, 10)] public Shield InitialShield;
        public Transform ShieldAttachment;
        public bool AutomaticallyBindAnimator = true;
        public int HandleShieldID = 1;

        [MMInspectorGroup("Input", true, 11)] public bool InputAuthorized = true;
        public bool ContinuousPress = true;

        protected MMInput.IMButton _shieldButton;

        public Shield CurrentShield { get; protected set; }
        public Animator CharacterAnimator { get; protected set; }

        protected override void Initialization()
        {
            base.Initialization();

            // Initialize input button
            if (_inputManager != null)
                _shieldButton = new MMInput.IMButton(
                    _character.PlayerID, "Shield", ShieldButtonDown, ShieldButtonPressed, ShieldButtonUp);

            // Setup shield and animator
            CharacterAnimator = _animator;
            SetupShield();
        }

        protected virtual void SetupShield()
        {
            if (ShieldAttachment == null) ShieldAttachment = transform;

            if (InitialShield != null) EquipShield(InitialShield);
        }

        protected override void HandleInput()
        {
            // Early exit if conditions aren't met
            if (!AbilityAuthorized
                || !InputAuthorized
                || CurrentShield == null
                || _inputManager == null
                || _shieldButton == null)
                return;

            if (_shieldButton.State.CurrentState == MMInput.ButtonStates.ButtonDown) ShieldStart();

            if (_shieldButton.State.CurrentState == MMInput.ButtonStates.ButtonUp) ShieldStop();

            if (ContinuousPress && _shieldButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed)
                CurrentShield.RaiseShield();
        }

        public virtual void ShieldStart()
        {
            if (CurrentShield == null) return;

            PlayAbilityStartFeedbacks();
            CurrentShield.RaiseShield();
        }

        public virtual void ShieldStop()
        {
            if (CurrentShield == null) return;

            PlayAbilityStopFeedbacks();
            CurrentShield.LowerShield();
        }

        protected virtual void ShieldButtonDown()
        {
            if (_shieldButton != null) _shieldButton.State.ChangeState(MMInput.ButtonStates.ButtonDown);
        }
        protected virtual void ShieldButtonPressed()
        {
            if (_shieldButton != null) _shieldButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed);
        }
        protected virtual void ShieldButtonUp()
        {
            if (_shieldButton != null) _shieldButton.State.ChangeState(MMInput.ButtonStates.ButtonUp);
        }

        public virtual void EquipShield(Shield newShield)
        {
            if (CurrentShield != null) Destroy(CurrentShield.gameObject);

            if (newShield != null)
            {
                CurrentShield = Instantiate(newShield, ShieldAttachment.position, ShieldAttachment.rotation);
                CurrentShield.transform.parent = ShieldAttachment;
                CurrentShield.SetOwner(_character, this);
                CurrentShield.Initialization();
                Debug.Log($"Shield equipped successfully: {CurrentShield.name}");
            }
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            ShieldStop();
        }

        protected override void OnRespawn()
        {
            base.OnRespawn();
            SetupShield();
        }
    }
}
