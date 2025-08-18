using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class SetupBuildAddressablesWithPlayerBuildProcessor : IBuildParameterProcessor
    {
        public int Priority => throw new System.NotImplementedException();

        public string ID => PARAMETER_ID;

        private const string PARAMETER_ID = "SetupBuildAddressables";

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
            if (bool.TryParse(value, out bool result))
            {
                AddressableAssetSettingsDefaultObject.Settings.BuildAddressablesWithPlayerBuild = result ?
                    AddressableAssetSettings.PlayerBuildOption.BuildWithPlayer :
                    AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer;

                JenkinsBuilder.SystemLog($"Modified Addressables Build With Player Settings: {value}");

                return BuildProcessorResult.Success;
            }
            else
            {
                return BuildProcessorResult.Warning;
            }
        }
    }
}
