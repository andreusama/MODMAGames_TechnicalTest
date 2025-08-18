﻿using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    /// <summary>
    /// Persistent singleton.
    /// </summary>
    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;
        protected bool _enabled;

        /// <summary>
        /// Singleton design pattern
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Returns whether an instance has been created or not.
        /// </summary>
        public static bool InstanceExists { 
            get 
            { 
                return _instance != null; 
            } 
        }

        /// <summary>
        /// On awake, we check if there's already a copy of the object in the scene. If there's one, we destroy it.
        /// </summary>
        protected virtual void Awake()
        {

            if (_instance == null)
            {
                //If I am the first instance, make me the Singleton
                _instance = this as T;
                transform.parent = null;
                _enabled = true;
            }

            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
            {
                Destroy(this);
            }
            else
            {
                DontDestroyOnLoad(this);
            }
        }
    }
}
