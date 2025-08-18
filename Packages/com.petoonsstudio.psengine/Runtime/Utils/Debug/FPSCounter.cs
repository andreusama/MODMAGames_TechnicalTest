using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    /// <summary>
    /// Class to see game fps
    /// </summary>
    [AddComponentMenu("Petoons Studio/PSEngine/Utils/Debug/FPS Counter")]
    public class FPSCounter : MonoBehaviour
    {
        public float UpdateInterval = 0.5f;
        public int FontSize = 24;

        private FramerateCounter m_FramerateCounter;
        private int m_SafeZone = 0;
        private float m_Fps = 0;

#if PETOONS_DEBUG || UNITY_EDITOR
        /// <summary>
        /// Start
        /// </summary>
        void Start()
        {
            m_SafeZone = (int)(Screen.width * 0.05f);
            m_FramerateCounter = new FramerateCounter(UpdateInterval);
        }

        /// <summary>
        /// Update
        /// </summary>
        void Update()
        {
            m_Fps = m_FramerateCounter.Update();
        }

        public void Hide()
        {
            enabled = false;
        }

        public void Show()
        {
            enabled = true;
        }

        /// <summary>
        /// On GUI
        /// </summary>
        void OnGUI()
        {
            GUIStyle style = GUI.skin.GetStyle("Label");
            style.fontSize = FontSize;
            style.alignment = TextAnchor.LowerLeft;
            style.wordWrap = false;

            GUIStyle lablestyle = GUI.skin.GetStyle("Box");
            lablestyle.alignment = TextAnchor.UpperRight;

            float height = style.lineHeight + 16;
            Rect frameBox = new Rect(Screen.width - 150, 30, 200 - m_SafeZone, height);
            GUI.Box(frameBox, "fps", lablestyle);
            GUI.Label(frameBox, System.String.Format("{0:F2}", m_Fps));
        }
#endif
    }
}
