using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class FindUtils
    {
        /// <summary>
        /// Find Objects Of type inside specific scene
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static T[] FindObjectsOfTypeInScene<T>(string sceneName) where T : Component
        {
            return UnityEngine.Object.FindObjectsOfType<T>().ToList().Where(x => x.gameObject.scene.name == sceneName).ToArray();
        }

        public static T GetComponentInActiveScene<T>()
        {
            GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var obj in gameObjects)
            {
                T find = obj.GetComponent<T>();
                if (find != null)
                {
                    return find;
                }
            }

            return default;
        }


    }
}

