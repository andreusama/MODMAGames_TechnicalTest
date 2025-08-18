using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using PetoonsStudio.PSEngine.Tools;
using System.IO;
using UnityEngine;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public struct BuildParameters
    {
        public string TargetDir;
        public BuildOptions BuildOptions;
        public BuildTarget BuildTarget;
    }

    public struct BuildParameterCommand
    {
        public string ID;
        public string Value;

        public BuildParameterCommand(string ID, string Value)
        {
            this.ID = ID;
            this.Value = Value;
        }
    }

    public enum BuildProcessorResult
    {
        Critical,
        Warning,
        Success
    }

    public static class PetoonsMasterBuilder
    {
        public static BuildPlayerOptions BuildOptions;
        public static string[] EnabledScenes = FindEnabledEditorScenes();
        public static BuildParameters BuildParameters;

#if MICROSOFT_GAME_CORE
        public static bool SubmissionEncryption = false;
        public static bool CreatePackage = true;
#endif

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(OpenPetoonsBuildWindow);
        }

        public static void OpenPetoonsBuildWindow(BuildPlayerOptions options)
        {
            BuildOptions = options;

            PetoonsBuildWindow.Init();
        }

        public static void InternalBuildPipeline(bool buildContent)
        {
            Debug.Log("[PETOONS_BUILDER] Executing Pre BuildPipeline Processors");
            foreach (var processor in BuildSystemUtils.GetInterfacesInstances<IPreBuildPipelineProcessor>())
            {
                processor.OnPreBuildPipeline();
            }

            Debug.Log("[PETOONS_BUILDER] Executing Addressables steps");
            if (buildContent)
            {
                AddressableAssetSettings.CleanPlayerContent(
                    AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
                AddressableAssetSettings.BuildPlayerContent();
            }

            Debug.Log("[PETOONS_BUILDER] Executing Player Build");
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(BuildOptions);
        }

#region CI
        public static void BuildCI()
        {
            JenkinsBuilder.BuildCI(EnabledScenes);
        }

        public static void BuildAddressablesCI()
        {
            JenkinsBuilder.BuildAddressables();
        }

        public static void AddDirectivesCI()
        {
            JenkinsBuilder.AddDirectives();
        }
#endregion

        private static string[] FindEnabledEditorScenes()
        {
            List<string> EditorScenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                if (scene.enabled)
                    EditorScenes.Add(scene.path);

            return EditorScenes.ToArray();
        }
    }
}
