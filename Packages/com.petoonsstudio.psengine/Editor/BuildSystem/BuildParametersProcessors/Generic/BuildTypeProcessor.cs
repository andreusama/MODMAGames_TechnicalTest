using System;
using UnityEditor;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class BuildTypeProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "BuildType";

        public int Priority => BuildParameterProcessorUtils.HIGH_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
            switch (value)
            {
                case "Development":
                    SetDevelopmentBuild(value);
                    break;
                case "Release":
                    SetReleaseBuild();
                    break;
                default:
                    return BuildProcessorResult.Critical;
            }
            return BuildProcessorResult.Success;
        }

        private void SetDevelopmentBuild(string value)
        {
            if (Enum.TryParse(value, true, out BuildOptions result))
            {
                JenkinsBuilder.SystemLog($"Modified BuildType: {value}");
                PetoonsMasterBuilder.BuildParameters.BuildOptions |= result;
            }
        }

        private void SetReleaseBuild()
        {
            if (PetoonsMasterBuilder.BuildParameters.BuildOptions.HasFlag(BuildOptions.Development))
            {
                PetoonsMasterBuilder.BuildParameters.BuildOptions &= ~BuildOptions.Development;
            }
        }
    }
}
