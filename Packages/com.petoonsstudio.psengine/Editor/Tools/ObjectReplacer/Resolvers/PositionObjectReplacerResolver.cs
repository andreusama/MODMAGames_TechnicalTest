using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PetoonsStudio.PSEngine.Utils
{
    public class PositionObjectReplacerResolver : DistanceObjectReplacerResolver
    {
        [SerializeField]
        protected Vector3 m_CentralPoint = Vector3.zero;

        public override VisualElement DrawResolverUI()
        {
            VisualElement root = new VisualElement();

            Vector3Field distanceFromAxisField = new("Central Point");
            distanceFromAxisField.name = "CentralPoint";
            distanceFromAxisField.tooltip = $"Position from where the distance will calculated";
            distanceFromAxisField.value = m_CentralPoint;
            distanceFromAxisField.RegisterValueChangedCallback((o) => m_CentralPoint = o.newValue);
            root.Add(distanceFromAxisField);

            return root;
        }

        public override Vector3 GetNearestPosition(Transform transform)
        {
            return m_CentralPoint;
        }

        public override void DrawHandles(SceneView sceneView)
        {
            base.DrawHandles(sceneView);

            GUIStyle style = new GUIStyle(GUIStyle.none);
            style.fontStyle = FontStyle.BoldAndItalic;
            style.normal.textColor = Color.yellow;

            var center = sceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, sceneView.camera.nearClipPlane));

            Color sColor = new Color(0, 1, 0, 0.15f);
            Color eColor = new Color(0, 1, 0, 0.05f);

            for (int i = 0; i < m_Data.Count; i++)
            {
                Handles.color = Color.Lerp(sColor, eColor, i + 1 / m_Data.Count);
                Handles.DrawSolidDisc(m_CentralPoint, center.direction.normalized, m_Data[i].Distance);
            }

            Handles.color = Color.white;
            Handles.DrawWireDisc(m_CentralPoint, center.direction.normalized, HandleUtility.GetHandleSize(m_CentralPoint) / 5f);
            Handles.Label(m_CentralPoint, "Central Point", style);
        }

        public override void DrawSelectedHandle(SceneView sceneView, GameObject selectedGameObject)
        {
            base.DrawSelectedHandle(sceneView, selectedGameObject);
            Handles.DrawDottedLine(m_CentralPoint, selectedGameObject.transform.position, HandleUtility.GetHandleSize(selectedGameObject.transform.position));
        }
    }
}
