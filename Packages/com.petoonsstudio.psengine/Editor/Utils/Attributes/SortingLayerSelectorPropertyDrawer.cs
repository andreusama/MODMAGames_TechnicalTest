using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [CustomPropertyDrawer(typeof(SortingLayerSelectorAttribute))]
    public class SortingLayerSelectorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string[] sortingLayers = new string[SortingLayer.layers.Length];

            for(int i=0; i< sortingLayers.Length;++i)
            {
                sortingLayers[i] = SortingLayer.layers[i].name;
            }

            property.intValue = EditorGUI.Popup(position, label.text, property.intValue, sortingLayers);
        }
    }
}