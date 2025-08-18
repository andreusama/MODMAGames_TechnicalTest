using UnityEditor;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class PS4NPTitleSelectorProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "PS4NPTitle";

        public int Priority => BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
            JenkinsBuilder.SystemLog($"Modified {PARAMETER_ID}: {value}");
#if UNITY_PS4
            PlayerSettings.PS4.NPtitleDatPath = value;
#endif
            return BuildProcessorResult.Success;
        }
    }
}
