using UnityEditor;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class EnableDebugPadProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "EnableDebugPad";

        public int Priority => BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
            if (bool.TryParse(value, out var result))
            {
#if UNITY_SWITCH
                EditorUserBuildSettings.switchEnableDebugPad = result;
#endif
                JenkinsBuilder.SystemLog($"Modified NSW Enable Debug Pad: {result}");
                return BuildProcessorResult.Success;
            }

            return BuildProcessorResult.Warning;
        }
    }
}
