namespace PetoonsStudio.PSEngine.BuildSystem
{
    public interface IBuildParameterProcessor
    {
        /// <summary>
        /// Determine the priority of this Processor.
        /// The lower the value the higher the priority.
        /// You can use default values:
        /// BuildParameterProcessorUtils.HIGH_BPP_PRIORITY (0)
        /// BuildParameterProcessorUtils.DEFAULT_BPP_PRIORITY (10)
        /// BuildParameterProcessorUtils. LOW_BP_PRIORITY (20)
        /// Or apply a custom one.
        /// Note that processors such as the TargerDir should always be among the first.
        /// </summary>
        public int Priority { get; }
        public string ID { get; }
        public BuildProcessorResult ApplyBuildParameter(string value);
    }
}
