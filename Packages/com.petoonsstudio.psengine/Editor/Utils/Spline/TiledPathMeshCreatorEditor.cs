using UnityEditor;
using UnityEditor.Splines;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [CustomEditor(typeof(TiledSplineMeshCreator))]
    public class TiledPathMeshCreatorEditor : Editor
    {
        private const string AUTO_UPDATE_PROPERTY_NAME = "m_AutoUpdate";
        private SerializedProperty m_AutoUpdateProperty;
        private const string SUBSCRIBED_PROPERTY_NAME = "m_Subscribed";
        private SerializedProperty m_SubscribedProperty;

        private TiledSplineMeshCreator m_TiledSplineMeshCreator;

        private void OnEnable()
        {
            m_TiledSplineMeshCreator = (TiledSplineMeshCreator)target;
            FindProperties();

            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }


        public override void OnInspectorGUI()
        {
            var creator = (TiledSplineMeshCreator)target;

            serializedObject.Update();

            base.OnInspectorGUI();

            GUILayout.Space(20);

            var currentAutoUpdateValue = m_AutoUpdateProperty.boolValue;
            EditorGUILayout.PropertyField(m_AutoUpdateProperty);


            if (GUILayout.Button("Update"))
            {
                creator.CreateMeshes();
                Subscribe();
            }

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                if(currentAutoUpdateValue != m_AutoUpdateProperty.boolValue)
                {
                    if (m_AutoUpdateProperty.boolValue)
                        Subscribe();
                    else
                        Unsubscribe();
                }
            }
        }

        private void Subscribe()
        {
            if (m_AutoUpdateProperty == null || m_SubscribedProperty == null)
                FindProperties();

            if (m_AutoUpdateProperty.boolValue && !m_SubscribedProperty.boolValue)
            {
                m_SubscribedProperty.boolValue = true;
                EditorSplineUtility.AfterSplineWasModified += m_TiledSplineMeshCreator.SetDirty;
            }
        }

        private void Unsubscribe()
        {
            if (m_AutoUpdateProperty == null || m_SubscribedProperty == null)
                FindProperties();

            m_SubscribedProperty.boolValue = false;
            EditorSplineUtility.AfterSplineWasModified -= m_TiledSplineMeshCreator.SetDirty;
        }

        private void FindProperties()
        {
            m_AutoUpdateProperty = serializedObject.FindProperty(AUTO_UPDATE_PROPERTY_NAME);
            m_SubscribedProperty = serializedObject.FindProperty(SUBSCRIBED_PROPERTY_NAME);
        }
    }
}
