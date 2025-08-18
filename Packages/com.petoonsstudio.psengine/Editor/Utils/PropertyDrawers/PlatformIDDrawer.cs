using UnityEditor;
using UnityEngine;
using PlatformID = PetoonsStudio.PSEngine.Multiplatform.PlatformID;

namespace PetoonsStudio.PSEngine
{
    [CustomPropertyDrawer(typeof(PlatformID))]
    public class PlatformIDDrawer : PropertyDrawer
    {
        private const int PLATFORM_ENUM_WIDTH = 120;
        private const int PLATFORM_SPACE = 5;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var platformEnumRect = new Rect(position.x, position.y, PLATFORM_ENUM_WIDTH, position.height);
            var IDRect = new Rect(position.x + PLATFORM_ENUM_WIDTH + PLATFORM_SPACE, position.y, position.width - PLATFORM_ENUM_WIDTH + PLATFORM_SPACE, position.height);

            EditorGUI.PropertyField(platformEnumRect, property.FindPropertyRelative("Platform"), GUIContent.none);
            EditorGUI.PropertyField(IDRect, property.FindPropertyRelative("ID"), GUIContent.none);

            EditorGUI.EndProperty();
        }
        
    }
}
