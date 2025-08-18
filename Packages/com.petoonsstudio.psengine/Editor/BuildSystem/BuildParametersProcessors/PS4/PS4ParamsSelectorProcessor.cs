using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class PS4ParamsSelectorProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "PS4Params";

        public int Priority => BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
#if UNITY_PS4
            JenkinsBuilder.SystemLog($"Modified {PARAMETER_ID}: {value}");
            PlayerSettings.PS4.paramSfxPath = value;
#endif
            return BuildProcessorResult.Success;
        }
    }
}
