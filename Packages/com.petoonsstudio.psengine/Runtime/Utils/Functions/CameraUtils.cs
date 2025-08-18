using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class CameraUtils
    {
        /// <summary>
        /// GameObject is visible by the camera
        /// </summary>
        /// <param name="go"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsTargetVisible(this GameObject go, Camera c)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(c);
            var point = go.transform.position;
            foreach (var plane in planes)
            {
                if (plane.GetDistanceToPoint(point) < 0)
                    return false;
            }
            return true;
        }

        public static Vector2 GetClosestScreenBoundary(Vector3 position)
        {
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(position);
            screenPosition = new Vector2(Mathf.Clamp(screenPosition.x, 0, Screen.width), Mathf.Clamp(screenPosition.y, 0, Screen.height));

            Vector2 newWorldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            return newWorldPosition;
        }
    }
}
