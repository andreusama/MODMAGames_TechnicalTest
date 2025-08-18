using PetoonsStudio.PSEngine.Utils;
using UnityEngine;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class GUIDebugProfile : EnGUIContent
    {
        public override void OnCancel()
        {
            base.OnCancel();
            EnGUIManager.Instance.RemoveLastContent();
        }

        public void ActiveFPS()
        {
#if PETOONS_DEBUG || UNITY_EDITOR
            var fpsCounter = FindObjectOfType<FPSCounter>();
            if(fpsCounter == null)
            {
                var fpsCounterGO = new GameObject("FPSCounter");
                fpsCounterGO.AddComponent<FPSCounter>();
                DontDestroyOnLoad(fpsCounterGO);
            }
            else
            {
                if (fpsCounter.enabled) fpsCounter.Hide();
                else fpsCounter.Show();
            }
#endif
        }
        public void ActiveMemoryViewer()
        {
            var memoryProfilerViewer = FindObjectOfType<MemoryProfilerViewer>();
            if (memoryProfilerViewer == null)
            {
                var memoryProfilerViewerGO = new GameObject("MemoryProfiler");
                memoryProfilerViewerGO.AddComponent<MemoryProfilerViewer>();
                DontDestroyOnLoad(memoryProfilerViewerGO);
            }
            else
            {
                if (memoryProfilerViewer.enabled) memoryProfilerViewer.Hide();
                else memoryProfilerViewer.Show();
            }
        }
    }
}