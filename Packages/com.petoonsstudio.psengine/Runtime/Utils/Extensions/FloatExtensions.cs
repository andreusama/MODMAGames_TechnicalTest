using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class FloatExtensions
    {
        /// <summary>
        /// Random variation of float
        /// </summary>
        /// <param name="input"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public static float RandomVariation(this float input, float variation)
        {
            return UnityEngine.Random.Range(input - variation, input + variation);
        }

        /// <summary>
        /// Linear remapping for sliders
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueRangeMin"></param>
        /// <param name="valueRangeMax"></param>
        /// <param name="newRangeMin"></param>
        /// <param name="newRangeMax"></param>
        /// <returns></returns>
        public static float LinearRemap(this float value,
                                     float valueRangeMin, float valueRangeMax,
                                     float newRangeMin, float newRangeMax)
        {
            return (value - valueRangeMin) / (valueRangeMax - valueRangeMin) * (newRangeMax - newRangeMin) + newRangeMin;
        }
    }
}
