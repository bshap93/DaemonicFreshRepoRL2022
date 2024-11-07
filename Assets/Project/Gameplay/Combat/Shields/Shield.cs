using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Combat
{
    /// <summary>
    ///     Core shield class that handles shield mechanics
    /// </summary>
    public class Shield : MonoBehaviour
    {
        [Header("Shield Properties")] [Tooltip("Maximum amount of damage the shield can block")]
        public float MaxShieldHealth = 100f;

        [Tooltip("Current shield durability")] [MMReadOnly]
        public float CurrentShieldHealth;

        [Tooltip("Percentage of damage blocked when shield is up (0-1)")]
        public float DamageReduction = 0.5f;

        [Header("Movement")] [Tooltip("Movement speed multiplier while shield is raised")]
        public float RaisedMovementMultiplier = 0.5f;

        [Header("Feedback")] public MMFeedbacks ShieldRaiseFeedback;
        public MMFeedbacks ShieldBlockFeedback;
        public MMFeedbacks ShieldBreakFeedback;
        protected CharacterMovement _characterMovement;
        protected bool _isShieldRaised;
        protected float _originalMovementSpeed;

        protected Character _owner;

        public virtual void Initialization()
        {
            CurrentShieldHealth = MaxShieldHealth;
            ShieldRaiseFeedback?.Initialization(gameObject);
            ShieldBlockFeedback?.Initialization(gameObject);
            ShieldBreakFeedback?.Initialization(gameObject);
        }

        public virtual void SetOwner(Character character)
        {
            _owner = character;
            _characterMovement = _owner.FindAbility<CharacterMovement>();
            if (_characterMovement != null) _originalMovementSpeed = _characterMovement.MovementSpeed;
        }

        public virtual void RaiseShield()
        {
            if (_isShieldRaised || CurrentShieldHealth <= 0) return;

            _isShieldRaised = true;
            ShieldRaiseFeedback?.PlayFeedbacks();

            if (_characterMovement != null)
                _characterMovement.MovementSpeed = _originalMovementSpeed * RaisedMovementMultiplier;
        }

        public virtual void LowerShield()
        {
            if (!_isShieldRaised) return;

            _isShieldRaised = false;

            if (_characterMovement != null) _characterMovement.MovementSpeed = _originalMovementSpeed;
        }

        public virtual bool ProcessDamage(float incomingDamage)
        {
            if (!_isShieldRaised || CurrentShieldHealth <= 0) return false;

            var reducedDamage = incomingDamage * (1f - DamageReduction);
            CurrentShieldHealth -= reducedDamage;

            ShieldBlockFeedback?.PlayFeedbacks();

            if (CurrentShieldHealth <= 0) ShieldBreak();

            return true;
        }

        protected virtual void ShieldBreak()
        {
            _isShieldRaised = false;
            ShieldBreakFeedback?.PlayFeedbacks();

            if (_characterMovement != null) _characterMovement.MovementSpeed = _originalMovementSpeed;
        }
    }
}
