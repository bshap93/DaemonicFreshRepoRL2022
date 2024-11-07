using System;
using MoreMountains.InventoryEngine;
using Project.Gameplay.Player.Inventory.Models;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(
        fileName = "InventoryCollectable", menuName = "MoreMountains/TopDownEngine/InventoryCollectable", order = 4)]
    [Serializable]
    public class InventoryCollectable : InventoryItem
    {
    }
}
