using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class CollectionExtensions
    {
        public static SerializedDictionary<TKey, TValue> Serialize<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            var r = new SerializedDictionary<TKey, TValue>();

            foreach (var element in dict)
            {
                r.Add(element.Key, element.Value);
            }

            return r;
        }

        public static Dictionary<TKey, TValue> Deserialize<TKey, TValue>(this SerializedDictionary<TKey, TValue> dict)
        {
            var r = new Dictionary<TKey, TValue>();

            foreach (var element in dict)
            {
                r.Add(element.Key, element.Value);
            }

            return r;
        }

        /// <summary>
        /// Shuffle a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Random item of a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T RandomItem<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot select a random item from an empty list");
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Add element to list if not exist yet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        public static void AddNoExist<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
    }
}
