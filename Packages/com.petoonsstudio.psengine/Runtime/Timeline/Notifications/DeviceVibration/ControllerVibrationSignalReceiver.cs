using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Input.Feedback;
using PetoonsStudio.PSEngine.Utils;
using UnityEngine;
using UnityEngine.Playables;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class ControllerVibrationSignalReceiver : MonoBehaviour, INotificationReceiver
    {
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is ControllerVibrationSignalEmitter emitter)
            {
                if (VibrationManager.InstanceExists)
                    VibrationManager.Instance.Vibrate(emitter.Preset, emitter.PlayerIndex);
            }
        }
    }
}