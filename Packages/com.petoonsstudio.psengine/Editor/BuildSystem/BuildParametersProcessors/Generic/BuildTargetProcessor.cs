using PetoonsStudio.PSEngine.BuildSystem;
using System;
using UnityEditor;

namespace PetoonsStudio.PSEngine
{
    public class BuildTargetProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "BuildTarget";

        public int Priority => BuildParameterProcessorUtils.HIGH_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
            JenkinsBuilder.SystemLog($"Modified BuildGroup: {value}");
            if (Enum.TryParse(value, false, out BuildTarget result))
            {
                JenkinsBuilder.SystemLog($"Modified BuildTarget: {result.ToString()}");
                PetoonsMasterBuilder.BuildParameters.BuildTarget = result;
                return BuildProcessorResult.Success;
            }
            else
            {
                return BuildProcessorResult.Critical;
            }
        }
    }
}
