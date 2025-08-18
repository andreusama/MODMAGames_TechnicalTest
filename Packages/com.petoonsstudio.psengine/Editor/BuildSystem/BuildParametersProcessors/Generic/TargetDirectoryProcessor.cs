using PetoonsStudio.PSEngine.BuildSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PetoonsStudio.PSEngine
{
    public class TargetDirectoryProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "TargetDir";

        public int Priority => BuildParameterProcessorUtils.HIGH_BPP_PRIORITY;
        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
            if (string.IsNullOrEmpty(value))
                return BuildProcessorResult.Critical;

            string path = Path.Combine(value, Application.productName);

#if UNITY_STANDALONE
            path += ".exe";
#endif
            JenkinsBuilder.SystemLog($"Modified Target directory: {path}");
            PetoonsMasterBuilder.BuildParameters.TargetDir = path;

            return BuildProcessorResult.Success;
        }
    }
}
