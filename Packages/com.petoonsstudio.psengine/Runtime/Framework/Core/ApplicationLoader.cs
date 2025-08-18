using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PetoonsStudio.PSEngine.Framework
{
    public class ApplicationLoader : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject m_InputManager;
        [SerializeField] private AssetReferenceGameObject m_GameManager;
        [SerializeField] private AssetReferenceGameObject m_SoundManager;
        [SerializeField] private AssetReferenceGameObject m_GUIManager;
        [SerializeField] private AssetReferenceGameObject m_DebugManager;

        [Header("Scene")]
        [SerializeField] private AssetReferenceT<SceneGroup> m_LoadScene;

        private List<AsyncOperationHandle<GameObject>> m_OperationList;

        void Awake()
        {
            m_OperationList = new List<AsyncOperationHandle<GameObject>>();
        }

        void OnEnable()
        {
            StartCoroutine(Initialize());
        }

        void OnDisable()
        {
            StopAllCoroutines();

            foreach (var operation in m_OperationList)
                operation.Completed -= OnOperationComplete;

            m_OperationList.Clear();
        }

        private IEnumerator Initialize()
        {
            yield return new WaitUntil(() => PlatformManager.Initialized);

            yield return Addressables.InstantiateAsync(m_InputManager);
            yield return Addressables.InstantiateAsync(m_GameManager);

            InstantateReference(m_SoundManager);
            InstantateReference(m_GUIManager);
            InstantateReference(m_DebugManager);
        }

        private void InstantateReference(AssetReference assetRef)
        {
            if (assetRef.RuntimeKeyIsValid())
            {
                var assetOp = Addressables.InstantiateAsync(assetRef);

                assetOp.Completed += OnOperationComplete;
                m_OperationList.Add(assetOp);
            }
        }

        private void OnOperationComplete(AsyncOperationHandle<GameObject> obj)
        {
            foreach (var element in m_OperationList)
            {
                if (!element.IsDone)
                    return;
            }

            OnLoaderComplete();
        }

        protected virtual void OnLoaderComplete()
        {
            SceneLoaderManager.Instance.LoadSceneGroup(m_LoadScene, loadingScene: false);
        }
    }
}

