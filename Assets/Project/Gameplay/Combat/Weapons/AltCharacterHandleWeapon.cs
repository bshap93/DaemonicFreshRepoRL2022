using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Combat.Weapons
{
    public enum WeaponAttachmentType
    {
        RightHandMelee,
        TwoHandedMelee,
        TwoHandedBow,
        RightHandRanged,
        TwoHandedRanged,
        Other
    }

    [AddComponentMenu("Roguelike/Character/Abilities/Alternate Character Handle Weapon")]
    public class AltCharacterHandleWeapon : CharacterHandleWeapon
    {
        protected new const string _weaponEquippedAnimationParameterName = "WeaponEquipped";
        protected new const string _weaponEquippedIDAnimationParameterName = "WeaponEquippedID";

        public WeaponAttachmentType WeaponAttachmentType;

        [Header("Weapon Attachment Points")] [Tooltip("List of attachment points based on weapon types.")]
        public List<AttachmentPoint> AttachmentPointList;

        [Tooltip("The position the weapon will be attached to. If left blank, will be this.transform.")]
        public new Transform WeaponAttachment;

        /// <summary>
        ///     Sets the weapon attachment based on WeaponAttachmentType.
        /// </summary>
        protected override void PreInitialization()
        {
            base.PreInitialization();

            WeaponAttachment = transform; // Default if no specific attachment is found
            foreach (var point in AttachmentPointList)
                if (point.Type == WeaponAttachmentType)
                {
                    WeaponAttachment = point.Attachment;
                    break;
                }

            Debug.Log($"WeaponAttachment set to {WeaponAttachment.name} for {WeaponAttachmentType}");
        }

        /// <summary>
        ///     Instantiates the specified weapon.
        /// </summary>
        protected override void InstantiateWeapon(Weapon newWeapon, string weaponID, bool combo = false)
        {
            if (WeaponAttachment == null) PreInitialization();

            if (!combo)
                CurrentWeapon = Instantiate(
                    newWeapon,
                    WeaponAttachment.position + newWeapon.WeaponAttachmentOffset,
                    WeaponAttachment.rotation);

            CurrentWeapon.name = newWeapon.name;
            CurrentWeapon.transform.parent = WeaponAttachment;
            CurrentWeapon.transform.localPosition = newWeapon.WeaponAttachmentOffset;
            CurrentWeapon.SetOwner(_character, this);
            CurrentWeapon.WeaponID = weaponID;
            CurrentWeapon.FlipWeapon();

            _weaponAim = CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>();
            HandleWeaponAim();
            HandleWeaponIK();
            HandleWeaponModel(newWeapon, weaponID, combo, CurrentWeapon);

            CurrentWeapon.Initialization();
            CurrentWeapon.InitializeComboWeapons();
            CurrentWeapon.InitializeAnimatorParameters();
            InitializeAnimatorParameters();
        }

        [Serializable]
        public struct AttachmentPoint
        {
            public WeaponAttachmentType Type;
            public Transform Attachment;
        }
    }
}
