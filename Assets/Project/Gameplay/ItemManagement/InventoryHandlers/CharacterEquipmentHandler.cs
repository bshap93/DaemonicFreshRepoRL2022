using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.ItemManagement.InventoryItemTypes;
using Project.Gameplay.ItemManagement.ItemUseAbilities;
using TopDownEngine.Common.Scripts.Characters.CharacterAbilities;
using UnityEngine;

namespace Project.Gameplay.ItemManagement
{
    /// <summary>
    ///     Base class for equipment models that integrate with the TopDownEngine's model system
    /// </summary>
    public class EquipmentModel : MonoBehaviour
    {
        [Header("Identification")] [Tooltip("Unique ID to match this model with its corresponding equipment")]
        public string EquipmentID = "EquipmentID";

        [Header("Visualization")] [Tooltip("The model to show/hide for this equipment")]
        public GameObject TargetModel;

        [Header("Animation")] [Tooltip("Whether to use IK with this model")]
        public bool UseIK;
        [Tooltip("Whether to add the animator to equipment's animator list")]
        public bool AddAnimator;
        public Animator TargetAnimator;

        [Header("Model Attachment")] [Tooltip("Transform to use as the equipment attachment point")]
        public Transform AttachmentPoint;

        [Header("Feedbacks")] public bool BindFeedbacks = true;
        public MMFeedbacks EquipFeedback;
        public MMFeedbacks UnequipFeedback;
        public MMFeedbacks EquipmentUseFeedback;

        public virtual CharacterEquipmentHandler Owner { get; protected set; }

        protected virtual void Awake()
        {
            Hide();
        }

        public virtual void Show(CharacterEquipmentHandler handler)
        {
            Owner = handler;
            if (TargetModel != null) TargetModel.SetActive(true);

            if (BindFeedbacks)
            {
                EquipFeedback?.Initialization(gameObject);
                UnequipFeedback?.Initialization(gameObject);
                EquipmentUseFeedback?.Initialization(gameObject);
            }
        }

        public virtual void Hide()
        {
            if (TargetModel != null) TargetModel.SetActive(false);
        }
    }

    /// <summary>
    ///     Main handler for character equipment, coordinates between inventory and equipment systems
    /// </summary>
    public class CharacterEquipmentHandler : CharacterAbility
    {
        [Header("Equipment Setup")] public string EquipmentInventoryName = "EquipmentInventory";
        public string MainInventoryName = "MainInventory";

        [Header("Equipment Slots")]
        [MMInformation(
            "Define all available equipment slots and their associated mount points",
            MMInformationAttribute.InformationType.Info, false)]
        public List<EquipmentSlotDefinition> EquipmentSlots = new();
        protected Inventory _equipmentInventory;
        protected List<EquipmentModel> _equipmentModels;
        protected Inventory _mainInventory;

        protected Dictionary<EquipmentSlotType, EquipmentSlotDefinition> _slotMap;

        public override string HelpBoxText()
        {
            return "Handles equipping and unequipping of items through the inventory system.";
        }

        protected override void Initialization()
        {
            base.Initialization();

            // Initialize slot mapping
            _slotMap = new Dictionary<EquipmentSlotType, EquipmentSlotDefinition>();
            foreach (var slot in EquipmentSlots) _slotMap[slot.SlotType] = slot;

            // Find equipment models
            _equipmentModels = new List<EquipmentModel>();
            foreach (var model in _character.gameObject.GetComponentsInChildren<EquipmentModel>())
                _equipmentModels.Add(model);

            // Bind inventories
            _equipmentInventory = Inventory.FindInventory(EquipmentInventoryName, _character.PlayerID);
            _mainInventory = Inventory.FindInventory(MainInventoryName, _character.PlayerID);

            if (_equipmentInventory == null || _mainInventory == null)
                Debug.LogWarning(
                    $"CharacterEquipmentHandler on {gameObject.name} could not find required inventories!");
        }

        public virtual bool EquipItem(InventoryItem item, EquipmentSlotType targetSlot)
        {
            if (!_slotMap.TryGetValue(targetSlot, out var slotDef)) return false;

            // Unequip current item if any
            if (slotDef.EquippedItem != null) UnequipItem(slotDef);

            // Handle specific equipment types
            switch (targetSlot)
            {
                case EquipmentSlotType.RightHand:
                    HandleWeaponEquip(item as InventoryWeapon, slotDef);
                    break;

                case EquipmentSlotType.LeftHand:
                    if (item is InventoryWeapon && slotDef.AllowDualWield)
                        HandleWeaponEquip(item as InventoryWeapon, slotDef);
                    else if (item is InventoryShield) HandleShieldEquip(item as InventoryShield, slotDef);

                    break;

                // Add cases for other equipment types
            }

            // Update equipment models
            UpdateEquipmentModels(item.ItemID, true);

            slotDef.EquippedItem = item;
            return true;
        }

        protected virtual void HandleWeaponEquip(InventoryWeapon weapon, EquipmentSlotDefinition slot)
        {
            if (weapon?.EquippableWeapon == null) return;

            var targetHandler = _character.FindAbilityByID<CharacterHandleWeapon>(slot.HandlerID);
            if (targetHandler != null) targetHandler.ChangeWeapon(weapon.EquippableWeapon, weapon.ItemID);
        }

        protected virtual void HandleShieldEquip(InventoryShield shield, EquipmentSlotDefinition slot)
        {
            if (shield?.EquippableShield == null) return;

            var shieldHandler = _character.FindAbilityByID<CharacterHandleShield>(slot.HandlerID);
            if (shieldHandler != null) shieldHandler.EquipShield(shield.EquippableShield);
        }

        protected virtual void UnequipItem(EquipmentSlotDefinition slot)
        {
            if (slot.EquippedItem == null) return;

            // Handle unequip based on slot type
            switch (slot.SlotType)
            {
                case EquipmentSlotType.RightHand:
                case EquipmentSlotType.LeftHand:
                    var weaponHandler = _character.FindAbilityByID<CharacterHandleWeapon>(slot.HandlerID);
                    if (weaponHandler != null) weaponHandler.ChangeWeapon(null, "");

                    var shieldHandler = _character.FindAbilityByID<CharacterHandleShield>(slot.HandlerID);
                    if (shieldHandler != null) shieldHandler.EquipShield(null);
                    break;
            }

            UpdateEquipmentModels(slot.EquippedItem.ItemID, false);
            slot.EquippedItem = null;
        }

        protected virtual void UpdateEquipmentModels(string equipmentID, bool equipped)
        {
            foreach (var model in _equipmentModels)
                if (model.EquipmentID == equipmentID)
                {
                    if (equipped)
                        model.Show(this);
                    else
                        model.Hide();
                }
        }

        public virtual void ForceUnequipAll()
        {
            foreach (var slot in EquipmentSlots)
                if (slot.EquippedItem != null)
                    UnequipItem(slot);
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            ForceUnequipAll();
        }

        protected override void OnRespawn()
        {
            base.OnRespawn();
            // Optionally restore equipment
        }

        [Serializable]
        public class EquipmentSlotDefinition
        {
            public EquipmentSlotType SlotType;
            public Transform MountPoint;
            public int HandlerID = 1; // For compatibility with existing weapon/shield handlers
            public bool AllowDualWield;
            [MMReadOnly] public InventoryItem EquippedItem;
        }
    }

    public enum EquipmentSlotType
    {
        None,
        RightHand,
        LeftHand,
        Head,
        Chest,
        Legs,
        Feet,
        Shoulders,
        Hands,
        Back,
        Neck,
        Ring1,
        Ring2
    }
}
