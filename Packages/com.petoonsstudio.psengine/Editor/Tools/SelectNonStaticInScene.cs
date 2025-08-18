using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PetoonsStudio.PSEngine.Tools
{
    public static class SelectNonStaticInScene
    {
        [MenuItem("GameObject/Petoons Studio/PSEngine/Tools/Select All Non-Static", false, 300)]
        public static void SelectAllNonStaticGameObjects()
        {
            SelectAllThat(go => !go.isStatic);
        }

        [MenuItem("GameObject/Petoons Studio/PSEngine/Tools/Select All Static", false, 301)]
        public static void SelectAllStaticGameObjects()
        {
            SelectAllThat(go => go.isStatic);
        }

        private static void SelectAllThat(Func<GameObject, bool> check)
        {
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }

            Scene activeScene = SceneManager.GetActiveScene();

            GameObject[] rootGameObjects = activeScene.GetRootGameObjects();

            List<GameObject> allGameObjects = new List<GameObject>(rootGameObjects);

            foreach (GameObject rootGameObject in rootGameObjects)
            {
                AddChildrenRecursive(rootGameObject, allGameObjects);
            }

            Selection.objects = allGameObjects
                .Where(check)
                .Select(go => go as UnityEngine.Object)
                .ToArray();
        }

        private static void AddChildrenRecursive(GameObject target, ICollection<GameObject> collection)
        {
            if (target == null)
            {
                throw new ArgumentNullException("Target");
            }

            if (collection == null)
            {
                throw new ArgumentNullException("Collection");
            }

            for (int I = 0; I < target.transform.childCount; I++)
            {
                GameObject Child = target.transform.GetChild(I).gameObject;
                collection.Add(Child);

                AddChildrenRecursive(Child, collection);
            }
        }
    }

}