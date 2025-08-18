namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class PS5ReferencePackageProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "PS5ReferencePkg";

        public int Priority => BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
#if UNITY_PS5
            JenkinsBuilder.SystemLog($"Modified {PARAMETER_ID}: {value}");
            UnityEditor.PS5.PlayerSettings.updateReferencePackage = value;
#endif
            return BuildProcessorResult.Success;
        }
    }
}
