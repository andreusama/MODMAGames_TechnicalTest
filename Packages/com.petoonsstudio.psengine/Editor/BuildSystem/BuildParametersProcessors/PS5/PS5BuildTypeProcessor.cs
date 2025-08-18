using System;
#if UNITY_PS5
using UnityEditor.PS5;
#endif

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class PS5BuildTypeProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "PS5BuildType";

        public int Priority => BuildParameterProcessorUtils.HIGH_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
#if UNITY_PS5
            JenkinsBuilder.SystemLog($"Modified {PARAMETER_ID}: {value}");
            
            if (Enum.TryParse(value, true, out PS5BuildSubtarget result))
            {
                PlayerSettings.buildSubtarget = result;
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
