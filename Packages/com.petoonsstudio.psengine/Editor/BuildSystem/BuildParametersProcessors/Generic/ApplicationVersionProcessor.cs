using UnityEditor;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class ApplicationVersionProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "ApplicationVersion";
        private const string DEFAULT_VERSION = "0";

        public int Priority => BuildParameterProcessorUtils.HIGH_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
            if(value == DEFAULT_VERSION)
            {
                JenkinsBuilder.SystemLog($"Current Application version: {PlayerSettings.bundleVersion}");
                return BuildProcessorResult.Success;
            }
            else if (!string.IsNullOrEmpty(value))
            {
                PlayerSettings.bundleVersion = value;
                JenkinsBuilder.SystemLog($"Modified Application version: {value}");
                return BuildProcessorResult.Success;
            }

            return BuildProcessorResult.Warning;
        }
    }
}
