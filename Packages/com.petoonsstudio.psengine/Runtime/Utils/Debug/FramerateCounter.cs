using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    /// <summary>
    /// Class that provides framerate counting in intervals bigger than a frame.
    /// Use with FramerateCounter(float desiredRefreshTime) constructor.
    /// </summary>
    public class FramerateCounter
    {
        private int m_FrameCounter = 0;
        private float m_TimeCounter = 0f;
        private float m_LastFramerate = 0f;
        private float m_RefreshTime = 0.01f;

        public FramerateCounter(float refreshTime)
        {
            m_RefreshTime = Mathf.Max(0.01f, refreshTime);
        }

        /// <summary>
        /// Updates the framerate counter. Returns the current framerate.
        /// </summary>
        /// <returns>Current Framerate</returns>
        public float Update()
        {
            if (m_TimeCounter < m_RefreshTime)
            {
                m_TimeCounter += Time.deltaTime;
                m_FrameCounter++;
            }
            else
            {
                m_LastFramerate = (float)m_FrameCounter / m_TimeCounter;
                m_FrameCounter = 0;
                m_TimeCounter = 0f;
            }

            return m_LastFramerate;
        }
    }
}