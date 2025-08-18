using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Inventory
{
    [CreateAssetMenu(fileName = "Inventory Preset", menuName = "Petoons Studio/Holy Night/Inventory/Preset")]
    public class InventoryPreset : ScriptableObject
    {
        public List<InventoryItemStack> Items;
    }
}