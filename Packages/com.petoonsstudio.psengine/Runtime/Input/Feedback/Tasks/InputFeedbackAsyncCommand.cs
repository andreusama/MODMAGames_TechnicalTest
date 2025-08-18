using System.Threading.Tasks;

namespace PetoonsStudio.PSEngine.Input.Feedback
{
    public abstract class InputFeedbackAsyncCommand : InputFeedbackCommand
    {
        protected bool m_Active = false;
        protected bool m_Running = false;

        public const int MILISECONDS_PER_SECOND = 1000;
        public const int MILISECONDS_PER_TICK = 50;
        public const float SECONDS_PER_TICK = (float)MILISECONDS_PER_TICK / (float)MILISECONDS_PER_SECOND;

        public InputFeedbackAsyncCommand(DeviceFeedbackData data):base(data)
        {
        }

        public override void Start()
        {
            m_Active = true;
            m_Running = true;
            base.Start();
        }
        public override void End()
        {
            base.End();
            m_Active = false;
            m_Running = false;
        }

        public virtual async Task Stop()
        {
            m_Active = false;
            while (m_Running)
            {
                await Task.Yield();
            }
        }

        public float RestrictValueByTick(float value)
        {
            return ((value * MILISECONDS_PER_SECOND) - MILISECONDS_PER_TICK) < 0 ? SECONDS_PER_TICK : value;
        }
    }
}
