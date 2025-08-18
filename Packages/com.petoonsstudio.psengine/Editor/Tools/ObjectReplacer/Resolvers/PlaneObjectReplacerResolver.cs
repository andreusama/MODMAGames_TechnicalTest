using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PetoonsStudio.PSEngine.Utils
{
    public class PlaneObjectReplacerResolver : DistanceObjectReplacerResolver
    {
        [SerializeField]
        protected Vector3 m_PlaneOffset = Vector3.zero;
        [SerializeField]
        protected Vector3 m_PlaneNormal = Vector3.forward;
        public override VisualElement DrawResolverUI()
        {
            VisualElement root = new VisualElement();

            Vector3Field planeOffsetField = new("Plane Offset");
            planeOffsetField.name = "PlaneOffset";
            planeOffsetField.tooltip = $"Offset of the plane";
            planeOffsetField.value = m_PlaneOffset;
            planeOffsetField.RegisterValueChangedCallback((o) => m_PlaneOffset = o.newValue);
            root.Add(planeOffsetField);

            Vector3Field planeNormalField = new("Plane Normal");
            planeNormalField.name = "PlaneNormal";
            planeNormalField.tooltip = $"Normal of the plane";
            planeNormalField.value = m_PlaneNormal;
            planeNormalField.RegisterValueChangedCallback((o) => m_PlaneNormal = o.newValue);
            root.Add(planeNormalField);

            return root;
        }

        public override Vector3 GetNearestPosition(Transform transform)
        {
            Plane plane = new Plane(m_PlaneNormal, m_PlaneOffset);
            return plane.ClosestPointOnPlane(transform.position);
        }

        public override void DrawHandles(SceneView sceneView)
        {
            base.DrawHandles(sceneView);
            Plane plane = new Plane(m_PlaneNormal.normalized, m_PlaneOffset);


            float size = HandleUtility.GetHandleSize(m_PlaneOffset);
            Color color = new Color(1f, 0f, 0f, 0.5f);
            Handles.color = color;
            Handles.DrawSolidDisc(m_PlaneOffset, m_PlaneNormal, size);
            color = new Color(1f, 1f, 0f, 0.5f);
            Handles.color = color;
            Handles.DrawLine(m_PlaneOffset, (m_PlaneOffset + (m_PlaneNormal.normalized * size)) , 5f);
        }

        public override void DrawSelectedHandle(SceneView sceneView, GameObject selectedGameObject)
        {
            base.DrawSelectedHandle(sceneView, selectedGameObject);

            Plane plane = new Plane(m_PlaneNormal, m_PlaneOffset);
            var closestPoint = plane.ClosestPointOnPlane(selectedGameObject.transform.position);
            float size = HandleUtility.GetHandleSize(selectedGameObject.transform.position);

            Handles.DrawDottedLine(selectedGameObject.transform.position, closestPoint, size);
        }
    }
}
