using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace PetoonsStudio.PSEngine.Inventory
{
    abstract public class ItemSO : ScriptableObject
    {
        public string ID;
        public LocalizedString Name;
        public LocalizedString Description;
        public AssetReferenceSprite Image;
    }

    public interface IUsableItem
    {
        public void Use();
    }
}