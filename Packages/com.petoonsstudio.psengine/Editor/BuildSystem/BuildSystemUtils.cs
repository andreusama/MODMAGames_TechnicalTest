using System;
using System.Collections.Generic;
using System.Linq;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public static class BuildSystemUtils
    {
        public static List<T> GetInterfacesInstances<T>()
        {
            var type = typeof(T);
            var preBuildPipelineProcessorTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);

            List<T> processors = new List<T>();

            foreach (var parameterprocessor in preBuildPipelineProcessorTypes)
            {
                T processor = (T)Activator.CreateInstance(parameterprocessor);
                processors.Add(processor);
            }

            return processors;
        }
    }
}
