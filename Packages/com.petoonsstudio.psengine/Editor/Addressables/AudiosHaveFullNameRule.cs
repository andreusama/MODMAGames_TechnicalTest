using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Addressables
{
    public class AudiosHaveFullNameRule : UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule
    {
        [SerializeField]
        private List<AddressableAssetEntry> m_BadlyNamedEntries = new List<AddressableAssetEntry>();

        private const string kLocalizationGroupName = "Localization";
        private const string kSharedDataName = "Localization-Assets-Shared";
        private const string kFolderSeparator = "/";
        private const string kNoIssuesMessage = "No issues found";
        private const string kAssetExtension = ".asset";

        public override bool CanFix
        {
            get => true;
            set => base.CanFix = value;
        }

        public override string ruleName => "Check there are no slashes on audio tables";

        public override List<AnalyzeResult> RefreshAnalysis(AddressableAssetSettings settings)
        {
            List<AnalyzeResult> results = new List<AnalyzeResult>();

            foreach (var group in settings.groups)
            {
                if (group.HasSchema<PlayerDataGroupSchema>())
                    continue;

                if (!group.Name.Contains(kLocalizationGroupName))
                    continue;

                if (group.Name.Contains(kSharedDataName))
                    continue;

                foreach (var entry in group.entries)
                {
                    if (entry.address.Contains(kFolderSeparator) || (!string.IsNullOrEmpty(Path.GetExtension(entry.address)) && Path.GetExtension(entry.address) != kAssetExtension))
                    {
                        m_BadlyNamedEntries.Add(entry);
                        results.Add(new AnalyzeResult { resultName = group.name + kDelimiter + entry.address, severity = MessageType.Warning });
                    }
                }
            }

            if (results.Count == 0)
                results.Add(new AnalyzeResult { resultName = kNoIssuesMessage });

            return results;
        }

        public override void FixIssues(AddressableAssetSettings settings)
        {
            if (m_BadlyNamedEntries == null || m_BadlyNamedEntries.Count == 0)
                return;

            List<string> splitAddress = new List<string>();
            foreach (var entry in m_BadlyNamedEntries)
            {
                string fixedEntry = entry.address;
                if (!string.IsNullOrEmpty(Path.GetExtension(entry.address)))
                    fixedEntry = Path.GetFileNameWithoutExtension(entry.address);

                splitAddress = fixedEntry.Split(kFolderSeparator).ToList();
                entry.address = splitAddress[splitAddress.Count - 1];
                splitAddress.Clear();
            }

            m_BadlyNamedEntries.Clear();
        }

        [InitializeOnLoad]
        public class RegisterAudiosHaveFullNameRule
        {
            static RegisterAudiosHaveFullNameRule()
            {
                AnalyzeSystem.RegisterNewRule<AudiosHaveFullNameRule>();
            }
        }
    }
}
