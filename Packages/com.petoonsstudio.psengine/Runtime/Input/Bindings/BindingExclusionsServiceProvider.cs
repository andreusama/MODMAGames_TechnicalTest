using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PetoonsStudio.PSEngine.Input
{
    [CreateAssetMenu(fileName = "BindingExclusionsServiceProvider", menuName = "Petoons Studio/Input/Binding Exclusions Service Provider")]
    public class BindingExclusionsServiceProvider : SingletonScriptableObject<BindingExclusionsServiceProvider>
    {
        [Header("Exclusions")]
        public AssetReferenceT<BindingList> PS4IconProvider;
        public AssetReferenceT<BindingList> PS5IconProvider;
        public AssetReferenceT<BindingList> XboxIconProvider;
        public AssetReferenceT<BindingList> PCIconProvider;
        public AssetReferenceT<BindingList> SwitchIconProvider;

        private BindingList m_CurrentBindingList;
        private AsyncOperationHandle<BindingList> m_CurrentOperation;

        void OnEnable()
        {
            LoadCurrentExclusions();

            m_CurrentOperation.Completed += OnBindingExclusionsLoaded;
        }

        void OnDisable()
        {
            m_CurrentOperation.Completed -= OnBindingExclusionsLoaded;

            Addressables.Release(m_CurrentOperation);
        }

        private void LoadCurrentExclusions()
        {
#if UNITY_SWITCH
            m_CurrentOperation = Addressables.LoadAssetAsync<BindingList>(SwitchIconProvider);
#elif UNITY_GAMECORE
            m_CurrentOperation = Addressables.LoadAssetAsync<BindingList>(XboxIconProvider);
#elif UNITY_PS4
            m_CurrentOperation = Addressables.LoadAssetAsync<BindingList>(PS4IconProvider);
#elif UNITY_PS5
            m_CurrentOperation = Addressables.LoadAssetAsync<BindingList>(PS5IconProvider);
#else
            m_CurrentOperation = Addressables.LoadAssetAsync<BindingList>(PCIconProvider);
#endif
        }

        private void OnBindingExclusionsLoaded(AsyncOperationHandle<BindingList> obj)
        {
            m_CurrentBindingList = obj.Result;
        }

        public bool IsBindingExclude(string path)
        {
            return m_CurrentBindingList.ExcludeList.Contains(path);
        }
    }
}