using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Input.Feedback
{
    public class SetColorCommand : InputFeedbackCommand
    {
        protected Color m_Color = Color.black;

        public SetColorCommand(DeviceFeedbackData data, Color color):base(data)
        {
            m_Color = color;
        }

        public override void Execute()
        {
            SetColor(m_Color);
            End();
        }
    }
}
