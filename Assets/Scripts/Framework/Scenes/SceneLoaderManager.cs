using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Collections.Generic;
using EnGUI;
using Utils;

namespace Game.Framework
{
    public struct SceneLoadingEvent
    {
        public enum EventType { LOADING_START, LOADING_END }
        public EventType Type;
        public string SceneName;

        public SceneLoadingEvent(EventType type, string sceneName) { Type = type; SceneName = sceneName; }
    }

    /// <summary>
    /// @Author: Alejandro Cortes Cabrejas
    /// </summary>
    public class SceneLoaderManager : PersistentSingleton<SceneLoaderManager>
    {
        [SerializeField] protected AssetReference m_LoadingScene;
        [SerializeField] protected float m_FadeDuration = 0.25f;

        protected List<SceneGroup> m_CurrentSceneGroups;
        protected List<SceneGroup> m_CurrentSceneGroupsPersistent;

        protected AsyncOperationHandle<SceneInstance> m_LoadingSceneOperation;

        public delegate void SceneGroupLoad(SceneGroup loadedSceneGroup);
        public virtual event SceneGroupLoad OnSceneGroupLoad;

        public List<SceneGroup> CurrentSceneGroups => m_CurrentSceneGroups;
        public List<SceneGroup> CurrentSceneGroupsPersistent => m_CurrentSceneGroupsPersistent;

        public bool IsLoading { get; private set; }

        public bool IsSceneGroupLoaded(SceneGroup group)
        {
            if (group.Persistent)
                return m_CurrentSceneGroupsPersistent.Contains(group);
            else
                return m_CurrentSceneGroups.Contains(group);
        }

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this) return;

