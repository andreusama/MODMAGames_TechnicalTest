using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class RigidbodyExtensions
    {
        /// <summary>
        /// Explosion force RB 2D
        /// </summary>
        /// <param name="body"></param>
        /// <param name="explosionForce"></param>
        /// <param name="explosionPosition"></param>
        /// <param name="explosionRadius"></param>
        public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius)
        {
            var dir = (body.transform.position - explosionPosition);
            float wearoff = 1 - (dir.magnitude / explosionRadius);

            Vector3 baseForce = dir.normalized * (wearoff <= 0f ? 0f : explosionForce) * wearoff;
            body.AddForce(baseForce, ForceMode2D.Impulse);
        }

        /// <summary>
        /// Explosion force RB 2D
        /// </summary>
        /// <param name="body"></param>
        /// <param name="explosionForce"></param>
        /// <param name="explosionPosition"></param>
        /// <param name="explosionRadius"></param>
        /// <param name="upliftModifier"></param>
        public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier)
        {
            var dir = (body.transform.position - explosionPosition);
            float wearoff = 1 - (dir.magnitude / explosionRadius);
            Vector3 baseForce = dir.normalized * (wearoff <= 0f ? 0f : explosionForce) * wearoff;
            body.AddForce(baseForce, ForceMode2D.Impulse);

            float upliftWearoff = 1 - upliftModifier / explosionRadius;
            Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
            body.AddForce(upliftForce, ForceMode2D.Impulse);
        }
    }
}