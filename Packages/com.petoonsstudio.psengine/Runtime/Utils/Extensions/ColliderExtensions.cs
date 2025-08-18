using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class ColliderExtensions
    {
        public static void UpdateShapeToSprite(this PolygonCollider2D collider, Sprite sprite)
        {
            // ensure both valid
            if (collider != null && sprite != null)
            {
                // update count
                collider.pathCount = sprite.GetPhysicsShapeCount();

                // new paths variable
                List<Vector2> path = new List<Vector2>();

                // loop path count
                for (int i = 0; i < collider.pathCount; i++)
                {
                    // clear
                    path.Clear();
                    // get shape
                    sprite.GetPhysicsShape(i, path);
                    // set path
                    collider.SetPath(i, path.ToArray());
                }
            }
        }
    }
}