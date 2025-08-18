using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PetoonsStudio.PSEngine.Utils
{
    [System.Serializable]
    public class AddressableEntry<T> : ITableEntry where T : UnityEngine.Object
    {
        public string Key;
        public AssetReferenceT<T> Value;

        public AddressableEntry() { }

        public AddressableEntry(string key, AssetReference asset)
        {
            Key = key;
            Value = new AssetReferenceT<T>(asset.AssetGUID);
        }

        public AddressableEntry(AssetReference asset)
        {
            Value = new AssetReferenceT<T>(asset.AssetGUID);
        }

        public string ID { get => Key; set => Key = value; }
    }

    /// <summary>
    /// Simple generic base class for implementing custom databases.
    /// </summary>
    /// <typeparam name="TEntry">Type of the database entries</typeparam>
    public abstract class AddressablesDatabase<TDatabase, TAddressable> : Database<TDatabase, AddressableEntry<TAddressable>>
        where TDatabase : AddressablesDatabase<TDatabase, TAddressable>
        where TAddressable : UnityEngine.Object
    {
        public static TAddressable LoadAsset(string id)
        {
            return LoadAssetAsync(id).WaitForCompletion();
        }

        public static void ReleaseAsset(TAddressable asset)
        {
            Addressables.Release(asset);
        }

        public static AsyncOperationHandle<TAddressable> LoadAssetAsync(string id)
        {
            if (!Instance.ContainsKey(id))
                Debug.LogError($"Trying to load an asset that does not exist on the DB (id = {id})");

            var operation = Addressables.LoadAssetAsync<TAddressable>(Instance[id].Value);
            return operation;
        }
    }
}