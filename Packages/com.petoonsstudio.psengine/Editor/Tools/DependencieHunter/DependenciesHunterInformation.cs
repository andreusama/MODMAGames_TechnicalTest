using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine
{
    [CreateAssetMenu(menuName = "Test")]
    public class DependenciesHunterInformation : ScriptableObject
    {
        public List<DependenciesHunterInformationResult> MapListCache = new List<DependenciesHunterInformationResult>();

        public void AddResult(string guidSearched, List<string> dependencies)
        {
            MapListCache.Add(new DependenciesHunterInformationResult(guidSearched, dependencies));
        }
        public Dictionary<string, List<string>> GetDictionary()
        {
            var diction = new Dictionary<string, List<string>>();
            foreach (var result in MapListCache)
            {
                if (result == null || string.IsNullOrEmpty(result.Guid))
                {
                    Debug.LogError("Something wrong with a result in MapListCache, reset DependenciesHunter ScriptableObject");
                    return diction;
                }
                diction.Add(result.Guid, result.Dependencies);
            }
            return diction;
        }
        public void AddCacheMap(Dictionary<string, List<string>> map)
        {
            MapListCache.Clear();
            foreach (var pair in map)
            {
                MapListCache.Add(new DependenciesHunterInformationResult(pair.Key, pair.Value));
            }
        }
        public void Clear()
        {
            MapListCache.Clear();
        }
    }

    [System.Serializable]
    public class DependenciesHunterInformationResult
    {
        public string Guid;
        public List<string> Dependencies = new List<string>();
        public DependenciesHunterInformationResult(string guidSearched, List<string> dependencies)
        {
            Guid = guidSearched;
            Dependencies = dependencies;
        }
    }
}
