using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class SceneExtensions
    {
        /// <summary>
        /// Set root gameobjects as active
        /// </summary>
        /// <param name="scene"></param>
        public static void Show(this Scene scene)
        {
            foreach (var gameObject in scene.GetRootGameObjects())
            {
                gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Set root gameobjects as not active
        /// </summary>
        /// <param name="scene"></param>
        public static void Hide(this Scene scene)
        {
            foreach (var gameObject in scene.GetRootGameObjects())
            {
                gameObject.SetActive(false);
            }
        }
    }
}
