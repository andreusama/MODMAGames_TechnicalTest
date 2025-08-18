namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class DebugDirective : IBuildParameterProcessor
    {
        private const string PARAMETER_ID = "Debug";
        private const string DEBUG_DIRECTIVE = "PETOONS_DEBUG";

        public int Priority => BuildParameterProcessorUtils.HIGH_BPP_PRIORITY;

        public string ID => PARAMETER_ID;

        public BuildProcessorResult ApplyBuildParameter(string value)
        {
            if(bool.TryParse(value, out var result))
            {
                if (result)
                {
                    if (!CITools.ExistDefine(DEBUG_DIRECTIVE))
                    {
                        CITools.AddDefine(DEBUG_DIRECTIVE);
                    }
                }
                else
                {
                    if (CITools.ExistDefine(DEBUG_DIRECTIVE))
                    {
                        CITools.RemoveDefine(DEBUG_DIRECTIVE);
                    }
                }
                return BuildProcessorResult.Success;
            }

            return BuildProcessorResult.Warning;
        }
    }
}
