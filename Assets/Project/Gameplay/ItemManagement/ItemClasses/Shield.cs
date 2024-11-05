using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.ItemManagement.ItemUseAbilities;
using TopDownEngine.Common.Scripts.Characters.Core;
using UnityEditor;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.ItemClasses
{
    /// <summary>
    ///     Shield base class that handles shield mechanics and states
    /// </summary>
    [SelectionBase]
    public class Shield : MMMonoBehaviour
    {
        public enum ShieldStates
        {
            ShieldIdle,
            ShieldStart,
            ShieldActive,
            ShieldBlock,
            ShieldBreak,
            ShieldRecover,
            ShieldInterrupted
        }

        // Animation timing constants
        protected const float RAISE_ANIMATION_TIME = 0.3f;
        protected const float LOWER_ANIMATION_TIME = 0.2f;

        [MMInspectorGroup("ID", true, 7)] [Tooltip("the name of the shield")]
        public string ShieldName;

        [MMInspectorGroup("Shield Properties", true, 8)]
        [Tooltip("The amount of damage this shield can block before breaking")]
        public float MaxShieldHealth = 100f;
        [Tooltip("Current shield durability")] public float CurrentShieldHealth;
        [Tooltip("Time required for the shield to recover after breaking")]
        public float RecoveryTime = 5f;
        [Tooltip("Damage reduction percentage when blocking (0-1)")]
        public float BlockingDamageReduction = 0.5f;

        [MMInspectorGroup("Movement", true, 9)] [Tooltip("Movement speed multiplier while shield is raised")]
        public float MovementMultiplier = 0.5f;
        [Tooltip("if true, shield will slow movement while active")]
        public bool ModifyMovementWhileBlocking = true;

        [MMInspectorGroup("Feedback", true, 10)]
        public MMFeedbacks ShieldRaiseFeedback;
        public MMFeedbacks ShieldBlockFeedback;
        public MMFeedbacks ShieldBreakFeedback;

        [MMInspectorGroup("Animation", true, 11)]
        public string ShieldUpAnimationParameter = "ShieldUp";
        public string ShieldBlockAnimationParameter = "ShieldBlock";
        public string ShieldBreakAnimationParameter = "ShieldBreak";

        protected HashSet<int> _animatorParameters;
        protected CharacterMovement _characterMovement;
        protected bool _initialized;
        protected float _movementMultiplierStorage = 1f;

        protected Animator _ownerAnimator;
        protected int _shieldBlockAnimationParameter;
        protected int _shieldBreakAnimationParameter;
        // Add this field at the top with your other parameters
        protected int _shieldStateParameter;

        protected int _shieldUpAnimationParameter;
        public MMStateMachine<ShieldStates> ShieldState;

        // References
        public Character Owner { get; protected set; }
        public CharacterHandleShield CharacterHandle { get; set; }

        // Add a debug gizmo to visualize the current state
        protected virtual void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            var offset = Vector3.up * 2f;
            var stateColor = ShieldState?.CurrentState switch
            {
                ShieldStates.ShieldIdle => Color.gray,
                ShieldStates.ShieldStart => Color.yellow,
                ShieldStates.ShieldActive => Color.green,
                ShieldStates.ShieldBlock => Color.blue,
                ShieldStates.ShieldBreak => Color.red,
                ShieldStates.ShieldRecover => Color.cyan,
                _ => Color.white
            };

            Gizmos.color = stateColor;
            Gizmos.DrawWireSphere(transform.position + offset, 0.3f);

#if UNITY_EDITOR
            // Draw state name
            if (ShieldState != null)
                Handles.Label(
                    transform.position + offset + Vector3.up * 0.3f,
                    ShieldState.CurrentState.ToString());
#endif
        }

        public virtual void Initialization()
        {
            if (_initialized) return;

            // Debug checkpoint

            ShieldState = new MMStateMachine<ShieldStates>(gameObject, true);
            ShieldState.ChangeState(ShieldStates.ShieldIdle);
            CurrentShieldHealth = MaxShieldHealth;

            // Initialize animator parameters if we already have an owner
            if (_ownerAnimator != null) InitializeAnimatorParameters();

            InitializeFeedbacks();
            _initialized = true;

            // Debug checkpoint
        }

        protected virtual void InitializeFeedbacks()
        {
            ShieldRaiseFeedback?.Initialization(gameObject);
            ShieldBlockFeedback?.Initialization(gameObject);
            ShieldBreakFeedback?.Initialization(gameObject);
        }

        public virtual void SetOwner(Character newOwner, CharacterHandleShield handleShield)
        {
            // Debug checkpoint

            Owner = newOwner;
            CharacterHandle = handleShield;

            if (Owner != null)
            {
                _characterMovement = Owner.FindAbility<CharacterMovement>();
                _ownerAnimator = handleShield.CharacterAnimator;

                // Debug checkpoint

                // Re-initialize parameters with new animator
                InitializeAnimatorParameters();
            }
            else
            {
                Debug.LogError($"Shield {ShieldName} assigned null owner", this);
            }
        }


        protected virtual void InitializeAnimatorParameters()
        {
            if (_ownerAnimator == null)
            {
                Debug.LogWarning(
                    $"Cannot initialize animator parameters - no animator found for shield {ShieldName}", this);

                return;
            }

            // Debug checkpoint

            _animatorParameters = new HashSet<int>();

            RegisterAnimatorParameter(
                "ShieldState",
                AnimatorControllerParameterType.Int,
                out _shieldStateParameter);

            RegisterAnimatorParameter(
                ShieldUpAnimationParameter,
                AnimatorControllerParameterType.Bool,
                out _shieldUpAnimationParameter);

            RegisterAnimatorParameter(
                ShieldBlockAnimationParameter,
                AnimatorControllerParameterType.Trigger,
                out _shieldBlockAnimationParameter);

            RegisterAnimatorParameter(
                ShieldBreakAnimationParameter,
                AnimatorControllerParameterType.Trigger,
                out _shieldBreakAnimationParameter);

            // Debug checkpoint
        }

        protected virtual void RegisterAnimatorParameter(string parameterName,
            AnimatorControllerParameterType parameterType, out int parameter)
        {
            parameter = Animator.StringToHash(parameterName);

            if (_ownerAnimator == null) return;

            if (_ownerAnimator.MMHasParameterOfType(parameterName, parameterType)) _animatorParameters.Add(parameter);
        }

        public virtual void RaiseShield()
        {
            Debug.Log($"RaiseShield called. Current state: {ShieldState.CurrentState}");

            if (ShieldState.CurrentState == ShieldStates.ShieldBreak)
            {
                Debug.Log("Shield is broken, cannot raise");
                return;
            }

            // First change to start state
            ShieldState.ChangeState(ShieldStates.ShieldStart);
            SyncStateToAnimator(ShieldStates.ShieldStart);

            // Start the raise sequence
            StartCoroutine(RaiseShieldSequence());
        }

        protected virtual IEnumerator RaiseShieldSequence()
        {
            yield return new WaitForSeconds(RAISE_ANIMATION_TIME);

            if (ShieldState.CurrentState == ShieldStates.ShieldStart)
            {
                ShieldState.ChangeState(ShieldStates.ShieldActive);
                SyncStateToAnimator(ShieldStates.ShieldActive);

                ShieldRaiseFeedback?.PlayFeedbacks();

                if (_characterMovement != null && ModifyMovementWhileBlocking)
                {
                    _movementMultiplierStorage = _characterMovement.MovementSpeedMultiplier;
                    _characterMovement.MovementSpeedMultiplier = MovementMultiplier;
                }
            }
        }

        // Add validation method
        public bool ValidateAnimatorSetup()
        {
            if (_ownerAnimator == null)
            {
                Debug.LogError($"Shield {ShieldName} has no animator assigned", this);
                return false;
            }

            if (_animatorParameters == null || _animatorParameters.Count == 0)
            {
                Debug.LogError($"Shield {ShieldName} animator parameters not initialized", this);
                return false;
            }

            // Check for required parameters
            var requiredParameters = new[]
            {
                "ShieldState",
                ShieldUpAnimationParameter,
                ShieldBlockAnimationParameter,
                ShieldBreakAnimationParameter
            };

            foreach (var param in requiredParameters)
                if (!_animatorParameters.Contains(Animator.StringToHash(param)))
                {
                    Debug.LogError($"Shield {ShieldName} missing required animator parameter: {param}", this);
                    return false;
                }

            return true;
        }

        public virtual void LowerShield()
        {
            if (ShieldState.CurrentState == ShieldStates.ShieldBreak) return;

            ShieldState.ChangeState(ShieldStates.ShieldIdle);
            SyncStateToAnimator(ShieldStates.ShieldIdle);

            if (_characterMovement != null && ModifyMovementWhileBlocking)
                _characterMovement.MovementSpeedMultiplier = _movementMultiplierStorage;
        }

        public virtual bool ProcessDamage(float incomingDamage)
        {
            if (ShieldState.CurrentState != ShieldStates.ShieldActive) return false;

            var reducedDamage = incomingDamage * (1f - BlockingDamageReduction);
            CurrentShieldHealth -= reducedDamage;

            // Temporarily change to block state
            ShieldState.ChangeState(ShieldStates.ShieldBlock);
            SyncStateToAnimator(ShieldStates.ShieldBlock);

            ShieldBlockFeedback?.PlayFeedbacks();

            // Start coroutine to return to active state
            StartCoroutine(ReturnToActiveState());

            if (CurrentShieldHealth <= 0) BreakShield();

            return true;
        }

        protected virtual IEnumerator ReturnToActiveState()
        {
            yield return new WaitForSeconds(0.5f); // Adjust time as needed for block animation

            if (ShieldState.CurrentState == ShieldStates.ShieldBlock)
            {
                ShieldState.ChangeState(ShieldStates.ShieldActive);
                SyncStateToAnimator(ShieldStates.ShieldActive);
            }
        }


        protected virtual void BreakShield()
        {
            ShieldState.ChangeState(ShieldStates.ShieldBreak);
            SyncStateToAnimator(ShieldStates.ShieldBreak);

            ShieldBreakFeedback?.PlayFeedbacks();

            StartCoroutine(RecoverShieldCoroutine());
        }


        protected virtual IEnumerator RecoverShieldCoroutine()
        {
            yield return new WaitForSeconds(RecoveryTime);

            CurrentShieldHealth = MaxShieldHealth;
            ShieldState.ChangeState(ShieldStates.ShieldIdle);
            SyncStateToAnimator(ShieldStates.ShieldIdle);
        }
        public virtual void UpdateAnimator()
        {
            if (_ownerAnimator == null || _animatorParameters == null) return;

            // Update both the state integer and the bool for compatibility
            MMAnimatorExtensions.UpdateAnimatorInteger(
                _ownerAnimator,
                _shieldStateParameter,
                (int)ShieldState.CurrentState,
                _animatorParameters
            );

            // Keep the existing bool update for backward compatibility
            var shieldUp = ShieldState.CurrentState == ShieldStates.ShieldActive;
            MMAnimatorExtensions.UpdateAnimatorBool(
                _ownerAnimator,
                _shieldUpAnimationParameter,
                shieldUp,
                _animatorParameters
            );
        }

        // Add this helper method to sync state to animator
        protected virtual void SyncStateToAnimator(ShieldStates state)
        {
            if (_ownerAnimator == null || _animatorParameters == null) return;


            // Update the state parameter
            MMAnimatorExtensions.UpdateAnimatorInteger(
                _ownerAnimator,
                _shieldStateParameter,
                (int)state,
                _animatorParameters,
                false
            );

            // For compatibility, also update the bool parameter
            var shieldUp = state == ShieldStates.ShieldActive || state == ShieldStates.ShieldStart;
            MMAnimatorExtensions.UpdateAnimatorBool(
                _ownerAnimator,
                _shieldUpAnimationParameter,
                shieldUp,
                _animatorParameters
            );
        }
    }
}
