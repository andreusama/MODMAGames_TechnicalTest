using UnityEditor;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class PS4PatchApplicationPkgProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "PS4PatchApplicationPkg";

        public int Priority => BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
#if UNITY_PS4
            JenkinsBuilder.SystemLog($"Modified {PARAMETER_ID}: {value}");
            PlayerSettings.PS4.PatchPkgPath = value;
#endif
            return BuildProcessorResult.Success;
        }
    }
}
