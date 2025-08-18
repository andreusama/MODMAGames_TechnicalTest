using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Tools
{
    public class MaterialsEditorProcessor : AssetPostprocessor
    {
        public static readonly string USERDATA_KEY = "ProcessedMaterial";
        public static readonly string INFORMATION_SEPARATOR = ";";
        public static readonly string INFORMATION_ASSIGNATMENT = "=";

        public static readonly string MATERIAL_EXTENSION = ".mat";

        [RunBeforeClass(typeof(AddressablesAssetsPostProcessor))]
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool isDomainReload)
        {
            if (isDomainReload)
                return;

            if (importedAssets.Length > 0)
            {
                HandleImportedAssests(importedAssets);
            }
        }

        private static void HandleImportedAssests(string[] importedAssets)
        {
            foreach (var asset in importedAssets)
            {
                if (!asset.EndsWith(MATERIAL_EXTENSION, StringComparison.InvariantCultureIgnoreCase))
                    return;

                var settings = PetoonsMaterialsProjectSettings.GetOrCreateSettings();

                if (!settings.Enabled)
                    return;

                if (settings.IsAssetExcluded(asset))
                    return;

                var assetImporter = AssetImporter.GetAtPath(asset);
                if (PetoonsProcessorUtils.AlreadyImported(assetImporter, USERDATA_KEY))
                {
                    return;
                }

                var material = (Material)AssetDatabase.LoadAssetAtPath(asset, typeof(Material));
                string defaultShader = material.shader.name;

                if (settings.DefaultShaderMaterial != null)
                {
                    material.shader = settings.DefaultShaderMaterial;
                    defaultShader = settings.DefaultShaderMaterial.name;
                }

                PetoonsProcessorUtils.AddUserData(assetImporter, USERDATA_KEY + INFORMATION_SEPARATOR + nameof(material.shader) + INFORMATION_ASSIGNATMENT + defaultShader);
                AssetDatabase.WriteImportSettingsIfDirty(assetImporter.assetPath);
            }
        }

        public static void ApplyImportUserDataToExistingMaterials()
        {
            foreach (var materialGUID in AssetDatabase.FindAssets("t:Material", new string[] { "Assets" }))
            {
                var materialPath = AssetDatabase.GUIDToAssetPath(materialGUID);
                var settings = PetoonsMaterialsProjectSettings.GetOrCreateSettings();

                if (settings.IsAssetExcluded(materialPath))
                    continue;

                AssetImporter importer = AssetImporter.GetAtPath(materialPath);

                if (!PetoonsProcessorUtils.ContainsKey(importer, USERDATA_KEY))
                {
                    var material = (Material)AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Material));
                    PetoonsProcessorUtils.AddUserData(importer, USERDATA_KEY + INFORMATION_SEPARATOR + nameof(material.shader) + INFORMATION_ASSIGNATMENT + material.shader.name);
                    AssetDatabase.WriteImportSettingsIfDirty(importer.assetPath);
                }
            }
        }

        public static void RevertMaterialImporterChanges()
        {
            foreach (var materialGUID in AssetDatabase.FindAssets("t:Material", new string[] { "Assets" }))
            {
                var settings = PetoonsMaterialsProjectSettings.GetOrCreateSettings();
                var materialPath = AssetDatabase.GUIDToAssetPath(materialGUID);

                if (settings.IsAssetExcluded(materialPath))
                    continue;

                AssetImporter importer = AssetImporter.GetAtPath(materialPath);

                if (PetoonsProcessorUtils.ContainsKey(importer, USERDATA_KEY))
                {
                    var importerData = PetoonsProcessorUtils.GetImporterUserData(importer, USERDATA_KEY);
                    ProcessImporterUserData(importer, importerData);
                }
            }
        }

        private static void ProcessImporterUserData(AssetImporter importer, string userData)
        {
            var infoSplit = userData.Split(INFORMATION_SEPARATOR);
            var shaderProperty = infoSplit[1];
            var propertyName = shaderProperty.Split(INFORMATION_ASSIGNATMENT)[0];
            var propertyValue = shaderProperty.Split(INFORMATION_ASSIGNATMENT)[1];

            var material = (Material)AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Material));
            var property = material.GetType().GetProperty(propertyName);
            property.SetValue(material, Shader.Find(propertyValue));
        }
    }
}
