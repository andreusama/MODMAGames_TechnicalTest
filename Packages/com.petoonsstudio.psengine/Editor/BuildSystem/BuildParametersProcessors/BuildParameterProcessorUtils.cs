using System;
using System.Collections.Generic;
using System.Linq;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public static class BuildParameterProcessorUtils
    {
        public static int HIGH_BPP_PRIORITY = 0;
        public static int DEFAULT_BPP_PRIORITY = 10;
        public static int LOW_BPP_PRIORITY = 20;

        private readonly static string PARAMETER_ASSIGN_CHAR = "=";

        public static BuildParameterCommand[] ParseCommands(string[] args)
        {
            BuildParameterCommand[] commands = new BuildParameterCommand[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var values = args[i].Split(PARAMETER_ASSIGN_CHAR);
                commands[i] = new BuildParameterCommand(values[0], values[1]);
            }
            return commands;
        }

        public static List<IBuildParameterProcessor> GetParameterProcessors(BuildParameterCommand[] commands)
        {
            List<IBuildParameterProcessor> selectedProcessors = new List<IBuildParameterProcessor>();

            foreach (var processor in BuildSystemUtils.GetInterfacesInstances<IBuildParameterProcessor>())
            {
                if(commands.Any(command => command.ID == processor.ID))
                    selectedProcessors.Add(processor);
            }

            return selectedProcessors;
        }
    }
}
