using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class AssetReferenceExtensions
    {
        public static Task<T> LoadAssetAsyncAsATask<T>(this AssetReference assetReference)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            var asyncOperationHandle = assetReference.LoadAssetAsync<T>();
            asyncOperationHandle.Completed += handle =>
                                                {
                                                    taskCompletionSource.SetResult(handle.Result);
                                                };
            return Task.Run(() => taskCompletionSource.Task);
        }

        public static void LoadAssetAsyncWithCallback<T>(this AssetReference assetReference, Action<T> completed)
        {
            var asyncOperationHandle = assetReference.LoadAssetAsync<T>();
            asyncOperationHandle.Completed += handle => completed?.Invoke(handle.Result);
        }
    }
}