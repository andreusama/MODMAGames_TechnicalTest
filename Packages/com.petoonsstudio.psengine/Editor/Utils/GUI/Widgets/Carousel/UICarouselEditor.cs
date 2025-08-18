using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using UnityEditor.SceneManagement;

namespace PetoonsStudio.PSEngine.Utils
{
    [CustomEditor(typeof(UICarousel)), CanEditMultipleObjects]
    public class UICarouselEditor : ScrollRectEditor
    {
        private SerializedProperty m_ElementPrefab;
        private SerializedProperty m_ScrollMask;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_ElementPrefab = serializedObject.FindProperty("ElementPrefab");
            m_ScrollMask = serializedObject.FindProperty("ScrollMask");
        }

        public override void OnInspectorGUI()
        {
            GUIStyle header = new GUIStyle(EditorStyles.boldLabel);
            header.fontSize = 14;

            UICarousel carousel = (UICarousel)target;

            GUILayout.Space(5);
            GUILayout.Label("Scroll Rect Behaviour", header);
            base.OnInspectorGUI();

            GUILayout.Label("Carousel", header);
            EditorGUILayout.PropertyField(m_ElementPrefab, new GUIContent("Element Prefab"));
            EditorGUILayout.PropertyField(m_ScrollMask, new GUIContent("Scroll Mask"));

            GUILayout.Space(5);
            GUILayout.Label("Animation", EditorStyles.boldLabel);
            carousel.ScrollSpeed = EditorGUILayout.FloatField("Scroll Speed", carousel.ScrollSpeed);
            carousel.ScrollEase = (Ease)EditorGUILayout.EnumPopup("Scroll Ease", carousel.ScrollEase);

            carousel.ScrollSpeed = Mathf.Max(carousel.ScrollSpeed, 0.01f);

            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("GameObject/UI/Carousel")]
        public static void CreateCarousel()
        {
            /// Create Carousel
            GameObject carouselGO = new GameObject("Carousel");
            GameObjectUtility.SetParentAndAlign(carouselGO, Selection.activeGameObject);

            if (!carouselGO.HasComponent<RectTransform>())
                ObjectFactory.AddComponent<RectTransform>(carouselGO);

            UICarousel carousel = ObjectFactory.AddComponent<UICarousel>(carouselGO);

            /// Create Viewport
            GameObject viewportGO = new GameObject("Viewport");
            GameObjectUtility.SetParentAndAlign(viewportGO, carouselGO);

            var viewportTransform = ObjectFactory.AddComponent<RectTransform>(viewportGO);
            viewportTransform.anchorMin = new Vector2(0f, 1f);
            viewportTransform.anchorMax = new Vector2(1f, 1f);
            viewportTransform.pivot = new Vector2(0.5f, 1f);
            viewportTransform.sizeDelta = Vector3.zero; // TODO

            var viewportImg = ObjectFactory.AddComponent<Image>(viewportGO);
            viewportImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd");
            ObjectFactory.AddComponent<Mask>(viewportGO).showMaskGraphic = false;

            /// Create Content
            GameObject contentGO = new GameObject("Content");
            GameObjectUtility.SetParentAndAlign(contentGO, viewportGO);

            var contentTransform = ObjectFactory.AddComponent<RectTransform>(contentGO);

            var contentHLG = ObjectFactory.AddComponent<HorizontalLayoutGroup>(contentGO);
            contentHLG.childControlWidth = false;
            contentHLG.childControlHeight = false;
            contentHLG.childForceExpandWidth = false;
            contentHLG.childForceExpandHeight = false;
            contentHLG.childScaleWidth = true;
            contentHLG.childScaleHeight = true;

            var contentSizeFitter = ObjectFactory.AddComponent<ContentSizeFitter>(contentGO);
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            carousel.content = contentTransform;
            carousel.viewport = viewportTransform;
            carousel.ScrollMask = viewportTransform;

            Undo.RegisterCreatedObjectUndo(carouselGO, "Created Carousel");
            Selection.activeObject = carouselGO;
        }

        protected override void OnDisable()
        {
            UICarousel carousel = (UICarousel)target;

            if (carousel)
                Undo.RecordObject(carousel, "UICarousel changed");

            base.OnDisable();
        }
    } 
}
