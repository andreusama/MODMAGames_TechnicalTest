using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtils
{
    /// <summary>
    /// Class used as entries for the <see cref="Database{TEntry}"/>
    /// </summary>
    public interface ITableEntry
    {
        string ID { get; set; }
    }

    /// <summary>
    /// Simple generic base class for implementing custom databases.
    /// </summary>
    /// <typeparam name="TEntry">Type of the database entries</typeparam>
    public abstract class Database<TDatabase, TEntry> : SingletonScriptableObject<TDatabase>
        where TDatabase : Database<TDatabase, TEntry>
        where TEntry : ITableEntry
    {
        [SerializeField] protected SerializedDictionary<string, TEntry> m_Entries = new SerializedDictionary<string, TEntry>();

        public ICollection<string> Keys => m_Entries.Keys;
        public ICollection<TEntry> Values => m_Entries.Values;

        public TEntry this[string key] => m_Entries[key];

        /// <summary>
        /// Add an entry to the database.
        /// </summary>
        /// <param name="key">The key of the entry</param>
        /// <param name="value">The value of the entry</param>
        /// <returns>Whether the entry was added or not</returns>
        public bool Add(string key, TEntry value)
        {
            if (m_Entries.ContainsKey(key)) return false;
            m_Entries[key] = value;
            m_Entries[key].ID = key;
            return true;
        }

        /// <summary>
        /// If an entry with the specified key exists, that entry is removed.
        /// </summary>
        /// <param name="key">The key of the entry to be removed</param>
        /// <returns>Whether the entry was removed or not</returns>
        public bool Remove(string key)
        {
            if (m_Entries.ContainsKey(key))
            {
                m_Entries.Remove(key);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Changes the key for a specific entry.
        /// </summary>
        /// <param name="key">The current key of the entry</param>
        /// <param name="newKey">The new key for the same entry</param>
        public void ChangeEntryKey(string key, string newKey)
        {
            if (!ContainsKey(key))
            {
                Debug.LogWarning($"Tried to replace key for entry: {key} with {newKey}, but {key} doesn't exist.");
                return;
            }
            var entry = m_Entries[key];
            m_Entries.Remove(key);
            m_Entries[newKey] = entry;
            m_Entries[newKey].ID = newKey;
        }

        /// <summary>
        /// Does the same as <see cref="ChangeEntryKey(string, string)"/> except that it
        /// appends a specified string at the end of the new key if an already existing
        /// entry has the same key, to avoid getting the entry removed because of
        /// conflicting keys.
        /// </summary>
        /// <param name="key">The current key of the entry</param>
        /// <param name="newKey">The new key for the same entry</param>
        /// <param name="appendIfFails">String appended to new key if a conflict appears</param>
        public void ChangeEntryKey(string key, string newKey, string appendIfFails)
        {
            while (m_Entries.ContainsKey(newKey))
            {
                newKey += appendIfFails;
            }
            ChangeEntryKey(key, newKey);
        }

        /// <summary>
        /// Checks if an entry with the specified key exists.
        /// </summary>
        /// <param name="key">The key we wish to check for</param>
        /// <returns>Whether an entry with that key exists or not</returns>
        public bool ContainsKey(string key)
        {
            return m_Entries.ContainsKey(key);
        }

        /// <summary>
        /// Returns the total number of entries contained in the database.
        /// </summary>
        /// <returns>Number of entries</returns>
        public int GetEntryCount()
        {
            return m_Entries.Count;
        }
    }
}