using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class StringExtensions
    {
        /// <summary>
        /// First character of string to upper
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        /// <summary>
        /// Retrieves a string contained between an initial string and end string.
        /// Example: "Bert likes tomatoes.".GetStringBetween("Bert", "tomatoes") == " likes "
        /// </summary>
        /// <param name="source">String on which we perform this search.</param>
        /// <param name="start">The part right before the needed string.</param>
        /// <param name="end">The part right after the needed string.</param>
        /// <returns></returns>
        public static string GetStringBetween(this string source, string start, string end)
        {
            if (source.Contains(start) && source.Contains(end))
            {
                int startIndex, endIndex;
                startIndex = source.IndexOf(start) + start.Length;
                endIndex = source.IndexOf(end, startIndex);
                return source.Substring(startIndex, endIndex - startIndex);
            }

            return "";
        }
    }
}