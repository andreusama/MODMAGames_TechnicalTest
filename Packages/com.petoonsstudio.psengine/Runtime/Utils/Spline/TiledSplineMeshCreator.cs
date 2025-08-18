#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Splines;
using Unity.Mathematics;
using UnityEditor;
#endif
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [ExecuteInEditMode]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class TiledSplineMeshCreator : MonoBehaviour
    {
#if UNITY_EDITOR
        [System.Serializable]
        public class Model
        {
            [Tooltip("Mesh to bend along the spline.")]
            public Mesh Mesh;
            [Tooltip("Materials to apply on the bent mesh.")]
            public Material[] Materials;

            [Header("Transform")]
            [Tooltip("Translation to apply on the mesh before bending it.")]
            public Vector3 Translation;
            [Tooltip("Rotation to apply on the mesh before bending it.")]
            public Vector3 Rotation;
            [Tooltip("Scale to apply on the mesh before bending it.")]
            public Vector3 Scale = Vector3.one;
        }

        [SerializeField] private SplineContainer m_SplineContainer;

        [SerializeField, HideInInspector]
        private bool m_AutoUpdate = false;
        [SerializeField, HideInInspector]
        private bool m_Subscribed = false;

        [Header("Rendering")]
        [Range(0, 1)]
        [SerializeField]
        private float m_StartOffsetPercentage = 0f;
        [SerializeField]
        private uint m_MaximumMeshesPerSegment = 5;

        [Header("Models")]
        [SerializeField, Tooltip("Randomize model used for each segment.")]
        bool m_RandomModels = false;
        [SerializeField, Tooltip("Number of segments to change the model")]
        private int m_NumOfMeshesToSwapModel = 5;
        [SerializeField]
        private Model[] m_Models;
        [Tooltip("The mode to use to fill the choosen interval with the bent mesh."), SerializeField]
        private SplineMeshBender.FillingMode m_Mode = SplineMeshBender.FillingMode.StretchToInterval;

        [Header("Physics")]
        [Tooltip("Physic material to apply on the bent mesh."), SerializeField]
        private PhysicsMaterial m_PhysicMaterial;
        [Tooltip("If true, a mesh collider will be generated."), SerializeField]
        private bool m_GenerateCollider = false;


        [SerializeField] private GameObject m_MeshHolder;

        [Header("Gizmos")]
        [Tooltip("If true, segments length will be drawn in viewer using Gizmos."), SerializeField]
        private bool m_DrawSegments = false;
        [Tooltip("If true, segments length will be drawn in viewer using Gizmos."), SerializeField]
        private float m_PointSize = 0.25f;
        [SerializeField]
        private Color m_GizmosColor = Color.white;

        private bool m_RecreateMeshes;
        private float m_StartOffsetWorldLength;
        private int m_CurrentMeshIndex = 0;
        private int m_MeshesSpawned = 0;
        private List<GameObject> m_SplineBenderSegments = new List<GameObject>();

        private Model RandomModel => m_Models.RandomItem();

        private void OnEnable()
        {
            if (m_SplineContainer == null && !TryGetComponent(out m_SplineContainer))
                m_SplineContainer = GetComponentInParent<SplineContainer>();

            if (m_SplineContainer == null)
            {
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            if (m_RecreateMeshes)
            {
                m_RecreateMeshes = false;
                CreateMeshes();
            }
        }

        [ContextMenu("Rebuild Mesh")]
        public void CreateMeshes()
        {
            // we don't update if we are in prefab mode
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null) return;

            float minX = float.MaxValue;
            float maxX = float.MinValue;

            if (m_Models[0] == null)
                Debug.LogError("There is no mesh asigned!");

            foreach (var vert in m_Models[0].Mesh.vertices)
            {
                Vector3 p = vert;
                maxX = Math.Max(maxX, p.x);
                minX = Math.Min(minX, p.x);
            }
            float sourceMeshScaledLength = Math.Abs(maxX - minX) * m_Models[0].Scale.x;

            //Offset Total length relative
            m_StartOffsetWorldLength = m_SplineContainer.Spline.GetLength() * m_StartOffsetPercentage;

            //Adjust offset if mesh will be placed outside of the path
            if (m_SplineContainer.Spline.GetLength() - m_StartOffsetWorldLength < sourceMeshScaledLength)
                m_StartOffsetWorldLength = m_SplineContainer.Spline.GetLength() - sourceMeshScaledLength;

            m_SplineBenderSegments.Clear();
            if (m_Mode == SplineMeshBender.FillingMode.Repeat)
            {
                GenerateSegments(sourceMeshScaledLength, m_SplineBenderSegments);
            }
            else
            {
                var initialPosition = m_SplineContainer.EvaluatePosition(0 + m_StartOffsetPercentage / 10);
                m_SplineBenderSegments.Add(GetSegment("Segment 0 mesh", initialPosition, m_SplineContainer.Spline[m_SplineContainer.Spline.Knots.Count() - 1].Position));
            }

            // we destroy the unused objects. This is classic pooling to recycle game objects.
            var objectsToDestroy = m_MeshHolder.transform
                .Cast<Transform>()
                .Select(child => child.gameObject).Except(m_SplineBenderSegments);

            var startIndex = objectsToDestroy.Count() - 1;
            for (int i = startIndex; i >= 0; i--)
            {
                DestroyImmediate(objectsToDestroy.ElementAt(i));
            }
        }

        private void GenerateSegments(float originalMeshFinalLength, List<GameObject> used)
        {
            float segmentLength = (m_MaximumMeshesPerSegment) * originalMeshFinalLength;
            int numberOfSegments = Mathf.CeilToInt((m_SplineContainer.Spline.GetLength() - m_StartOffsetWorldLength) / segmentLength);

            for (int i = 0; i < numberOfSegments; i++)
            {
                Vector3 StartPoint, EndPoint;
                var segmentStartDistance = MMMaths.Remap((i * segmentLength) + m_StartOffsetWorldLength, 0 , m_SplineContainer.Spline.GetLength(), 0, 1);
                StartPoint = m_SplineContainer.EvaluatePosition(segmentStartDistance);

                float segmentEndDistance;
                uint? numberOfMeshesPerSegment = null;

                if (i == numberOfSegments - 1)
                {
                    segmentEndDistance = m_SplineContainer.Spline.GetLength();
                    numberOfMeshesPerSegment = (Mathf.Approximately(m_StartOffsetWorldLength, m_SplineContainer.Spline.GetLength() - originalMeshFinalLength)) ? 1 : null;
                    if(!numberOfMeshesPerSegment.HasValue)
                    {
                        continue;
                    }
                }
                else
                {
                    segmentEndDistance = i * segmentLength + segmentLength;
                    numberOfMeshesPerSegment = m_MaximumMeshesPerSegment;
                }
                //Get the position given the 
                var segmentSplineEndDistance = MMMaths.Remap(segmentEndDistance, 0, m_SplineContainer.Spline.GetLength(), 0, 1);
                EndPoint = m_SplineContainer.EvaluatePosition(segmentSplineEndDistance);

                used.Add(GetSegment("Segment_" + i + "_mesh", StartPoint, EndPoint, numberOfMeshesPerSegment));
            }
        }

        private GameObject GetSegment(string name, Vector3 startPoint, Vector3 endPoint, uint? repetitionCount = null)
        {
            startPoint = m_SplineContainer.transform.InverseTransformPoint(startPoint);
            endPoint = m_SplineContainer.transform.InverseTransformPoint(endPoint);
            SplineUtility.GetNearestPoint(m_SplineContainer.Spline, startPoint, out float3 nearestStartPoint, out Single startDist, resolution:6, SplineUtility.PickResolutionMax);
            SplineUtility.GetNearestPoint(m_SplineContainer.Spline, endPoint, out float3 nearestEndPoint, out Single endDist, resolution: 6, SplineUtility.PickResolutionMax);

            var go = FindOrCreate(name);
            go.GetComponent<SplineMeshBender>().SetInterval(m_SplineContainer, startDist * m_SplineContainer.Spline.GetLength(), endDist * m_SplineContainer.Spline.GetLength(), repetitionCount);
            go.GetComponent<MeshCollider>().enabled = m_GenerateCollider;
            return go;
        }

        private GameObject FindOrCreate(string name)
        {
            var childTransform = m_MeshHolder.transform.Find(name);
            GameObject res;
            if (childTransform == null)
            {
                res = SplineMeshUtils.CreateGameObject(name,
                    m_MeshHolder,
                    typeof(MeshFilter),
                    typeof(MeshRenderer),
                    typeof(SplineMeshBender),
                    typeof(MeshCollider));
                res.isStatic = true;
            }
            else
            {
                res = childTransform.gameObject;
            }

            Model modelSpawned;
            if (m_RandomModels)
                modelSpawned = RandomModel;
            else
                modelSpawned = m_Models[m_CurrentMeshIndex];

            MeshRenderer modelMeshRenderer = res.GetComponent<MeshRenderer>();
            modelMeshRenderer.materials = modelSpawned.Materials;
            res.GetComponent<MeshCollider>().material = m_PhysicMaterial;

            SplineMeshBender mb = res.GetComponent<SplineMeshBender>();

            mb.Source = BentMeshData.Build(modelSpawned.Mesh)
                .Translate(modelSpawned.Translation)
                .Rotate(Quaternion.Euler(modelSpawned.Rotation))
                .Scale(modelSpawned.Scale);

            mb.Mode = m_Mode;

            if (m_MeshesSpawned >= m_NumOfMeshesToSwapModel)
            {
                m_MeshesSpawned = 0;
                m_CurrentMeshIndex++;
                if (m_CurrentMeshIndex >= m_Models.Length)
                    m_CurrentMeshIndex = 0;
            }
            m_MeshesSpawned++;

            return res;
        }

        public void SetDirty(Spline spline)
        {
            if (m_SplineContainer != null && m_SplineContainer.Spline == spline)
            {
                CreateMeshes();
            }
        }

        [MenuItem("GameObject/Level Design/Tiled Meshes along Path", false, 10)]
        private static void CreatePathCreator(MenuCommand menuCommand)
        {
            GameObject selectedGO = menuCommand.context as GameObject;

            GameObject splineGO;
            SplineContainer splineContainer;

            if (selectedGO != null && selectedGO.TryGetComponent(out splineContainer))
            {
                // Selected already has path
                splineGO = selectedGO;
            }
            else
            {
                // Create a custom game object
                splineGO = new GameObject("Path");

                // Ensure it gets reparented if this was a context click (otherwise does nothing)
                GameObjectUtility.SetParentAndAlign(splineGO, menuCommand.context as GameObject);

                // Add Path Creator component
                splineGO.AddComponent<SplineContainer>();
            }

            GameObject meshCreatorGO = new GameObject("Tiled Meshes");
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(meshCreatorGO, splineGO);

            meshCreatorGO.AddComponent<TiledSplineMeshCreator>();

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(splineGO, "Create " + splineGO.name);
            Undo.RegisterCreatedObjectUndo(meshCreatorGO, "Create " + meshCreatorGO.name);

            Selection.activeObject = splineGO;
        }

        public void InitializeScript(SplineContainer spline, float startOffsetPercentage, uint maxMeshesPerSegment, Model[] models,
            SplineMeshBender.FillingMode mode, PhysicsMaterial physicMaterial, bool generateCollider)
        {
            m_SplineContainer = spline;
            m_StartOffsetPercentage = startOffsetPercentage;
            m_MaximumMeshesPerSegment = maxMeshesPerSegment;
            m_Models = models;
            m_Mode = mode;
            m_PhysicMaterial = physicMaterial;
            m_GenerateCollider = generateCollider;
        }

        public void SetMeshHolder(GameObject meshHolder)
        {
            m_MeshHolder = meshHolder;
        }

        private void OnDrawGizmosSelected()
        {
            if (m_DrawSegments)
            {
                for (int i = 0; i < m_SplineBenderSegments.Count; i++)
                {
                    var segmentBender = m_SplineBenderSegments[i].GetComponent<SplineMeshBender>();
                    var startPoint = m_SplineContainer.EvaluatePosition(MMMaths.Remap(segmentBender.IntervalStart, 0, m_SplineContainer.Spline.GetLength(), 0, 1));
                    var endPoint = m_SplineContainer.EvaluatePosition(MMMaths.Remap(segmentBender.IntervalEnd, 0, m_SplineContainer.Spline.GetLength(), 0, 1));
                    var currentColor = Gizmos.color;
                    
                    Gizmos.color = m_GizmosColor;
                    Gizmos.DrawLine(startPoint, endPoint);
                    Gizmos.color = currentColor;

                    Gizmos.color = m_GizmosColor;
                    MMDebug.DrawGizmoPoint(startPoint, m_PointSize, m_GizmosColor);
                    Gizmos.color = currentColor;

                    if (i == m_SplineBenderSegments.Count - 1)
                    {
                        Gizmos.color = m_GizmosColor;
                        MMDebug.DrawGizmoPoint(endPoint, m_PointSize, m_GizmosColor);
                        Gizmos.color = currentColor;
                    }
                }
            }
        }
#endif
    }
}
