using UnityEditor;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class WaitScriptDebuggingProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "WaitScriptDebugging";

        public int Priority => BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
            if (bool.TryParse(value, out bool result))
            {
                if (result)
                {
                    EditorUserBuildSettings.waitForManagedDebugger = true;
                }
                else
                {
                    EditorUserBuildSettings.waitForManagedDebugger = false;
                }

                JenkinsBuilder.SystemLog($"Modified waitForManagedDebugger: {result}");
                return BuildProcessorResult.Success;
            }

            return BuildProcessorResult.Warning;
        }
    }
}
