using System;
using System.Linq;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class SingletonScriptableObjectProcessor : AssetPostprocessor
    {
        private static readonly string ASSET_FOLDER_RESTRICTION = "Assets";
        private static readonly string PLUGIN_FOLDER_IGNORE = "Plugins";

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            HandleNewAssets(importedAssets);
            HandleDeletedAssets(deletedAssets);
        }

        private static void HandleDeletedAssets(string[] deletedAssets)
        {
            for (int i = 0; i < deletedAssets.Length; i++)
            {
                RemoveFromPreloadSettings(deletedAssets[i]);
            }
        }

        private static void HandleNewAssets(string[] importedAssets)
        {
            foreach (string assetPath in importedAssets)
            {
                if (IgnoreByPath(assetPath))
                    continue;

                var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));

                if (asset == null)
                    continue;

                if (IsSingletonScriptableObject(assetPath, asset) && !IsInPreload(asset))
                {
                    AddToPreloadSettings(assetPath);
                }
            }
        }

        private static bool IsInPreload(UnityEngine.Object asset)
        {
            var preloadAssets = PlayerSettings.GetPreloadedAssets().ToList();
            return preloadAssets.Contains(asset);
        }

        private static void RemoveFromPreloadSettings(string assetPath)
        {
            var preloadAssets = PlayerSettings.GetPreloadedAssets().ToList();
            
            for (int i = preloadAssets.Count - 1; i >= 0; --i)
            {
                if (preloadAssets[i] == null)
                {
                    Debug.Log($"Asset from Preload settings: {assetPath} removed");
                    preloadAssets.RemoveAt(i);
                }
            }

            PlayerSettings.SetPreloadedAssets(preloadAssets.ToArray());
        }

        private static void AddToPreloadSettings(string assetPath)
        {
            var preloadAssets = PlayerSettings.GetPreloadedAssets().ToList();
            var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
            preloadAssets.Add(asset);
            Debug.Log($"New asset added to Preload settings: {asset.name} at {assetPath}");
            PlayerSettings.SetPreloadedAssets(preloadAssets.ToArray());
        }

        private static bool IsSingletonScriptableObject(string assetPath, UnityEngine.Object asset)
        {
            return IsSubclassOfRawGeneric(typeof(SingletonScriptableObject<>), asset.GetType());
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        private static bool IgnoreByPath(string assetPath)
        {
            if (!assetPath.Contains(ASSET_FOLDER_RESTRICTION))
                return true;

            if (assetPath.Contains(PLUGIN_FOLDER_IGNORE))
                return true;

            return false;
        }
    }
}
