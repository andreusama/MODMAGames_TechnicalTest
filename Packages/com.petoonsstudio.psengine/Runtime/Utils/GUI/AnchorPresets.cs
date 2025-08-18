using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class AnchorPresets
    {
        public static void TopLeft(RectTransform uitransform)
        {
            uitransform.anchorMin = new Vector2(0, 1);
            uitransform.anchorMax = new Vector2(0, 1);
            uitransform.pivot = new Vector2(0, 1);
        }

        public static void TopMiddle(RectTransform uitransform)
        {
            uitransform.anchorMin = new Vector2(0.5f, 1);
            uitransform.anchorMax = new Vector2(0.5f, 1);
            uitransform.pivot = new Vector2(0.5f, 1);
        }


        public static void TopRight(RectTransform uitransform)
        {
            uitransform.anchorMin = new Vector2(1, 1);
            uitransform.anchorMax = new Vector2(1, 1);
            uitransform.pivot = new Vector2(1, 1);
        }

        public static void MiddleLeft(RectTransform uitransform)
        {
            uitransform.anchorMin = new Vector2(0, 0.5f);
            uitransform.anchorMax = new Vector2(0, 0.5f);
            uitransform.pivot = new Vector2(0, 0.5f);
        }

        public static void Middle(RectTransform uitransform)
        {
            uitransform.anchorMin = new Vector2(0.5f, 0.5f);
            uitransform.anchorMax = new Vector2(0.5f, 0.5f);
            uitransform.pivot = new Vector2(0.5f, 0.5f);
        }

        public static void MiddleRight(RectTransform uitransform)
        {
            uitransform.anchorMin = new Vector2(1, 0.5f);
            uitransform.anchorMax = new Vector2(1, 0.5f);
            uitransform.pivot = new Vector2(1, 0.5f);
        }

        public static void BottomLeft(RectTransform uitransform)
        {
            uitransform.anchorMin = new Vector2(0, 0);
            uitransform.anchorMax = new Vector2(0, 0);
            uitransform.pivot = new Vector2(0, 0);
        }

        public static void BottomMiddle(RectTransform uitransform)
        {
            uitransform.anchorMin = new Vector2(0.5f, 0);
            uitransform.anchorMax = new Vector2(0.5f, 0);
            uitransform.pivot = new Vector2(0.5f, 0);
        }

        public static void BottomRight(RectTransform uitransform)
        {
            uitransform.anchorMin = new Vector2(1, 0);
            uitransform.anchorMax = new Vector2(1, 0);
            uitransform.pivot = new Vector2(1, 0);
        }
    }

}
