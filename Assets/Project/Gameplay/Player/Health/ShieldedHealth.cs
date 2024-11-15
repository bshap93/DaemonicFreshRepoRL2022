using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Combat.Shields;
using UnityEngine;

namespace Project.Gameplay.Player.Health
{
    public class ShieldedHealth : MoreMountains.TopDownEngine.Health
    {
        Shield _shield;
        ShieldProtectionArea _shieldProtection;

        protected override void Awake()
        {
            base.Awake();
            ShieldProtectionArea.OnShieldEquipped += AssignShield;
        }

        void OnDestroy()
        {
            ShieldProtectionArea.OnShieldEquipped -= AssignShield;
        }

        public override void Damage(float damage, GameObject instigator, float flickerDuration,
            float invincibilityDuration, Vector3 damageDirection, List<TypedDamage> typedDamages = null)
        {
            // Check if the shield blocks the damage
            if (_shieldProtection != null && _shieldProtection.IsBlocking(instigator.transform.position))
            {
                _shield.PlayBlockFeedback();
                return; // Exit early if shield blocks damage
            }

            // Otherwise, apply damage as usual
            base.Damage(damage, instigator, flickerDuration, invincibilityDuration, damageDirection, typedDamages);
        }

        void AssignShield(ShieldProtectionArea shieldPa)
        {
            _shieldProtection = shieldPa;
            _shield = shieldPa.gameObject.GetComponent<Shield>();
        }
    }
}
