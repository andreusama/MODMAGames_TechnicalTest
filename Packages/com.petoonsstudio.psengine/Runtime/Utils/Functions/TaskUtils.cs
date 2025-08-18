using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class TaskUtils
    {
        public static async Task WaitUntil(Func<bool> predicate, int sleep = 50)
        {
            while (!predicate())
            {
                await Task.Delay(sleep);
            }
        }

        public static async Task WaitUntil(Func<bool> predicate, TimeSpan timeSpan)
        {
            while (!predicate())
            {
                await Task.Delay(timeSpan);
            }
        }

        public static async Task WaitYieldCoroutine(Func<IEnumerator> yieldFunction, MonoBehaviour yieldMonoBehaviourInstance, int sleep = 50)
        {
            bool completed = false;
            yieldMonoBehaviourInstance.StartCoroutine(CoroutineWithCompleteCallback(yieldFunction, yieldMonoBehaviourInstance, () => completed = true));
            await WaitUntil(() => completed == true, sleep);
        }

        private static IEnumerator CoroutineWithCompleteCallback(Func<IEnumerator> yieldFunction, MonoBehaviour monoBehaviourInstance, Action onCompleteCallBack)
        {
            yield return monoBehaviourInstance.StartCoroutine(yieldFunction());
            onCompleteCallBack.Invoke();
        }
    }
}