using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    internal class UnmanagedCallback<T, U>
    {
        internal T directCallback;
        internal U userCallback;
    }

    class AsyncHelpers
    {
        internal static XAsyncBlockPtr WrapAsyncBlock(XTaskQueueHandle queue, XAsyncCompletionRoutine callback)
        {
            var acb = new UnmanagedCallback<XAsyncCompletionRoutine, XAsyncCompletionRoutine>
            {
                directCallback = AsyncBlockCallback,
                userCallback = callback
            };

            // Prevent callbacks from being GC'd
            GCHandle gcHandle = GCHandle.Alloc(acb);

            XAsyncBlock asyncBlock = new XAsyncBlock()
            {
                queue = queue,
                context = GCHandle.ToIntPtr(gcHandle),
                callback = Marshal.GetFunctionPointerForDelegate(acb.directCallback)
            };

            Int32 blockSize = Marshal.SizeOf(asyncBlock);
            IntPtr asyncBlockPnt = Marshal.AllocHGlobal(blockSize);
            Marshal.StructureToPtr(asyncBlock, asyncBlockPnt, false);

            return new XAsyncBlockPtr(asyncBlockPnt);
        }

        internal static void CleanupAsyncBlock(XAsyncBlockPtr block)
        {
            XAsyncBlock asyncBlock = (XAsyncBlock)Marshal.PtrToStructure(block.IntPtr, typeof(XAsyncBlock));
            GCHandle callbackHandle = GCHandle.FromIntPtr(asyncBlock.context);
            callbackHandle.Free();
            Marshal.FreeHGlobal(block.IntPtr);
        }

        [MonoPInvokeCallback]
        private static void AsyncBlockCallback(XAsyncBlockPtr block)
        {
            XAsyncBlock asyncBlock = (XAsyncBlock)Marshal.PtrToStructure(block.IntPtr, typeof(XAsyncBlock));
            GCHandle callbackHandle = GCHandle.FromIntPtr(asyncBlock.context);
            var ab = callbackHandle.Target as UnmanagedCallback<XAsyncCompletionRoutine, XAsyncCompletionRoutine>;

#if DEVELOPMENT_BUILD
            try 
            {
#endif
            // invoke user callback
            ab.userCallback(block);
            
#if DEVELOPMENT_BUILD
            }
            catch ( System.Exception ex )
            {
                UnityEngine.Debug.Log($"XTaskQueueDispatch encountered an Exception in an AsyncBlockCallback, application will likely crash:");
                UnityEngine.Debug.Log ( ex );
                throw;
            }
#endif
            // clean up pinned GC content
            callbackHandle.Free();
            Marshal.FreeHGlobal(block.IntPtr);
        }
    }
}
