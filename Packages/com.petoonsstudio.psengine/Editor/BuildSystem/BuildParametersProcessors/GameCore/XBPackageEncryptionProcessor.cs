using System;

#if UNITY_GAMECORE
using UnityEditor.GameCore;
#endif

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class XBPackageEncryptionProcessor : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "XBPkgEncryption";

        public int Priority => BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
#if UNITY_GAMECORE
            if (Enum.TryParse(value, out GameCorePackageEncryption result))
            {
                GameCoreSettings gameCoreSettings;
#if UNITY_GAMECORE_XBOXONE
                gameCoreSettings = GameCoreXboxOneSettings.GetInstance();
#elif UNITY_GAMECORE_XBOXSERIES
                gameCoreSettings = GameCoreScarlettSettings.GetInstance();
#endif
                JenkinsBuilder.SystemLog($"Modified {PARAMETER_ID}: {result}");
                gameCoreSettings.PackageEncryption = result;
            }
            else
            {
                JenkinsBuilder.SystemLog($"FAILED to modify {PARAMETER_ID}: {value}");
                return BuildProcessorResult.Warning;
            }
#endif

            return BuildProcessorResult.Success;
        }
    }
}
