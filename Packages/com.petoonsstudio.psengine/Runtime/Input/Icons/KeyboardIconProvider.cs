using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Input
{
    [CreateAssetMenu(fileName = "KeyboardProvider", menuName = "Petoons Studio/PSEngine/Input/Icons/Provider/Keyboard Icon Provider")]
    public class KeyboardIconProvider : ScriptableObject, IIconProvider
    {
        [Serializable]
        private struct IconMap
        {
            public string Map;
            public KeyboardIconsMapSO Icon;
        }

        private Dictionary<string, KeyboardIconsMapSO> m_IconMaps;

        [SerializeField] private List<IconMap> m_IconList;
        [SerializeField] private TMP_SpriteAsset m_TextIcons;

        public TMP_SpriteAsset IconTextSprites => m_TextIcons;

        public void Initialize()
        {
            m_IconMaps = new Dictionary<string, KeyboardIconsMapSO>();

            foreach (var element in m_IconList)
            {
                m_IconMaps[element.Map] = element.Icon;
            }
        }

        public bool IsSpecialKey(string controlPath, string mapKey)
        {
            foreach(var element in m_IconMaps[mapKey].SpecialKeyList)
            {
                if (element.Key == controlPath)
                    return true;
            }

            return false;
        }

        public Sprite GetSpecialKey(string controlPath, string mapKey)
        {
            foreach (var element in m_IconMaps[mapKey].SpecialKeyList)
            {
                if (element.Key == controlPath)
                    return element.Icon;
            }

            return null;
        }

        public Sprite GetSprite(string mapType, string controlPath)
        {
            KeyboardIconsMapSO icons = m_IconMaps[mapType];

            if (icons == null)
                return null;

            if (controlPath.Length == 1)
            {
                return icons.StandardKey;
            }
            else
            {
                Sprite specialKeySprite = GetSpecialKey(controlPath, mapType);

                if (specialKeySprite != null) return specialKeySprite;
                return icons.RectangleKey;
            }
        }
    }
}