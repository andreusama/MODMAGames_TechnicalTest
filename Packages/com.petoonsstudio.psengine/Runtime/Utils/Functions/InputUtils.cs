using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class InputUtils
    {
        public enum KeyboardLayout
        {
            QWERTY,
            AZERTY,
            DVORAK
        }

        /// <summary>
        /// Get the Input Control from a key of the actual keyboard.
        /// </summary>
        /// <param name="controlPath"></param>
        /// <returns></returns>
        public static InputControl GetControlKey(string controlPath)
        {
            return GetCurrent()[controlPath];
        }

        /// <summary>
        /// Get the current keyboard.
        /// </summary>
        /// <returns></returns>
        public static Keyboard GetCurrent()
        {
            return Keyboard.current;
        }

        /// <summary>
        /// Get the actual Keyboard layout from a comparation of keys.
        /// It can detect:
        /// QWERTY, AZERTY and DVORAK
        /// </summary>
        /// <returns></returns>
        public static KeyboardLayout GetKeyboardLayout()
        {
            string keyToCheck = "W";

            switch (GetControlKey(keyToCheck).displayName)
            {
                case "W":
                    return KeyboardLayout.QWERTY;
                case ",":
                    return KeyboardLayout.DVORAK;
                case "Z":
                    return KeyboardLayout.AZERTY;
                default:
                    return KeyboardLayout.QWERTY;
            }
        }

        /// <summary>
        /// Returns if the key is a Special key like: space, enter, escape, backspace
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsSpecialKey(string key)
        {
            switch (key)
            {
                case "backspace":
                case "enter":
                case "escape":
                case "space":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get all keyboards founded.
        /// </summary>
        /// <returns></returns>
        private static Keyboard[] GetAllKeyboards()
        {
            return InputSystem.devices.Select(device => device as Keyboard)
                                .Where(device => device != null)
                                .ToArray();
        }
    }
}

