using System.Threading.Tasks;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Input.Feedback
{
    public abstract class InputFeedbackCommand
    {
        public DeviceFeedbackData Data => m_Data;
        protected DeviceFeedbackData m_Data;

        public string GUID;

        private InputFeedbackCommand() { }
        public InputFeedbackCommand(DeviceFeedbackData data)
        {
            m_Data = data;
            GUID = System.Guid.NewGuid().ToString();
        }

        public virtual void Start()
        {
            m_Data.CurrentTask = this;
            Execute();
        }
        public virtual void End()
        {
            m_Data.CurrentTask = null;
        }

        public abstract void Execute();

        protected virtual void SetColor(Color color)
        {
            InputFeedbackHUB.Instance.SetDeviceColor(Data.DeviceId, color);
        }

        protected virtual void SetVibration(VibrationParams vibrationData)
        {
            InputFeedbackHUB.Instance.SetDeviceVibration(Data.DeviceId, vibrationData);
        }

        protected virtual AudioSource PlayVibration(int userIdex, AudioVibrationParams vibrationParams)
        {
            return InputFeedbackHUB.Instance.PlayDeviceAudioVibration(Data.DeviceId, userIdex, vibrationParams);
        }
    }
}
