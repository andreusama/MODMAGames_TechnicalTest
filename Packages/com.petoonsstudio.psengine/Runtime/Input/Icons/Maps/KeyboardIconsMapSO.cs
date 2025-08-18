using PetoonsStudio.PSEngine.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Input
{
    [CreateAssetMenu(fileName = "Icons", menuName = "Petoons Studio/PSEngine/Input/Icons/Maps/Keyboard")]
    public class KeyboardIconsMapSO : DeviceIconsMapSO
    {
        [System.Serializable]
        public struct SpecialKey
        {
            public string Key;
            public Sprite Icon;
        }

        public Sprite RectangleKey;
        public Sprite StandardKey;
        public List<SpecialKey> SpecialKeyList;
    }
}

