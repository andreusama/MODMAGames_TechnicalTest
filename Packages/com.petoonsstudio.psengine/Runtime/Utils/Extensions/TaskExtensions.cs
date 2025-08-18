using System;
using System.Threading.Tasks;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class TaskExtensions
    {
        public static bool Succeeded<T>(this System.Threading.Tasks.Task<T> task)
        {
            return task.Status == System.Threading.Tasks.TaskStatus.RanToCompletion;
        }
    }
}