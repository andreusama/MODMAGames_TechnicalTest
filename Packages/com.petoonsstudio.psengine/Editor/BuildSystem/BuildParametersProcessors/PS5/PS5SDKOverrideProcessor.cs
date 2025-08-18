namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class PS5SDKOverrideProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "PS5SDKOverride";

        public int Priority => BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
#if UNITY_PS5
            UnityEditor.PS5.PlayerSettings.sdkOverride = value;
            JenkinsBuilder.SystemLog($"Modified {PARAMETER_ID}: {value}");
#endif
            return BuildProcessorResult.Success;
        }
    }
}
