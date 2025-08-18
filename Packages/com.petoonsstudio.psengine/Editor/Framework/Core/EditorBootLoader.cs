using PetoonsStudio.PSEngine.Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PetoonsStudio.PSEngine
{
    public static class EditorBootLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void Preload()
        {
            if (HasToInstantiateBootEditorScene())
            {
                var prefab = (GameObject)EditorGUIUtility.Load(PetoonsBootEditorProjectSettings.GetOrCreateSettings().EditorBootPrefabPath);
                if (prefab != null)
                    GameObject.Instantiate(prefab);
                else
                    Debug.LogError("No MANAGERS found!!. Check Project Settings / Boot Editor to check if the prefab exists!");
            }
        }

        private static bool HasToInstantiateBootEditorScene()
        {
            return PetoonsBootEditorProjectSettings.GetOrCreateSettings().Enabled && 
                SceneManager.GetSceneByName(PetoonsBootEditorProjectSettings.GetOrCreateSettings().BootScene) != null &&
                !SceneManager.GetSceneByName(PetoonsBootEditorProjectSettings.GetOrCreateSettings().BootScene).isLoaded;
        }
    }
}
