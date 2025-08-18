#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Addressables
{
    public class AddressableNamePathRule : UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule
    {
        public override bool CanFix
        {
            get { return true; }
            set { }
        }

        public override string ruleName
        {
            get { return "Compares if Addressable Name and Path are different"; }
        }

        [SerializeField]
        List<AddressableAssetEntry> m_MisnamedEntries = new();

        public override List<AnalyzeResult> RefreshAnalysis(AddressableAssetSettings settings)
        {
            List<AnalyzeResult> results = new List<AnalyzeResult>();
            foreach (var group in settings.groups)
            {
                if (group.HasSchema<PlayerDataGroupSchema>())
                    continue;
                foreach (var e in group.entries)
                {
                    if (!e.address.Equals(e.AssetPath) && !e.ReadOnly)
                    {
                        m_MisnamedEntries.Add(e);
                        results.Add(new AnalyzeResult { resultName = group.Name + kDelimiter + e.address, severity = MessageType.Error });
                    }
                }
            }

            if (results.Count == 0)
                results.Add(new AnalyzeResult { resultName = ruleName + " - No issues found." });

            return results;
        }

        public override void FixIssues(AddressableAssetSettings settings)
        {
            foreach (var e in m_MisnamedEntries)
            {
                e.address = e.AssetPath;
            }
            m_MisnamedEntries = new List<AddressableAssetEntry>();
        }

        public override void ClearAnalysis()
        {
            m_MisnamedEntries = new List<AddressableAssetEntry>();
        }
    }

    [InitializeOnLoad]
    class RegisterNamePathRule
    {
        static RegisterNamePathRule()
        {
            AnalyzeSystem.RegisterNewRule<AddressableNamePathRule>();
        }
    }

}
#endif
