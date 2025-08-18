using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class CanvasGroupExtensions
    {
        /// <summary>
        /// Show Canvas group
        /// </summary>
        /// <param name="canvas"></param>
        public static void ShowCanvasGroup(this CanvasGroup canvas)
        {
            canvas.interactable = true;
            canvas.alpha = 1f;
        }

        /// <summary>
        /// Show Canvas group
        /// </summary>
        /// <param name="canvas"></param>
        public static void HideCanvasGroup(this CanvasGroup canvas)
        {
            canvas.interactable = false;
            canvas.alpha = 0f;
        }
    }
}