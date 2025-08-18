using ParadoxNotion.Design;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class SelectAssetNodeCanvasDrawer : AttributeDrawer<SelectAssetNodeCanvasAttribute>
    {
        private List<object> m_AssetCache;

        private const BindingFlags BINDING_FLAGS = BindingFlags.Public | BindingFlags.Instance;

        public override object OnGUI(GUIContent content, object instance)
        {

            if (instance is string assetName)
            {
                if (m_AssetCache == null) FetchAssets(attribute.AssetType);

                var sceneObject = GetAsset(assetName, attribute.ParameterName, attribute.AssetType);
                var asset = EditorGUILayout.ObjectField(sceneObject as Object, attribute.AssetType, true);
                string assetIdentifier = GetIdentifier(asset, attribute.ParameterName, attribute.AssetType);
                if (asset == null)
                {
                    assetName = "";
                }
                else if (assetIdentifier != assetName)
                {
                    var sceneObj = GetAsset(assetIdentifier, attribute.ParameterName, attribute.AssetType);

                    if (sceneObj != null)
                    {
                        assetName = assetIdentifier;
                    }
                }

                return assetName;
            }
            else
            {
                EditorGUILayout.LabelField(content, "Use [SelectAsset] with strings.");
            }

            return instance;
        }

        protected object GetAsset(string assetIdentifier, string propertyName, System.Type assetType)
        {
            if (string.IsNullOrEmpty(assetIdentifier))
            {
                return null;
            }

            foreach (var asset in m_AssetCache)
            {
                string identifier = GetIdentifier(asset, propertyName, assetType);
                if (!string.IsNullOrEmpty(identifier) && identifier.Equals(assetIdentifier))
                {
                    return asset;
                }
            }

            Debug.LogWarning("Asset [" + assetIdentifier + "] couldn't be found.");

            return null;
        }

        private string GetIdentifier(object asset, string propertyName, System.Type assetType)
        {
            if (asset == null) return string.Empty;

            PropertyInfo property = assetType.GetProperties(BINDING_FLAGS).FirstOrDefault((x) => x.Name == propertyName);
            if (property != null)
            {
                return property.GetValue(asset, null) as string;
            }

            FieldInfo field = assetType.GetFields(BINDING_FLAGS).FirstOrDefault((x) => x.Name == propertyName);
            if (field != null)
            {
                return field.GetValue(asset) as string;
            }

            return string.Empty;
        }

        private void FetchAssets(System.Type assetType)
        {
            m_AssetCache = new List<object>();

            var assetGUIDs = AssetDatabase.FindAssets($"t:{assetType.Name}");

            foreach (string guid in assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);
                m_AssetCache.Add(asset);
            }
        }
    }
}
