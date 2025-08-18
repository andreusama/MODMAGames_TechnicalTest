using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Localization;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Tools
{
    public class AddressablesAssetsPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool isDomainReload)
        {
            if (isDomainReload)
                return;

            foreach (string assetPath in importedAssets)
            {
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));

                if (asset == null)
                    continue;

                if (asset is not LocalizationTableCollection table) continue;

                table.SetPreloadTableFlag(false);
            }

        }

    }
}
