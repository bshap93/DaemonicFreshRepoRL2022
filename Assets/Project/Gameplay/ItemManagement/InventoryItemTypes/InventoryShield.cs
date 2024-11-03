using System;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.ItemManagement.ItemClasses;
using Project.Gameplay.ItemManagement.ItemUseAbilities;
using TopDownEngine.Common.Scripts.Characters.Core;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(fileName = "InventoryShield", menuName = "MoreMountains/TopDownEngine/InventoryShield", order = 3)]
    [Serializable]
    public class InventoryShield : InventoryItem
    {
        [Header("Shield")]
        [MMInformation(
            "Bind the shield you want to equip when picking this item.", MMInformationAttribute.InformationType.Info,
            false)]
        public Shield EquippableShield;

        [Header("Auto Equip")] [Tooltip("Whether to automatically equip this shield when picked up")]
        public bool AutoEquip;

        [Header("Handle ID")] [Tooltip("The ID of the CharacterHandleShield you want this shield to be equipped to")]
        public int HandleShieldID = 1;

        public override bool Equip(string playerID)
        {
            EquipShield(EquippableShield, playerID);
            return true;
        }

        public override bool UnEquip(string playerID)
        {
            if (TargetEquipmentInventory(playerID) == null) return false;

            if (TargetEquipmentInventory(playerID).InventoryContains(ItemID).Count > 0) EquipShield(null, playerID);

            return true;
        }

        protected virtual void EquipShield(Shield newShield, string playerID)
        {
            if (TargetInventory(playerID).Owner == null) return;

            var character = TargetInventory(playerID).Owner.GetComponent<Character>();
            if (character == null) return;

            CharacterHandleShield targetHandleShield = null;
            var handleShields = character.GetComponentsInChildren<CharacterHandleShield>();

            foreach (var handleShield in handleShields)
                if (handleShield.HandleShieldID == HandleShieldID)
                {
                    targetHandleShield = handleShield;
                    break;
                }

            if (targetHandleShield != null) targetHandleShield.EquipShield(newShield);
        }
    }
}
