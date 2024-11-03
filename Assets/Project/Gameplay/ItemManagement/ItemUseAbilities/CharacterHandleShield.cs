using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.ItemManagement.ItemClasses;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.ItemUseAbilities
{
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Handle Shield")]
    public class CharacterHandleShield : CharacterAbility
    {
        [MMInspectorGroup("Input", true, 11)]
        /// if this is true, this shield will be able to read input, otherwise input will be disabled
        [Tooltip("if this is true, this shield will be able to read input, otherwise input will be disabled")]
        public bool InputAuthorized = true;
        /// if true, the shield button will need to be released to block again
        [Tooltip("if true, the shield button will need to be released to block again")]
        public bool ContinuousPress = true;

        [MMInspectorGroup("Shield", true, 10)] public Shield InitialShield;
        public Transform ShieldAttachment;
        public bool AutomaticallyBindAnimator = true;

        /// the ID of this CharacterHandleShield. This will be used to determine what handle shield ability should equip a shield.
        [Tooltip("ID of this shield handler - used to determine which handler equips which shield")]
        public int HandleShieldID = 1;

        [MMInspectorGroup("Input", true, 11)] [Tooltip("The name of the input to use for shield activation")]
        public string ShieldButtonName = "Shield";

        protected MMInput.IMButton _shieldButton;


        protected bool _shieldButtonPressed = false;

        public Shield CurrentShield { get; protected set; }
        public Animator CharacterAnimator { get; protected set; }
        public override string HelpBoxText()
        {
            return
                "This component handles shield mechanics. You can set an initial shield and configure shield-specific parameters here.";
        }

        protected override void Initialization()
        {
            base.Initialization();
            SetupShield();
        }

        protected virtual void SetupShield()
        {
            CharacterAnimator = _animator;

            if (ShieldAttachment == null) ShieldAttachment = transform;

            if (InitialShield != null) EquipShield(InitialShield);
        }

        protected override void HandleInput()
        {
            if (!AbilityAuthorized || !InputAuthorized || CurrentShield == null) return;

            if (_inputManager.ShieldInput.State.CurrentState == MMInput.ButtonStates.ButtonDown) ShieldStart();

            if (_inputManager.ShieldInput.State.CurrentState == MMInput.ButtonStates.ButtonUp) ShieldStop();

            if (ContinuousPress && _inputManager.ShieldInput.State.CurrentState == MMInput.ButtonStates.ButtonPressed)
                CurrentShield.RaiseShield();
        }

        public virtual void ShieldStart()
        {
            if (CurrentShield != null)
            {
                PlayAbilityStartFeedbacks();
                CurrentShield.RaiseShield();
            }
        }

        public virtual void ShieldStop()
        {
            if (CurrentShield != null)
            {
                PlayAbilityStopFeedbacks();
                CurrentShield.LowerShield();
            }
        }

        public virtual void EquipShield(Shield newShield)
        {
            if (CurrentShield != null) Destroy(CurrentShield.gameObject);

            CurrentShield = Instantiate(newShield, ShieldAttachment.position, ShieldAttachment.rotation);
            CurrentShield.transform.parent = ShieldAttachment;
            CurrentShield.SetOwner(_character, this);
            CurrentShield.Initialization();
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
