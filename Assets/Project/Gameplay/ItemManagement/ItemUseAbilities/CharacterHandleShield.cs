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

        public Animator CharacterAnimator
        {
            get
            {
                // Make sure we always return a valid animator
                if (_animator == null && AutomaticallyBindAnimator)
                {
                    _animator = GetComponent<Animator>();
                    if (_animator == null) _animator = GetComponentInChildren<Animator>();
                    if (_animator == null) Debug.LogError("Still cannot find animator!", this);
                }

                return _animator;
            }
        }
        public override string HelpBoxText()
        {
            return "This ability allows the character to use shields for blocking damage.";
        }

        protected override void Initialization()
        {
            base.Initialization();


            // Ensure we have an animator
            if (_animator == null && AutomaticallyBindAnimator)
            {
                _animator = GetComponent<Animator>();
                if (_animator == null) _animator = GetComponentInChildren<Animator>();
            }

            if (_animator == null) Debug.LogError("No animator found for CharacterHandleShield!", this);

            // Set up initial shield if we have one
            SetupShield();
        }

        protected virtual void SetupShield()
        {
            if (ShieldAttachment == null)
            {
                ShieldAttachment = transform;
                Debug.Log("Using transform as shield attachment point");
            }

            if (InitialShield != null) EquipShield(InitialShield);
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
            }
        }
        protected override void HandleInput()
        {
            base.HandleInput();

            if (!AbilityAuthorized || !InputAuthorized || CurrentShield == null)
            {
                Debug.Log(
                    $"Shield input blocked. Auth:{AbilityAuthorized} Input:{InputAuthorized} Shield:{CurrentShield != null}");

                return;
            }

            // Use the Input Manager directly to debug
            if (_inputManager.ShieldButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
                ShieldStart();
            else if (_inputManager.ShieldButton.State.CurrentState == MMInput.ButtonStates.ButtonUp) ShieldStop();
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

        // Make sure these are called
        public virtual void ShieldButtonDown()
        {
            _shieldButton?.State.ChangeState(MMInput.ButtonStates.ButtonDown);
        }

        public virtual void ShieldButtonPressed()
        {
            _shieldButton?.State.ChangeState(MMInput.ButtonStates.ButtonPressed);
        }

        public virtual void ShieldButtonUp()
        {
            _shieldButton?.State.ChangeState(MMInput.ButtonStates.ButtonUp);
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
