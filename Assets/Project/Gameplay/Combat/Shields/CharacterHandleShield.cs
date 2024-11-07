using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Combat.Shields
{
    /// <summary>
    ///     Character ability that handles shield usage
    /// </summary>
    public class CharacterHandleShield : CharacterAbility
    {
        protected const float _lowShieldAnimationDuration = 0.2f;
        protected const string _shieldStateAnimationParameter = "ShieldState";
        [Header("Shield Settings")] [Tooltip("Shield to equip by default, if any")]
        public Shield DefaultShield;

        [Tooltip("Where to attach the shield")]
        public Transform ShieldAttachment;

        [Header("Animation")] public string ShieldRaisedAnimationParameter = "ShieldUp";
        public string ShieldBlockAnimationParameter = "ShieldBlock";
        public string ShieldBreakAnimationParameter = "ShieldBreak";
        protected int _shieldBlockAnimationParameterID;
        protected int _shieldBreakAnimationParameterID;

        protected bool _shieldInput;
        protected int _shieldRaisedAnimationParameterID;
        protected int _shieldStateParameterID;

        public Shield CurrentShield { get; protected set; }

        protected override void PreInitialization()
        {
            base.PreInitialization();

            if (ShieldAttachment == null) ShieldAttachment = transform;
        }

        protected override void Initialization()
        {
            base.Initialization();
            RegisterAnimationParameters();

            // Don't auto-equip shield here since character might be inventory-managed
        }

        public virtual void EquipShield(Shield shieldPrefab)
        {
            if (CurrentShield != null)
            {
                Destroy(CurrentShield.gameObject);
                CurrentShield = null;
            }

            if (shieldPrefab != null)
            {
                CurrentShield = Instantiate(shieldPrefab, ShieldAttachment.position, ShieldAttachment.rotation);
                CurrentShield.transform.parent = ShieldAttachment;
                CurrentShield.SetOwner(_character);
                CurrentShield.Initialization();
            }
        }

        protected override void HandleInput()
        {
            if (!AbilityAuthorized || CurrentShield == null) return;

            _shieldInput = UnityEngine.Input.GetMouseButton(1);
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (CurrentShield == null) return;

            if (_shieldInput)
            {
                CurrentShield.RaiseShield();
                PlayAbilityStartFeedbacks();
            }
            else
            {
                CurrentShield.LowerShield();
                StopStartFeedbacks();
            }
        }

        protected virtual void RegisterAnimationParameters()
        {
            RegisterAnimatorParameter(
                ShieldRaisedAnimationParameter, AnimatorControllerParameterType.Bool,
                out _shieldRaisedAnimationParameterID);

            RegisterAnimatorParameter(
                ShieldBlockAnimationParameter, AnimatorControllerParameterType.Trigger,
                out _shieldBlockAnimationParameterID);

            RegisterAnimatorParameter(
                ShieldBreakAnimationParameter, AnimatorControllerParameterType.Trigger,
                out _shieldBreakAnimationParameterID);

            RegisterAnimatorParameter(
                _shieldStateAnimationParameter, AnimatorControllerParameterType.Int, out _shieldStateParameterID);
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            if (CurrentShield != null) CurrentShield.LowerShield();
        }

        protected override void OnRespawn()
        {
            base.OnRespawn();
            // Shield will be managed by inventory system
        }
    }
}
