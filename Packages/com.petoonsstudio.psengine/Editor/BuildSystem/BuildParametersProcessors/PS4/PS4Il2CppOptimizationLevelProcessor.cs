#if UNITY_PS4
using UnityEditor;
using UnityEngine;
#endif

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class PS4Il2CppOptimizationLevelProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "PS4OptimizationLevel";

        private const int MIN_OPT_LVL = 0;
        private const int MAX_OPT_LVL = 3;

        public int Priority => BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
#if UNITY_PS4
            if(int.TryParse(value, out var result))
            {
                result = Mathf.Clamp(result, MIN_OPT_LVL, MAX_OPT_LVL);
                PlayerSettings.PS4.scriptOptimizationLevel = result;
                JenkinsBuilder.SystemLog($"Modified {PARAMETER_ID}: {result}");
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
