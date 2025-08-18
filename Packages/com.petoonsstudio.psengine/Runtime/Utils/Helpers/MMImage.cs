using System.Collections;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    /// <summary>
    /// Various static methods used throughout the Infinite Runner Engine and the Corgi Engine.
    /// </summary>

    public class MMImage : MonoBehaviour
    {
        /// <summary>
        /// Fades the specified image to the target opacity and duration.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="opacity">Opacity.</param>
        /// <param name="duration">Duration.</param>
        public static IEnumerator ColorLerp(SpriteRenderer renderer, float duration, Color color)
        {
            if (renderer == null)
            {
                yield break;
            }

            float t = 0f;
            while (t < 1.0f)
            {
                renderer.color = Color.Lerp(renderer.color, color, t);
                t += Time.deltaTime / duration;
                yield return null;
            }
        }

        /// <summary>
        /// Set the specified image to transparent
        /// </summary>
        /// <param name="renderer"></param>
        public static float SetAlpha(SpriteRenderer renderer, float value)
        {
            Color color = renderer.color;
            float oldTransparency = color.a;

            color.a = value;

            renderer.color = color;
            return oldTransparency;
        }

        /// <summary>
        /// Coroutine used to make the character's material flicker (when hurt for example).
        /// </summary>
        public static IEnumerator Flicker(Renderer renderer, Color initialColor, Color flickerColor, float flickerSpeed, float flickerDuration)
        {
            if (renderer == null)
            {
                yield break;
            }

            if (!renderer.material.HasProperty("_Color"))
            {
                yield break;
            }

            if (initialColor == flickerColor)
            {
                yield break;
            }

            float flickerStop = Time.time + flickerDuration;

            while (Time.time < flickerStop)
            {
                renderer.material.color = flickerColor;
                yield return new WaitForSeconds(flickerSpeed);
                renderer.material.color = initialColor;
                yield return new WaitForSeconds(flickerSpeed);
            }

            renderer.material.color = initialColor;
        }

        /// <summary>
        /// Coroutine used to make the character's sprite renderer (when hurt for example).
        /// </summary>
        public static IEnumerator Flicker(SpriteRenderer renderer, Color initialColor, Color flickerColor, float flickerSpeed, float flickerDuration)
        {
            if (renderer == null)
            {
                yield break;
            }

            if (initialColor == flickerColor)
            {
                yield break;
            }

            float flickerStop = Time.time + flickerDuration;

            while (Time.time < flickerStop)
            {
                renderer.color = flickerColor;
                yield return new WaitForSeconds(flickerSpeed);
                renderer.color = initialColor;
                yield return new WaitForSeconds(flickerSpeed);
            }

            renderer.color = initialColor;
        }
    }
}

