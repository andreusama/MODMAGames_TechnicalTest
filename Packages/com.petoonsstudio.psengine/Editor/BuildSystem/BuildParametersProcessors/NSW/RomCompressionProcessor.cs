using UnityEditor;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class RomCompressionProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "RomCompression";

        public int Priority => BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
            if (bool.TryParse(value, out var result))
            {
#if UNITY_SWITCH
                EditorUserBuildSettings.switchEnableRomCompression = result;
#endif
                JenkinsBuilder.SystemLog($"Modified NSW ROM Compression: {result}");
                return BuildProcessorResult.Success;
            }

            return BuildProcessorResult.Warning;
        }
    }
}
