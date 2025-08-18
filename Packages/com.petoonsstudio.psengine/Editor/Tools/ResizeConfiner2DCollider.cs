using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Reflection;
using UnityEditor;

namespace PetoonsStudio.PSEngine.Tools
{
    /// <summary>
    /// Tool to resize a collider of a Cinemachine Confiner 2D to the interior pollygon that is cached (the cached collider is visible with a gizmo of confiner2D)
    /// </summary>
    public static class ResizeConfiner2DCollider
    {

        [MenuItem("GameObject/Petoons Studio/PSEngine/Tools/Resize Confiner2D Collider", false, 302)]
        public static void ResizeColliderConfiner()
        {
            ResizeColliderConfiner(Selection.activeGameObject);
        }

        public static void ResizeColliderConfiner(GameObject go)
        {
#if UNITY_EDITOR
            /// Null checks
            CinemachineVirtualCamera virtualCamera = go.GetComponent<CinemachineVirtualCamera>();
            if (virtualCamera == null)
            {
                Debug.LogError("Selected GameObject doesn't have a VirtualCamera");
                return;
            }
            CinemachineConfiner2D conf2D = virtualCamera.GetComponent<CinemachineConfiner2D>();
            if (conf2D == null)
            {
                Debug.LogError("Selected GameObject doesn't have a Confiner 2D");
                return;
            }
            PolygonCollider2D ColliderToUpdate = (PolygonCollider2D)conf2D.m_BoundingShape2D;
            if (ColliderToUpdate == null)
            {
                Debug.LogError("Selected GameObject doesn't have a PolygonCollider2D referenced on Confiner 2D");
                return;
            }

            /// Create editor to get cached interior polygon
            Editor tmpEditor = Editor.CreateEditor(conf2D);
            var type = tmpEditor.GetType();
            var field = type.GetField("s_currentPathCache", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            List<List<Vector2>> value = (List<List<Vector2>>)field.GetValue(tmpEditor);
            if (value.Count > 0)
            {
                Vector2[] newPoints = value[0].ToArray();
                ColliderToUpdate.points = newPoints;
            }

#endif
        }

    }
}