using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PetoonsStudio.PSEngine.Tools
{
    public class ObjectReplacerToolWindow : EditorWindow
    {
        protected const int DEFAULT_RESOLVER = 0;

        protected List<Type> m_Resolvers = new();
        [SerializeField]
        protected List<string> m_ResolversNames = new();
        [SerializeField, SerializeReference]
        protected ObjectReplacerResolver m_CurrentResolver;

        [SerializeField]
        protected int m_CurrentResolverIndex = DEFAULT_RESOLVER;

        [SerializeField]
        protected VisualElement m_ResolverUI;
        [SerializeField]
        protected VisualElement m_ReplacersUI;

        protected void Awake()
        {
            CheckResolvers();
        }

        [MenuItem("Window/Petoons Studio/PSEngine/Editor/Object Replacer Tool", priority = ToolsUtils.EDITOR_CATEGORY)]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            ObjectReplacerToolWindow window = (ObjectReplacerToolWindow)EditorWindow.GetWindow(typeof(ObjectReplacerToolWindow));
            window.titleContent = new GUIContent("Object Replacer Tool");
            window.minSize = new Vector2(250f, 150f);
            window.Show();
        }

        protected void OnFocus()
        {
            CheckResolvers();
        }

        protected void CreateGUI()
        {
            DrawResolverHeader();
            DrawResolverOptions();

            Space(rootVisualElement);

            DrawReplacersList();
            DrawReplaceButton();
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            Color lastColor = Handles.color;

            m_CurrentResolver.DrawHandles(sceneView);

            Handles.color = lastColor;
        }
        #region GUI
        protected void DrawResolverHeader()
        {
            Label titleField = new("RESOLVER");
            titleField.style.fontSize = 12;
            rootVisualElement.Add(titleField);

            HelpBox helpBox = new HelpBox("Select a Resolver", HelpBoxMessageType.Info);
            rootVisualElement.Add(helpBox);

            DropdownField resolverSelector = new DropdownField(m_ResolversNames, m_CurrentResolverIndex);
            resolverSelector.RegisterValueChangedCallback(changeEvent => UpdateResolver(changeEvent.newValue));
            rootVisualElement.Add(resolverSelector);
        }
        protected void DrawResolverOptions()
        {
            m_ResolverUI = new();
            m_ResolverUI.Add(m_CurrentResolver.DrawResolverUI());
            rootVisualElement.Add(m_ResolverUI);
        }
        protected void DrawReplacersList()
        {
            Label replacersTitle = new("REPLACERS");
            rootVisualElement.Add(replacersTitle);

            HelpBox helpBox = new HelpBox("Replacers are chosen depending on the distance the selected object is, starting from zero to the distance of the first object." +
                "\n If the distance is greater than the defined, the last one will be picked", HelpBoxMessageType.Info);
            rootVisualElement.Add(helpBox);

            m_ReplacersUI = new();
            m_ReplacersUI.Add(m_CurrentResolver.DrawReplacersUI());

            rootVisualElement.Add(m_ReplacersUI);
        }
        protected void DrawReplaceButton()
        {
            Button replaceButton = new Button(Replace);
            replaceButton.name = "Replace Button";
            replaceButton.text = "REPLACE";
            replaceButton.clicked += () => Replace();
            rootVisualElement.Add(replaceButton);
        }
        #endregion
        #region REPLACE LOGIC
        protected void ReplaceSelectedObjectWithPrefabs()
        {
            List<GameObject> newObjects = new List<GameObject>();
            Queue<GameObject> oldObjects = new Queue<GameObject>(Selection.gameObjects);

            int undoID = Undo.GetCurrentGroup();

            while (oldObjects.Count > 0)
            {
                var obj = oldObjects.Dequeue();
                var newObj = ReplaceObjectWithPrefab(obj);
                newObjects.Add(newObj);
                Undo.RegisterCreatedObjectUndo(newObj, $"Replaced old object with {newObj.name}");
            }

            Undo.CollapseUndoOperations(undoID);

            Selection.objects = newObjects.ToArray();
        }
        protected GameObject ReplaceObjectWithPrefab(GameObject oldObject)
        {
            int hierarchyIndex = oldObject.transform.GetSiblingIndex();

            GameObject newObject = PrefabUtility.InstantiatePrefab(ResolvePrefab(oldObject), oldObject.transform.parent) as GameObject;
            newObject.transform.position = oldObject.transform.position;
            newObject.transform.localRotation = oldObject.transform.localRotation;
            newObject.transform.localScale = oldObject.transform.localScale;

            Undo.DestroyObjectImmediate(oldObject);
            newObject.transform.SetSiblingIndex(hierarchyIndex);

            return newObject;
        }
        protected UnityEngine.Object ResolvePrefab(GameObject currentObject)
        {
            ObjectReplacerResolverData replacer = m_CurrentResolver.ResolveReplacement(currentObject);

            if (replacer == null)
            {
                replacer = m_CurrentResolver.Data[m_CurrentResolver.Data.Count-1];
                Debug.LogWarning($"Object {currentObject.name} it's outside the define limits, will be replaced with {replacer.Replacement.name}");
            }

            return replacer.Replacement;
        }
        protected void Replace()
        {
            ReplaceSelectedObjectWithPrefabs();
        }
        #endregion
        #region RESOLVER
        protected void UpdateResolver(string newResolverName)
        {
            m_CurrentResolver = null;
            Type newResolverType = m_Resolvers.Find(x => x.Name == newResolverName);
            m_CurrentResolverIndex = m_Resolvers.IndexOf(newResolverType);
            m_CurrentResolver = Activator.CreateInstance(newResolverType) as ObjectReplacerResolver;

            m_ResolverUI.Clear();
            m_ResolverUI.Add(m_CurrentResolver.DrawResolverUI());

            m_ReplacersUI.Clear();
            m_ReplacersUI.Add(m_CurrentResolver.DrawReplacersUI());
        }

        protected void GetResolvers()
        {
            Type selectorType = typeof(ObjectReplacerResolver);

            if (selectorType == null)
                return;

            var typeArray = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && selectorType.IsAssignableFrom(x) && !x.IsAbstract).ToList();

            m_Resolvers = typeArray.ToList();

            m_ResolversNames.Clear();
            foreach (var resolver in m_Resolvers)
            {
                m_ResolversNames.Add(resolver.Name);
            }
        }
        protected void CheckResolvers()
        {
            GetResolvers();
            if ((m_CurrentResolver == null && m_Resolvers.Count > 0) || !m_ResolversNames.Contains(m_CurrentResolver.GetType().Name))
                m_CurrentResolver = Activator.CreateInstance(m_Resolvers[DEFAULT_RESOLVER]) as ObjectReplacerResolver;
        }
        #endregion
        #region VISUAL ELEMENTS HELPERS
        protected void Space(VisualElement root)
        {
            Label spaceField = new();
            spaceField.visible = false;
            root.Add(spaceField);
        }
        #endregion
    }
}
