using PetoonsStudio.PSEngine.Utils;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Inventory
{
    [System.Serializable]
    public class InventoryItem
    {
        public InventoryItemStack StackedItem;
        public int MaxQuantity;

        public InventoryItem(string item, int quantity, int maxQuantity)
        {
            StackedItem = new InventoryItemStack();
            StackedItem.ItemID = item;
            StackedItem.Quantity = quantity;

            MaxQuantity = maxQuantity;
        }
    }

    [System.Serializable]
    public struct InventoryItemStack
    {
        [SelectAsset(typeof(ItemSO), nameof(ItemSO.ID))] public string ItemID;
        public int Quantity;
    }
}