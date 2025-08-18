using UnityEditor;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class CreateRomProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "CreateRom";

        public int Priority => BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
            if(bool.TryParse(value, out var result))
            {
#if UNITY_SWITCH
                EditorUserBuildSettings.switchCreateRomFile = result;
#endif
                PetoonsMasterBuilder.BuildParameters.TargetDir += "Build.nsp";
                JenkinsBuilder.SystemLog($"Modified NSW create ROM: {result}");
                return BuildProcessorResult.Success;
            }

            return BuildProcessorResult.Warning;
        }
    }
}