            m_CurrentSceneGroups = new();
            m_CurrentSceneGroupsPersistent = new();
        }

        void OnEnable()
        {
            LightProbes.needsRetetrahedralization += OnTetrahedralizationNeeded;
        }

        void OnDisable()
        {
            LightProbes.needsRetetrahedralization -= OnTetrahedralizationNeeded;
        }

        private void OnTetrahedralizationNeeded()
        {
            LightProbes.TetrahedralizeAsync();
        }

        public void LoadSceneGroup(SceneGroup sceneGroup, bool additive = false, bool loadingScene = true, bool releaseMemory = true)
        {
            LoadSceneGroup<IFader>(sceneGroup, additive, loadingScene, releaseMemory);
        }

        public void LoadSceneGroup<T>(SceneGroup sceneGroup, bool additive = false, bool loadingScene = true, bool releaseMemory = true) where T : IFader
        {
            StartCoroutine(LoadSceneGroup_Internal<T>(sceneGroup, additive, loadingScene, releaseMemory));
        }

        public void LoadSceneGroup(AssetReferenceT<SceneGroup> sceneReference, bool additive = false, bool loadingScene = true, bool releaseMemory = true)
        {
            LoadSceneGroup<IFader>(sceneReference, additive, loadingScene, releaseMemory);
        }

        public void LoadSceneGroup<T>(AssetReferenceT<SceneGroup> sceneReference, bool additive = false, bool loadingScene = true, bool releaseMemory = true) where T : IFader
        {
            StartCoroutine(LoadSceneGroup_Internal<T>(sceneReference, additive, loadingScene, releaseMemory));
        }

        public void ReleasePersistentSceneGroups()
        {
            StartCoroutine(ReleasePersistentSceneGroups_Internal());
        }

        protected virtual IEnumerator LoadSceneGroup_Internal<T>(AssetReferenceT<SceneGroup> sceneReference, bool additive = false,
            bool loadingScene = true, bool releaseMemory = true) where T : IFader
        {
            if (IsLoading)
                yield return new WaitUntil(() => !IsLoading);

            IsLoading = true;

            var op = Addressables.LoadAssetAsync<SceneGroup>(sceneReference);
            yield return op;
            yield return StartCoroutine(LoadSceneGroup_Internal<T>(op.Result, additive, loadingScene, releaseMemory));
            Addressables.Release(op);

            IsLoading = false;
        }

        protected virtual IEnumerator LoadSceneGroup_Internal<T>(SceneGroup sceneGroup, bool additive, bool loadingScene, bool releaseMemory) where T : IFader
        {
            Type t = typeof(T);
            bool fade = !t.IsInterface;

            if (fade)
                yield return EnGUIManager.Instance.FadeOut<T>(m_FadeDuration);

            if (additive)
            {
                if (loadingScene)
                {
                    yield return LoadLoadingScene(LoadSceneMode.Additive);

                    if (fade)
                        yield return EnGUIManager.Instance.FadeIn<T>(m_FadeDuration);

                    yield return sceneGroup.Load(LoadSceneMode.Additive);
                }
                else
                {
                    yield return sceneGroup.Load(LoadSceneMode.Additive);

                    yield return EnGUIManager.Instance.FadeIn<T>(m_FadeDuration);
                }

                if (sceneGroup.Persistent)
                    m_CurrentSceneGroupsPersistent.Add(sceneGroup);
                else
                    m_CurrentSceneGroups.Add(sceneGroup);

                if (releaseMemory)
                    ReleaseResources();

                if (loadingScene)
                    yield return UnloadLoadingScene();
            }
            else
            {
                if (loadingScene)
                {
                    yield return LoadLoadingScene(LoadSceneMode.Additive);

                    if (fade)
                        yield return EnGUIManager.Instance.FadeIn<T>(m_FadeDuration);

                    yield return ReleasePreviousScenes();
                    yield return ReleaseEditorScenes();

                    yield return sceneGroup.Load(LoadSceneMode.Additive);
                }
                else
                {
                    yield return sceneGroup.Load(LoadSceneMode.Single);

                    yield return ReleasePreviousScenes();

                    if (fade)
                        yield return EnGUIManager.Instance.FadeIn<T>(m_FadeDuration);
                }

                if (sceneGroup.Persistent)
                    m_CurrentSceneGroupsPersistent.Add(sceneGroup);
                else
                    m_CurrentSceneGroups.Add(sceneGroup);

                if (releaseMemory)
                    ReleaseResources();

                if (loadingScene)
                {
                    if (fade)
                        yield return EnGUIManager.Instance.FadeOut<T>(1f);

                    yield return UnloadLoadingScene();

                    if (fade)
                        yield return EnGUIManager.Instance.FadeIn<T>(1f);
                }
            }

            OnSceneGroupLoad?.Invoke(sceneGroup);
        }

        private IEnumerator ReleaseEditorScenes()
        {
            List<Scene> scenesToUnload = new List<Scene>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (ShouldReleaseEditorScene(scene))
                    scenesToUnload.Add(scene);
            }

            foreach (Scene scene in scenesToUnload)
            {
                yield return SceneManager.UnloadSceneAsync(scene);
            }
        }

        private bool ShouldReleaseEditorScene(Scene scene)
        {
            foreach (var group in m_CurrentSceneGroupsPersistent)
            {
                if (group.HasLoadedScene(scene))
                    return false;
            }

            if (m_LoadingSceneOperation.IsDone && m_LoadingSceneOperation.Result.Scene == scene)
                return false;

            return true;
        }

        public static bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == sceneName)
                {
                    return SceneManager.GetSceneAt(i).isLoaded;
                }
            }

            return false;
        }

        protected void ReleaseResources()
        {
            Resources.UnloadUnusedAssets();
        }

        protected IEnumerator ReleasePreviousScenes()
        {
            foreach (var item in m_CurrentSceneGroups)
            {
                yield return item.Unload();
            }

            m_CurrentSceneGroups.Clear();
        }

        protected IEnumerator ReleasePersistentSceneGroups_Internal()
        {
            foreach (var item in m_CurrentSceneGroupsPersistent)
            {
                yield return item.Unload();
            }

            m_CurrentSceneGroupsPersistent.Clear();
        }

        #region Loading Scene

        public IEnumerator LoadLoadingScene(LoadSceneMode mode)
        {
            m_LoadingSceneOperation = Addressables.LoadSceneAsync(m_LoadingScene, mode);
            yield return m_LoadingSceneOperation;
        }

        public IEnumerator UnloadLoadingScene()
        {
            yield return Addressables.UnloadSceneAsync(m_LoadingSceneOperation);
        }

        #endregion

        #region Unload Scene

        public IEnumerator UnloadScene(string assetGUID)
        {
            foreach (var scene in m_CurrentSceneGroups)
                yield return scene.Unload(assetGUID);
        }

        #endregion
    }
}