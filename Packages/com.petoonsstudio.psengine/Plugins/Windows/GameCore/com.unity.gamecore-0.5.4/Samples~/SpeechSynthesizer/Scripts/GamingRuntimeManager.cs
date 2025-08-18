using System;
using UnityEngine;
using Unity.GameCore;
using System.Threading;

namespace SpeechSynthesizer
{
    public class GamingRuntimeManager : MonoBehaviour
    {
        public SpeechSynthesizerManager SpeechSynthesizerManager
        {
            get { return m_SpeechSynthesizerManager; }
        }

        public SpeechSynthesizeSceneManager SpeechSynthesizeSceneManager;

        public static GamingRuntimeManager Instance { get { return m_Instance; } }

        private void Awake()
        {
            if (m_Instance != null && m_Instance != this)
            {
                Destroy(this);
                return;
            }
            else
            {
                m_Instance = this;
                DontDestroyOnLoad(this);

                // initialise the runtime
                Int32 hr = SDK.XGameRuntimeInitialize();
                if (hr == 0)
                {
                    // start the async task dispatch thread
                    m_StopExecution = false;
                    m_DispatchJob = new Thread(DispatchGXDKTaskQueue) { Name = "GXDK Task Queue Dispatch" };
                    m_DispatchJob.Start();
                }

                if (hr != 0)
                {
                    // something went wrong
                    Debug.Log("Error initialising the gaming runtime, hresult: " + hr);
                }
            }
        }

        private void OnDestroy()
        {
            if (m_Instance == this)
            {
                m_Instance = null;
            }
        }

        SpeechSynthesizerManager m_SpeechSynthesizerManager;

        Thread m_DispatchJob;
        bool m_StopExecution;
        static GamingRuntimeManager m_Instance;

        void DispatchGXDKTaskQueue()
        {
            // this will execute all GXDK async work
            while (m_StopExecution == false)
            {
                SDK.XTaskQueueDispatch(0);
                Thread.Sleep(32);
            }
        }
    }
}