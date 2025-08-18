using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class IntegerExtensions
    {
        /// <summary>
        /// Random variation of integer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public static int RandomVariation(this int input, int variation)
        {
            return UnityEngine.Random.Range(input - variation, input + variation);
        }
    }
}