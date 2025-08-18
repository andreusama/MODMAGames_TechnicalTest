using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class PlayerInputExtensions
    {
        public static void DisableAllActions(this PlayerInput input)
        {
            foreach(var action in input.actions)
            {
                action.Disable();
            }
        }
    }
}