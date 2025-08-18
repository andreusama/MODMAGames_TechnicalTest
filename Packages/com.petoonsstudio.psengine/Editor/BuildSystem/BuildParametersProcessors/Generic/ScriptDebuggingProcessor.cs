using UnityEditor;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class ScriptDebuggingProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "ScriptDebugging";

        public int Priority => BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
            if (bool.TryParse(value, out bool result))
            {
                if(result)
                    PetoonsMasterBuilder.BuildParameters.BuildOptions |= BuildOptions.AllowDebugging;

                JenkinsBuilder.SystemLog($"Modified Script Debugging: {result}");
                return BuildProcessorResult.Success;
            }

            return BuildProcessorResult.Warning;
        }
    }
}
