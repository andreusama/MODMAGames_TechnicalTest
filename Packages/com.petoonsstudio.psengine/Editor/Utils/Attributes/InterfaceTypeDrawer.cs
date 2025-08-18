using UnityEngine;
using UnityEditor;
using System.Linq;

namespace PetoonsStudio.PSEngine.Utils
{
    /// <summary>
    /// Drawer for the RequireInterface attribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(InterfaceTypeAttribute))]
    public class InterfaceTypeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Overrides GUI drawing for the attribute.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="property">Property.</param>
        /// <param name="label">Label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InterfaceTypeAttribute att = attribute as InterfaceTypeAttribute;

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, label.text, "InterfaceType Attribute can only be used with MonoBehaviour Components!");
                return;
            }

            // Pick a specific component
            MonoBehaviour oldComp = property.objectReferenceValue as MonoBehaviour;

            GameObject temp = null;
            string oldName = "";

            if (Event.current.type == EventType.Repaint)
            {
                if (oldComp == null)
                {
                    temp = new GameObject("None [" + att.type.Name + "]");
                    oldComp = temp.AddComponent<MonoInterface>();
                }
                else
                {
                    oldName = oldComp.name;
                    oldComp.name = oldName + " [" + att.type.Name + "]";
                }
            }

            MonoBehaviour comp = EditorGUI.ObjectField(position, label, oldComp, typeof(MonoBehaviour), true) as MonoBehaviour;

            if (Event.current.type == EventType.Repaint)
            {
                if (temp != null)
                    GameObject.DestroyImmediate(temp);
                else
                    oldComp.name = oldName;
            }

            // Make sure something changed.
            if (oldComp == comp) return;

            // If a component is assigned, make sure it is the interface we are looking for.
            if (comp != null)
            {
                // Make sure component is of the right interface
                if (comp.GetType() != att.type)
                    // Component failed. Check game object.
                    comp = comp.gameObject.GetComponent(att.type) as MonoBehaviour;

                // Item failed test. Do not override old component
                if (comp == null) return;
            }

            property.objectReferenceValue = comp;
            property.serializedObject.ApplyModifiedProperties();
        }
    }

    public class MonoInterface : MonoBehaviour
    {
    }
}