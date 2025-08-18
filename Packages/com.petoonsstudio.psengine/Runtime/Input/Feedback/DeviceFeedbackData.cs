using PetoonsStudio.PSEngine.Utils;
using System.Threading;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Input.Feedback
{
    [System.Serializable]
    public class DeviceFeedbackData
    {
        [SerializeField, ReadOnly]
        protected int m_DeviceId;

        public InputFeedbackCommand CurrentTask = null;

        public int DeviceId
        {
            get
            {
                return m_DeviceId;
            }
            protected set
            {
                m_DeviceId = value;
            }
        }
        public DeviceFeedbackData()
        {

        }
        public DeviceFeedbackData(int deviceId)
        {
            m_DeviceId = deviceId;
        }
    }

    [System.Serializable]
    public class ColorDeviceFeedbackData : DeviceFeedbackData, IColorDeviceFeedbackData
    {
        public Color Color { get => m_Color; set => m_Color = value; }
        [SerializeField, ReadOnly]
        protected Color m_Color;

        public ColorDeviceFeedbackData(int deviceId) : base(deviceId)
        {
            m_Color = Color.black;
            m_Color.a = 1f;
        }

    }

    public interface IColorDeviceFeedbackData
    { 
        public Color Color { get; set; }
    }
}
