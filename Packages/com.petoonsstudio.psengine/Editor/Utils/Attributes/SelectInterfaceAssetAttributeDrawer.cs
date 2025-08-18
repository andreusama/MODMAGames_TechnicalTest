using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [CustomPropertyDrawer(typeof(SelectInterfaceAssetAttribute))]
    public class SelectInterfaceAssetAttributeDrawer : PropertyDrawer
    {
        private List<object> m_AssetCache;

        private const BindingFlags BINDING_FLAGS = BindingFlags.Public | BindingFlags.Instance;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var selectAttribute = attribute as SelectInterfaceAssetAttribute;

            if (m_AssetCache == null) FetchAssets(selectAttribute.AssetType, selectAttribute.AssetFilter);

            if (property.propertyType == SerializedPropertyType.String)
            {
                var sceneObject = GetAsset(property.stringValue, selectAttribute.ParameterName, selectAttribute.AssetFilter);
                var asset = EditorGUI.ObjectField(position, label, sceneObject as Object, selectAttribute.AssetFilter, true);
                string assetIdentifier = GetIdentifier(asset, selectAttribute.ParameterName, selectAttribute.AssetFilter);
                if (asset == null)
                {
                    property.stringValue = "";
                }
                else if (assetIdentifier != property.stringValue)
                {
                    var sceneObj = GetAsset(assetIdentifier, selectAttribute.ParameterName, selectAttribute.AssetFilter);

                    if (sceneObj != null)
                    {
                        property.stringValue = assetIdentifier;
                    }
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [SelectAsset] with strings.");
            }
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

        private void FetchAssets(System.Type assetType, System.Type assetFilter)
        {
            m_AssetCache = new List<object>();

            var assetGUIDs = AssetDatabase.FindAssets($"t:{assetType}");

            foreach (string guid in assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);

                if (asset.GetType().GetInterfaces().Contains(assetFilter))
                    m_AssetCache.Add(asset);
            }
        }
    }
}
