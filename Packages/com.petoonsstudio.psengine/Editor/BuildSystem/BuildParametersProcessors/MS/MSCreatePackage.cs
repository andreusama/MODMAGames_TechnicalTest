namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class MSCreatePackage : IBuildParameterProcessor
    {
        public int Priority => BuildParameterProcessorUtils.HIGH_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        private const string PARAMETER_ID = "MSCreatePackage";

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
#if MICROSOFT_GAME_CORE
            if (bool.TryParse(value, out bool result))
            {
                PetoonsMasterBuilder.CreatePackage = result;
                JenkinsBuilder.SystemLog($"Modified MS Create Package: {result}");
                return BuildProcessorResult.Success;
            }
            else
            {
                return BuildProcessorResult.Warning;
            }
#endif
            return BuildProcessorResult.Success;
        }
    }
}
