using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class GUIEdtitorUtils
    {
        public static string SeparateCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "(\\B[A-Z])", " $1");
        }

        public static Vector3 DrawDirectionSelector(ref bool customDirection, Vector3 direction, bool invertDir = false, float buttonMaxWidth = 52, bool showNormalizeBtn = true)
        {
            EditorGUILayout.BeginHorizontal();
            if (customDirection)
            {
                customDirection = !GUILayout.Toggle(!customDirection, "< Preset", "Button");
                direction = EditorGUILayout.Vector3Field("", direction);
                if (GUILayout.Button("Reset", GUILayout.MaxWidth(buttonMaxWidth))) direction = Vector3.right;
            }
            else
            {
                GUILayout.FlexibleSpace();

                direction = DrawDirectionSelectorButton("Left", invertDir ? Vector3.right : Vector3.left, direction, buttonMaxWidth);
                direction = DrawDirectionSelectorButton("Right", invertDir ? Vector3.left : Vector3.right, direction, buttonMaxWidth);
                direction = DrawDirectionSelectorButton("Top", invertDir ? Vector3.down : Vector3.up, direction, buttonMaxWidth);
                direction = DrawDirectionSelectorButton("Bottom", invertDir ? Vector3.up : Vector3.down, direction, buttonMaxWidth);
                direction = DrawDirectionSelectorButton("Front", invertDir ? Vector3.forward : Vector3.back, direction, buttonMaxWidth);
                direction = DrawDirectionSelectorButton("Back", invertDir ? Vector3.back : Vector3.forward, direction, buttonMaxWidth);
                customDirection = GUILayout.Toggle(customDirection, "Custom >", "Button");
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            if (customDirection && GUILayout.Button("Normalize"))
            {
                direction = direction.normalized;
            }

            return direction;
        }

        private static Vector3 DrawDirectionSelectorButton(string name, Vector3 buttonDirection, Vector3 direction, float buttonMaxWidth)
        {
            bool selected = buttonDirection == direction;

            GUI.enabled = !selected;

            if (GUILayout.Button(name, GUILayout.Width(buttonMaxWidth)))
            {
                GUI.enabled = true;
                return buttonDirection;
            }

            GUI.enabled = true;
            return direction;
        }
    }
}

