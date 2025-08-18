using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.AnalyzeRules;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Addressables
{
    public class DuplicatedAssetEntryRule : AnalyzeRule
    {
        public override bool CanFix
        {
            get { return false; }
            set { }
        }

        public override string ruleName
        {
            get { return "Check Duplicated Asset Entries"; }
        }

        [SerializeField]
        List<AddressableAssetEntry> m_DuplicatedEntries = new();

        public override List<AnalyzeResult> RefreshAnalysis(AddressableAssetSettings settings)
        {
            List<AnalyzeResult> results = new();
            List<AddressableAssetEntry> entries = new();
            foreach (var group in settings.groups)
            {
                if (group.HasSchema<PlayerDataGroupSchema>())
                    continue;

                foreach (var entry in group.entries)
                {
                    if (entries.Where((collectedEntry) => collectedEntry.guid == entry.guid).Count() > 0)
                    {
                        results.Add(new AnalyzeResult { resultName = group.Name + kDelimiter + entry.address, severity = MessageType.Error });
                    }
                    else
                    {
                        entries.Add(entry);
                    }
                }
            }

            if (results.Count == 0)
                results.Add(new AnalyzeResult { resultName = ruleName + " - No issues found." });

            return results;
        }

        public override void ClearAnalysis()
        {
            m_DuplicatedEntries = new List<AddressableAssetEntry>();
        }
    }

    [InitializeOnLoad]
    class RegisterDuplicatedEntryRule
    {
        static RegisterDuplicatedEntryRule()
        {
            AnalyzeSystem.RegisterNewRule<DuplicatedAssetEntryRule>();
        }
    }
}


