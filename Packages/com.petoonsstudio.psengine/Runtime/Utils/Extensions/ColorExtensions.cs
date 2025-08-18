using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Creates a base color with defined alpha
        /// </summary>
        /// <param name="self"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color AplhaColor(this Color self, float alpha)
        {
            return new Color(self.r, self.g, self.b, alpha);
        }
    }
}