namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class MSPackageEncryption : IBuildParameterProcessor
    {
        public int Priority => BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        private const string PARAMETER_ID = "MSPackageEncryption";

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
#if MICROSOFT_GAME_CORE
            if(bool.TryParse(value, out bool result))
            {
                PetoonsMasterBuilder.SubmissionEncryption = result;
                JenkinsBuilder.SystemLog($"Modified MS Submission encryption: {result}");
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
