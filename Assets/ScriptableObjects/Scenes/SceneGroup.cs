using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Game.Framework
{
    [CreateAssetMenu(fileName = "New Addressable Scene Group", menuName = "Game/Framework/Addressable Scene Group")]
    public class SceneGroup : ScriptableObject
    {
        public AssetReference[] Scenes;
        public bool Sequence = true;
        public bool Persistent = false;

        private Dictionary<string, AsyncOperationHandle<SceneInstance>> m_Operations = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();

        public bool HasLoadedScene(Scene scene)
        {
            foreach (var key in m_Operations.Keys)
            {
                if (m_Operations[key].IsDone && m_Operations[key].Result.Scene == scene)
                    return true;
            }

            return false;
        }

        public IEnumerator Load(LoadSceneMode mode)
        {
            for (int i = 0; i < Scenes.Length; i++)
            {
                AsyncOperationHandle<SceneInstance> asyncOp;

                if (mode == LoadSceneMode.Single && i == 0)
                    asyncOp = Addressables.LoadSceneAsync(Scenes[i], LoadSceneMode.Single);
                else
                    asyncOp = Addressables.LoadSceneAsync(Scenes[i], LoadSceneMode.Additive);

                m_Operations.Add(Scenes[i].AssetGUID, asyncOp);

                if (Sequence)
                    yield return asyncOp;
            }

            if (!Sequence)
            {
                foreach (var asyncOp in m_Operations.Values)
                {
                    if (asyncOp.IsDone)
                        continue;
                    else
                        yield return new WaitUntil(() => asyncOp.IsDone);
                }
            }
        }

        public IEnumerator Unload()
        {
            foreach (var currentOperation in m_Operations.Values)
            {
                if (currentOperation.IsValid())
                {
                    yield return Addressables.UnloadSceneAsync(currentOperation);
                }
            }

            m_Operations.Clear();
        }

        public IEnumerator Unload(string assetGUID)
        {
            if (m_Operations.TryGetValue(assetGUID, out var asyncOp))
            {
                if (asyncOp.IsValid())
                {
                    yield return Addressables.UnloadSceneAsync(asyncOp);
                }
            }

            m_Operations.Remove(assetGUID);
        }
    }
}