using System;
using UnityEditor;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class Il2CppCompilerConfigurationProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "Il2CppConfiguration";

        public int Priority => BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
            if (Enum.TryParse(value, true, out Il2CppCompilerConfiguration result))
            {
                PlayerSettings.SetIl2CppCompilerConfiguration(EditorUserBuildSettings.selectedBuildTargetGroup, result);
                JenkinsBuilder.SystemLog($"Modified Il2CppConfiguration: {result}");
                return BuildProcessorResult.Success;
            }

            return BuildProcessorResult.Warning;
        }
    }
}
