using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class RunInfo
    {
        public int count;
        public static Dictionary<string, RunInfo> runners = new Dictionary<string, RunInfo>();
    }

    public class ParallelSupport : MonoBehaviour
    {
        public static ParallelSupport instance;
        void Awake() {
            if (instance == null)
            {
                instance = this;
            }
        }

    }

    public static class ParallelCoroutineExt
    {
        public static RunInfo ParallelCoroutine(this IEnumerator coroutine, string group = "default")
        {
            if (!RunInfo.runners.ContainsKey(group))
            {
                RunInfo.runners[group] = new RunInfo();
            }
            var ri = RunInfo.runners[group];
            ri.count++;
            ParallelSupport.instance.StartCoroutine(DoParallel(coroutine, ri));
            return ri;
        }
        static IEnumerator DoParallel(IEnumerator coroutine, RunInfo ri)
        {
            yield return ParallelSupport.instance.StartCoroutine(coroutine);
            ri.count--;
        }

        
    }

    
}