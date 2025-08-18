using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class GameObjectExtensions
    {
        static List<Component> m_ComponentCache = new List<Component>();

        /// <summary>
        /// Determines if an gameobject has component
        /// </summary>
        /// <returns><c>true</c> if has parameter of type the specified self name type; otherwise, <c>false</c>.</returns>
        /// <param name="self">Self.</param>
        /// <param name="name">Name.</param>
        /// <param name="type">Type.</param>
        public static bool HasComponent<T>(this GameObject self)
        {
            return self.GetComponent<T>() != null;
        }

        /// <summary>
        /// Return the closest gameobject to self
        /// </summary>
        /// <param name="self"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static GameObject GetClosestGameObject(this GameObject self, GameObject[] list)
        {
            GameObject closestGameObject = null;
            float minDistance = Mathf.Infinity;
            foreach (var obj in list)
            {
                float distance = Vector3.Distance(obj.transform.position, self.transform.position);

                if (distance < minDistance)
                {
                    closestGameObject = obj;
                    minDistance = distance;
                }
            }

            return closestGameObject;
        }

        public static T GetOrAddComponent<T>(this GameObject target) where T : Component
        {
            T component = target.GetComponent<T>();
            if (!component) component = target.AddComponent<T>();
            return component;
        }

        public static void ChangeLayerRecursive(this GameObject target, int layer)
        {
            target.layer = layer;

            foreach (Transform child in target.transform.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = layer;
            }
        }

        /// <summary>
        /// Get component without allocation memory
        /// </summary>
        /// <param name="this"></param>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static Component GetComponentNoAlloc(this GameObject @this, System.Type componentType)
        {
            @this.GetComponents(componentType, m_ComponentCache);
            var component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
            m_ComponentCache.Clear();
            return component;
        }

        /// <summary>
        /// Get component without allocation memory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T GetComponentNoAlloc<T>(this GameObject @this) where T : Component
        {
            @this.GetComponents(typeof(T), m_ComponentCache);
            var component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
            m_ComponentCache.Clear();
            return component as T;
        }

        /// <summary>
        /// Return the maximum bounds of all the children renderers
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static Bounds GetMaxBounds(this GameObject @this)
        {
            var bounds = new Bounds(@this.transform.position, Vector3.zero);
            foreach (Renderer r in @this.GetComponentsInChildren<Renderer>())
            {
                if (r is SkinnedMeshRenderer || r is MeshRenderer)
                    bounds.Encapsulate(r.bounds);
            }
            return bounds;
        }
    }
}