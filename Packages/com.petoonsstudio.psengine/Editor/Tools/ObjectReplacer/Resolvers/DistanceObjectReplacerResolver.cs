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
    public abstract class DistanceObjectReplacerResolver : ObjectReplacerResolver
    {
        public override IList<ObjectReplacerResolverData> Data { get => m_Data.ToList<ObjectReplacerResolverData>(); protected set => m_Data = (List<ObjectReplacerDistanceResolverData>)value; }
        [SerializeField]
        protected List<ObjectReplacerDistanceResolverData> m_Data = new();

        public override VisualElement DrawReplacersUI()
        {
            VisualElement root = new VisualElement();

            ListView replacersList = new ListView();
            replacersList.itemsAdded += ReplacerItemAdded;
            replacersList.makeItem = MakeReplacerItem;
            replacersList.bindItem = BindReplacerItem;
            replacersList.itemsSource = m_Data;
            replacersList.showAddRemoveFooter = true;
            replacersList.reorderable = true;
            replacersList.reorderMode = ListViewReorderMode.Animated;
            replacersList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            replacersList.showBoundCollectionSize = true;
            root.Add(replacersList);

            return root;
        }

        public override void BindReplacerItem(VisualElement v, int i)
        {
            base.BindReplacerItem(v, i);
            FloatField distanceField = v.Q<FloatField>(ObjectReplacerDistanceResolverData.DISTANCE_NAME);
            distanceField.value = m_Data[i].Distance;
            distanceField.RegisterValueChangedCallback((o) => m_Data[i].Distance = o.newValue);
        }

        public override VisualElement MakeReplacerItem()
        {
            VisualElement root = base.MakeReplacerItem();

            FloatField distanceField = new(ObjectReplacerDistanceResolverData.DISTANCE_NAME);
            distanceField.name = ObjectReplacerDistanceResolverData.DISTANCE_NAME;
            root.Add(distanceField);

            return root;
        }

        public override void ReplacerItemAdded(IEnumerable<int> indices)
        {
            foreach (var index in indices)
            {
                var x = new ObjectReplacerDistanceResolverData();
                m_Data[index] = x;
            }
        }

        public abstract Vector3 GetNearestPosition(Transform transform);

        public override ObjectReplacerResolverData ResolveReplacement(GameObject gameObject)
        {
            var nearestPosition = GetNearestPosition(gameObject.transform);
            float distance = Vector3.Distance(gameObject.transform.position, nearestPosition);

            foreach (var replacer in m_Data)
            {
                if (replacer.Distance < distance) continue;
                return replacer;
            }

            return null;
        }

        public override void DrawHandles(SceneView sceneView)
        {
            base.DrawHandles(sceneView);       
        }

        public override void DrawSelectedHandle(SceneView sceneView, GameObject selectedGameObject)
        {
            base.DrawSelectedHandle(sceneView,selectedGameObject);
            GUIStyle style = new GUIStyle(GUIStyle.none);
            style.fontStyle = FontStyle.BoldAndItalic;
            style.normal.textColor = Color.yellow;

            base.DrawSelectedHandle(sceneView, selectedGameObject);
            var nearestPosition = GetNearestPosition(selectedGameObject.transform);
            Handles.color = Color.white;
            Handles.DrawDottedLine(selectedGameObject.transform.position, nearestPosition, HandleUtility.GetHandleSize(selectedGameObject.transform.position));

            Handles.Label(selectedGameObject.transform.position, $"DISTANCE {Vector3.Distance(nearestPosition, selectedGameObject.transform.position)}", style);
        }
    }
}
