using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Combat.Shields
{
    public class CharacterHandleShield : CharacterAbility 
    {
        [MMInspectorGroup("Shield", true, 10)]
        public Shield InitialShield;
        public Transform ShieldAttachment;
        public bool AutomaticallyBindAnimator = true;
        public int HandleShieldID = 1;

        [MMInspectorGroup("Input", true, 11)]
        /// if this is true, this ability can perform as usual, if not, it'll be ignored
        [Tooltip("if this is true, this ability can perform as usual, if not, it'll be ignored")]
        public bool InputAuthorized = true;
        /// if true, the shield will stay up as long as the button is held
        [Tooltip("if true, the shield will stay up as long as the button is held")]
        public bool ContinuousPress = true;

        public Shield CurrentShield { get; protected set; }
        public Animator CharacterAnimator { get; set; }

        protected bool _shieldActive = false;

        public override string HelpBoxText()
        {
            return "This ability allows the character to use shields for blocking damage. Uses the Interact button.";
        }

        protected override void Initialization()
        {
            base.Initialization();
            AssignRequiredComponents();
            SetupShield();
        }

        protected virtual void AssignRequiredComponents()
        {
            // Ensure we have an animator
            if (_animator == null && AutomaticallyBindAnimator)
            {
                _animator = GetComponent<Animator>();
                if (_animator == null) _animator = GetComponentInChildren<Animator>();
            }

            if (_animator == null) Debug.LogError("No animator found for CharacterHandleShield!", this);

            // Set up shield attachment point
            if (ShieldAttachment == null)
            {
                ShieldAttachment = transform;
                Debug.Log("Using transform as shield attachment point");
            }
        }

        protected virtual void SetupShield()
        {
            if (InitialShield != null)
            {
                EquipShield(InitialShield);
            }
        }

        public virtual void EquipShield(Shield newShield)
        {
            // Cleanup existing shield
            if (CurrentShield != null)
            {
                Destroy(CurrentShield.gameObject);
                CurrentShield = null;
            }

            if (newShield != null)
            {
                // Instantiate new shield
                GameObject shieldGO = Instantiate(newShield.gameObject, ShieldAttachment.position, ShieldAttachment.rotation);
                CurrentShield = shieldGO.GetComponent<Shield>();
                shieldGO.transform.SetParent(ShieldAttachment);
                shieldGO.transform.localPosition = Vector3.zero;
                shieldGO.transform.localRotation = Quaternion.identity;

                // Initialize shield
                if (CurrentShield != null)
                {
                    CurrentShield.SetOwner(_character, this);
                    CurrentShield.Initialization();
                }
            }
        }

        protected override void HandleInput()
        {
            if (!AbilityAuthorized || !InputAuthorized || CurrentShield == null)
            {
                return;
            }

            // Use InteractButton instead of a dedicated shield button
            if (_inputManager.InteractButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                ShieldStart();
            }
            else if (_inputManager.InteractButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed && ContinuousPress)
            {
                // Keep shield up while button is held
                if (!_shieldActive)
                {
                    ShieldStart();
                }
            }
            else if (_inputManager.InteractButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
            {
                ShieldStop();
            }
        }

        public virtual void ShieldStart()
        {
            if (!AbilityAuthorized || !InputAuthorized || CurrentShield == null) return;
            
            _shieldActive = true;
            PlayAbilityStartFeedbacks();
            CurrentShield?.RaiseShield();
        }

        public virtual void ShieldStop()
        {
            if (!AbilityAuthorized || !InputAuthorized || CurrentShield == null) return;

            _shieldActive = false;
            PlayAbilityStopFeedbacks();
            CurrentShield?.LowerShield();
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