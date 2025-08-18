using System;
using System.Threading;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public partial class SDK
    {
        static XTaskQueue defaultQueue = null;
        static volatile bool isInitialized = false; // because of the access pattern to this bool, volatile is enough here
        static Thread m_DispatchJob;
        static volatile bool m_StopExecution;       // because of the access pattern to this bool, volatile is enough here

        public static Int32 XGameRuntimeInitialize()
        {
            if (!isInitialized)
            {
                Int32 hr = XGRInterop.XGameRuntimeInitialize();
                if (HR.SUCCEEDED(hr))
                {
                    XTaskQueueHandle handle;
                    hr = XGRInterop.XTaskQueueCreate(XTaskQueueDispatchMode.Manual, XTaskQueueDispatchMode.Manual, out handle);

                    if (HR.SUCCEEDED(hr))
                    {
                        defaultQueue = new XTaskQueue { handle = handle };

                        m_StopExecution = false;
                        m_DispatchJob = new Thread(DispatchGXDKTaskQueue) { Name = "GXDK Task Queue Dispatch" };
                        m_DispatchJob.Start();

                        isInitialized = true;
                    }
                    else
                    {
                        // couldn't create a queue so nothing will work, uninitialise
                        XGRInterop.XGameRuntimeUninitialize();
                    }
                }

                return hr;
            }

            return HR.E_FAIL;
        }

        static void DispatchGXDKTaskQueue(object completionDelegate)
        {
            // this will execute all GXDK async work
            while (m_StopExecution == false)
            {
                XGRInterop.XTaskQueueDispatch(defaultQueue.handle, XTaskQueuePort.Work, 0);
                Thread.Sleep(32);
            }

            DispatchGXDKTaskDone();
        }

        static void DispatchGXDKTaskDone()
        {
            XGRInterop.XTaskQueueCloseHandle(defaultQueue.handle);
            XGRInterop.XGameRuntimeUninitialize();

            isInitialized = false;
        }

        public static void XGameRuntimeUninitialize()
        {
            if (isInitialized)
            {
                if (!m_StopExecution)
                {
                    m_StopExecution = true;

                    // the worker thread will call uninitialise and close the task queue handle when it's safe
                }
            }
        }

        public static void XTaskQueueDispatch(UInt32 timeoutMs = 0)
        {
            if (isInitialized)
            {
                XGRInterop.XTaskQueueDispatch(defaultQueue.handle, XTaskQueuePort.Completion, timeoutMs);
            }
        }
    }
}
