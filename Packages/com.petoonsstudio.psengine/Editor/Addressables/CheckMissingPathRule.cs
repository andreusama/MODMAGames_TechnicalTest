using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Build.AnalyzeRules;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEditor.AddressableAssets.Build;
using UnityEditor;

namespace PetoonsStudio.PSEngine.Addressables
{
    public class CheckMissingPathRule : AnalyzeRule
    {
        public override bool CanFix
        {
            get { return false; }
            set { }
        }

        public override string ruleName
        {
            get { return "Check Missing Asset path in Entries"; }
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
                    if (string.IsNullOrEmpty(e.AssetPath))
                    {
                        results.Add(new AnalyzeResult { resultName = group.Name + kDelimiter + e.address, severity = MessageType.Error });
                    }
                }
            }

            if (results.Count == 0)
                results.Add(new AnalyzeResult { resultName = ruleName + " - No issues found." });

            return results;
        }

        public override void ClearAnalysis()
        {
            m_MisnamedEntries = new List<AddressableAssetEntry>();
        }
    }

    [InitializeOnLoad]
    class RegisterMissingPathRule
    {
        static RegisterMissingPathRule()
        {
            AnalyzeSystem.RegisterNewRule<CheckMissingPathRule>();
        }
    }
}
