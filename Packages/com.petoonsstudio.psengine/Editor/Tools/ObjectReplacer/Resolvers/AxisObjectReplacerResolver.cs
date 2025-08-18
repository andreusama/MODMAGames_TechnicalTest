using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PetoonsStudio.PSEngine.Utils
{
    [Serializable]
    public class AxisObjectReplacerResolver : DistanceObjectReplacerResolver
    {
        protected enum Axis
        {
            X,
            Y,
            Z
        }
        [SerializeField]
        protected Axis m_ResolverAxis;
        [SerializeField]
        protected Vector3 m_DistanceFromAxis;
        public override Vector3 GetNearestPosition(Transform transform)
        {
            Vector3 resolvedPosition = transform.position;
            resolvedPosition.x = m_ResolverAxis == Axis.Y || m_ResolverAxis == Axis.Z ? 0 + m_DistanceFromAxis.x : resolvedPosition.x;
            resolvedPosition.y = m_ResolverAxis == Axis.X || m_ResolverAxis == Axis.Z ? 0 + m_DistanceFromAxis.y : resolvedPosition.y;
            resolvedPosition.z = m_ResolverAxis == Axis.Y || m_ResolverAxis == Axis.X ? 0 + m_DistanceFromAxis.z : resolvedPosition.z;

            return resolvedPosition;
        }
        public override VisualElement DrawResolverUI()
        {
            VisualElement root = new VisualElement();

            DropdownField axisSelectorField = new("Axis", Enum.GetNames(typeof(Axis)).ToList(),0);
            axisSelectorField.name = "Axis";
            axisSelectorField.tooltip = $"Axis will be used as reference";
            axisSelectorField.value = m_ResolverAxis.ToString();
            axisSelectorField.RegisterValueChangedCallback((o) => m_ResolverAxis = Enum.Parse<Axis>(o.newValue));
            root.Add(axisSelectorField);

            Vector3Field distanceFromAxisField = new("Distance from Axis");
            distanceFromAxisField.name = "DistanceFromAxis";
            axisSelectorField.tooltip = $"Displacement of the axis";
            distanceFromAxisField.value = m_DistanceFromAxis;
            distanceFromAxisField.RegisterValueChangedCallback((o)=> m_DistanceFromAxis = o.newValue);
            root.Add(distanceFromAxisField);

            return root;
        }
        public override void DrawHandles(SceneView sceneView)
        {
            base.DrawHandles(sceneView);

            var screeBottomLeftPosition = sceneView.camera.ViewportToWorldPoint(new Vector3(0f, 0f, sceneView.camera.farClipPlane));
            var screenUpperRightPosition = sceneView.camera.ViewportToWorldPoint(new Vector3(1f, 1f, sceneView.camera.farClipPlane));
            var screenCenterPosition = sceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, sceneView.camera.nearClipPlane));
            var axisHanleDistance = Vector3.Distance(screeBottomLeftPosition, screenUpperRightPosition);

            Vector3 axisStartPosition = Vector3.zero;
            Vector3 axisEndPosition = Vector3.zero;
            Vector3 axisCenterOnScreen = Vector3.zero;
            Vector3 normal = Vector3.zero;
            float extremeValue = axisHanleDistance;
            switch (m_ResolverAxis)
            {
                default:
                case Axis.X:
                    axisStartPosition = new Vector3(extremeValue, m_DistanceFromAxis.y, m_DistanceFromAxis.z);
                    axisEndPosition = new Vector3(-extremeValue, m_DistanceFromAxis.y, m_DistanceFromAxis.z);
                    Handles.color = Color.red;
                    normal = Vector3.right;
                    axisCenterOnScreen = new Vector3(axisCenterOnScreen.x, m_DistanceFromAxis.y, m_DistanceFromAxis.z);
                    axisCenterOnScreen = Vector3.Cross(normal, axisCenterOnScreen);
                    break;
                case Axis.Y:
                    axisStartPosition = new Vector3(m_DistanceFromAxis.x, extremeValue, m_DistanceFromAxis.z);
                    axisEndPosition = new Vector3(m_DistanceFromAxis.x, -extremeValue, m_DistanceFromAxis.z);
                    Handles.color = Color.green;
                    normal = Vector3.up;
                    break;
                case Axis.Z:
                    axisStartPosition = new Vector3(m_DistanceFromAxis.x, m_DistanceFromAxis.y, extremeValue);
                    axisEndPosition = new Vector3(m_DistanceFromAxis.x, m_DistanceFromAxis.y, -extremeValue);
                    Handles.color = Color.blue;
                    normal = Vector3.forward;
                    break;
            }

            Handles.DrawLine(axisStartPosition, axisEndPosition, HandleUtility.GetHandleSize(screenCenterPosition) / 100f);
        }
    }
}
