using System;
using UnityEditor;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class PS4BuildTypeProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "PS4BuildType";

        public int Priority => BuildParameterProcessorUtils.HIGH_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
#if UNITY_PS4

            if (Enum.TryParse(value, true, out PS4BuildSubtarget result))
            {
                EditorUserBuildSettings.ps4BuildSubtarget = result;
                JenkinsBuilder.SystemLog($"Modified {PARAMETER_ID}: {result}");
                return BuildProcessorResult.Success;
            }
            else
            {
                JenkinsBuilder.SystemLog($"FAILED to modify {PARAMETER_ID}: {value}");
                return BuildProcessorResult.Warning;
            }
#endif
            return BuildProcessorResult.Success;
        }
    }
}
