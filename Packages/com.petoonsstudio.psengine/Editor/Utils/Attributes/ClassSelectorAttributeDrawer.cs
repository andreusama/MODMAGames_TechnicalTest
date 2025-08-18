using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [CustomPropertyDrawer(typeof(ClassSelectorAttribute))]
    public class ClassSelectorAttributeDrawer : PropertyDrawer
    {
        public int CurrentIndex;
        private const string NONE_DISPLAY = "None";

        private List<Type> m_CacheAssignableTypes = new List<Type>();
        private string[] m_PossibleTypeNames;
        private bool m_IsDirty = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_IsDirty)
            {
                m_CacheAssignableTypes = GetClassesFromType();
                m_PossibleTypeNames = GetClassesNames(m_CacheAssignableTypes);
                m_IsDirty = false;
            }
            
            EditorGUI.BeginProperty(position, label, property);
            
            if (property.propertyType == SerializedPropertyType.String)
            {
                Rect r = EditorGUI.PrefixLabel(position, label);
                Rect labelRect = position;
                labelRect.xMax = r.xMin;
                position = r;

                CurrentIndex = FindCurrentTypeInList(m_CacheAssignableTypes, property.stringValue);

                CurrentIndex = EditorGUI.Popup(position, CurrentIndex, m_PossibleTypeNames);
                
                if (GUI.changed)
                {
                    if(m_CacheAssignableTypes[CurrentIndex] == null)
                    {
                        property.stringValue = null;

                    }
                    else
                    {
                        property.stringValue = m_CacheAssignableTypes[CurrentIndex].AssemblyQualifiedName;
                    }
                }
            }
            else
            {
                GUI.Label(position, "The ClassSelector attribute can only be used on string variables");
            }
            EditorGUI.EndProperty();
        }

        private int FindCurrentTypeInList(List<Type> classesFromType, string currentType)
        {
            var index = classesFromType.FindIndex(0, classesFromType.Count, x =>
            {
                if (x == null)
                    return false;
                else
                    return x.AssemblyQualifiedName == currentType;
            });

            return index < 0 ? 0 : index;
        }

        private List<Type> GetClassesFromType()
        {
            ClassSelectorAttribute attr = (ClassSelectorAttribute)attribute;
            Type selectorType = attr.SelectableType;

            if (selectorType == null)
                return null;

            var typeArray = ReflectionUtils.GetClassChildren(selectorType, attr.IncludeParent);

            if (attr.IncludeNone)
            {
                typeArray.Insert(0, null);         
            }

            return typeArray;
        }

        private string[] GetClassesNames(List<Type> types)
        {
            string[] typesNames = new string[types.Count()];
            for (int i = 0; i < types.Count(); i++)
            {
                if (types[i] == null)
                    typesNames[i] = NONE_DISPLAY;
                else
                    typesNames[i] = types[i].Name;
            }
            return typesNames;
        }
    }
}

