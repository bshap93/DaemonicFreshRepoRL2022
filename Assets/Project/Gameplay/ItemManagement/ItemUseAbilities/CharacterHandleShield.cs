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
        public override string HelpBoxText()
        {
            return "This ability allows the character to use shields for blocking damage.";
        }

        protected override void Initialization()
        {
            base.Initialization();

            Debug.Log("CharacterHandleShield initializing...");

            // Ensure we have an animator
            if (_animator == null && AutomaticallyBindAnimator)
            {
                _animator = GetComponent<Animator>();
                if (_animator == null)
                {
                    _animator = GetComponentInChildren<Animator>();
                }
            }

            if (_animator == null)
            {
                Debug.LogError("No animator found for CharacterHandleShield!", this);
            }

            // Set up initial shield if we have one
            SetupShield();

            Debug.Log($"CharacterHandleShield initialized. Animator: {_animator != null}, Initial shield: {InitialShield != null}");
        }
        
        public Animator CharacterAnimator
        {
            get 
            { 
                // Make sure we always return a valid animator
                if (_animator == null && AutomaticallyBindAnimator)
                {
                    _animator = GetComponent<Animator>();
                    if (_animator == null)
                    {
                        _animator = GetComponentInChildren<Animator>();
                    }
                    if (_animator == null)
                    {
                        Debug.LogError("Still cannot find animator!", this);
                    }
                }
                return _animator; 
            }
        }

        protected virtual void SetupShield()
        {
            if (ShieldAttachment == null)
            {
                ShieldAttachment = transform;
                Debug.Log("Using transform as shield attachment point");
            }

            if (InitialShield != null)
            {
                Debug.Log("Setting up initial shield");
                EquipShield(InitialShield);
            }
        }
        public virtual void EquipShield(Shield newShield)
        {
            Debug.Log($"Equipping shield: {(newShield != null ? newShield.ShieldName : "null")}");

            if (CurrentShield != null)
            {
                Debug.Log("Destroying current shield");
                Destroy(CurrentShield.gameObject);
            }

            if (newShield != null)
            {
                Debug.Log("Instantiating new shield");
                CurrentShield = Instantiate(newShield, ShieldAttachment.position, ShieldAttachment.rotation);
                CurrentShield.transform.parent = ShieldAttachment;
                CurrentShield.SetOwner(_character, this);
                CurrentShield.Initialization();
                Debug.Log($"Shield equipped successfully: {CurrentShield.name}");
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
            {
                Debug.Log("Shield button pressed");
                ShieldStart();
            }
            else if (_inputManager.ShieldButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
            {
                Debug.Log("Shield button released");
                ShieldStop();
            }
        }

        public virtual void ShieldStart()
        {
            Debug.Log("ShieldStart was called");
            if (CurrentShield == null) return;
            Debug.Log("ShieldStart was called and shield exists");

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
            Debug.Log("Shield button down called");
            _shieldButton?.State.ChangeState(MMInput.ButtonStates.ButtonDown);
        }

        public virtual void ShieldButtonPressed()
        {
            Debug.Log("Shield button pressed called");
            _shieldButton?.State.ChangeState(MMInput.ButtonStates.ButtonPressed);
        }

        public virtual void ShieldButtonUp()
        {
            Debug.Log("Shield button up called");
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
