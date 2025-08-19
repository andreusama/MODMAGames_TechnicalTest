using UnityEditor;
using UnityEngine;

namespace GameUtils
{
    public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        protected static T _instance;

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
#if UNITY_EDITOR
                    string[] guid = AssetDatabase.FindAssets("t:" + typeof(T).ToString());
                    if (guid.Length == 0)
                    {
                        throw new System.Exception("Asset not found");
                    }
                    else
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid[0]);
                        _instance = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
                    }
#else
                    _instance = FindObjectOfType<T>();
#endif
                }
                return _instance;
            }
        }

        /// <summary>
        /// Returns whether an Instance exists or not.
        /// </summary>
        public static bool InstanceExists
        {
            get
            {
                return _instance != null;
            }
        }

        /// <summary>
        /// On awake, we initialize our instance. Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            _instance = this as T;
        }
    }
}