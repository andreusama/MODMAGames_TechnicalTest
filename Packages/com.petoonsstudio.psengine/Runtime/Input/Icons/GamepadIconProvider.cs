using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Input
{
    [CreateAssetMenu(fileName = "GamepadIconProvider", menuName = "Petoons Studio/PSEngine/Input/Icons/Provider/Gamepad Icon Provider")]
    public class GamepadIconProvider : ScriptableObject, IIconProvider
    {
        [Serializable]
        private struct IconMap
        {
            public string Map;
            public GamepadIconsMapSO Icon;
        }

        protected Dictionary<string, GamepadIconsMapSO> m_IconMaps;

        [SerializeField] private List<IconMap> m_IconList;
        [SerializeField] private TMP_SpriteAsset m_TextIcons;

        public TMP_SpriteAsset IconTextSprites => m_TextIcons;

        public void Initialize()
        {
            m_IconMaps = new Dictionary<string, GamepadIconsMapSO>();

            foreach (var element in m_IconList)
            {
                m_IconMaps[element.Map] = element.Icon;
            }
        }

        public virtual Sprite GetSprite(string mapType, string controlPath)
        {
            GamepadIconsMapSO icons = m_IconMaps[mapType];

            if (icons == null)
                return null;

            switch (controlPath)
            {
                case "buttonSouth": return icons.ButtonSouth;
                case "buttonEast": return icons.ButtonEast;
                case "buttonNorth": return icons.ButtonNorth;
                case "buttonWest": return icons.ButtonWest;
                case "start": return icons.StartButton;
                case "select": return icons.SelectButton;
                case "leftTrigger": return icons.LeftTrigger;
                case "leftTriggerButton": return icons.LeftTrigger;
                case "rightTrigger": return icons.RightTrigger;
                case "rightTriggerButton": return icons.RightTrigger;
                case "leftShoulder": return icons.LeftShoulder;
                case "leftSL": return icons.LeftShoulder;
                case "rightSL": return icons.LeftShoulder;
                case "rightShoulder": return icons.RightShoulder;
                case "leftSR": return icons.RightShoulder;
                case "rightSR": return icons.RightShoulder;
                case "dpad": return icons.Dpad;
                case "dpad/up": return icons.DpadUp;
                case "dpad/down": return icons.DpadDown;
                case "dpad/left": return icons.DpadLeft;
                case "dpad/right": return icons.DpadRight;
                case "leftStick": return icons.LeftStick;
                case "leftStick/up": return icons.LeftStickUp;
                case "leftStick/down": return icons.LeftStickDown;
                case "leftStick/left": return icons.LeftStickLeft;
                case "leftStick/right": return icons.LeftStickRight;
                case "leftStickPress": return icons.LeftStickPress;
                case "leftStick/clockwise": return icons.LeftStickClockwise;
                case "leftStick/counterclockwise": return icons.LeftStickCounterClockwise;
                case "leftStick/x": return icons.LeftStickX;
                case "leftStick/y": return icons.LeftStickY;
                case "rightStick": return icons.RightStick;
                case "rightStick/up": return icons.RightStickUp;
                case "rightStick/down": return icons.RightStickDown;
                case "rightStick/left": return icons.RightStickLeft;
                case "rightStick/right": return icons.RightStickRight;
                case "rightStickPress": return icons.RightStickPress;
                case "rightStick/clockwise": return icons.RightStickClockwise;
                case "rightStick/counterclockwise": return icons.RightStickCounterClockwise;
                case "rightStick/x": return icons.RightStickX;
                case "rightStick/y": return icons.RightStickY;
                default:
                    break;
            }

            return null;
        }
    }
}