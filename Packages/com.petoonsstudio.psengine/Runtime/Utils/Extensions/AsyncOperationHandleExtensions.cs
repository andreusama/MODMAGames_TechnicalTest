using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class AsyncOperationHandleExtensions
    {
        public static bool Succeeded<T>(this AsyncOperationHandle<T> operation)
        {
            return operation.Status == AsyncOperationStatus.Succeeded;
        }

    }
}