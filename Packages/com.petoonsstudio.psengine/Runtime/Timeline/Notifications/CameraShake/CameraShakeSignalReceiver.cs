using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Rendering;
using PetoonsStudio.PSEngine.Utils;
using UnityEngine;
using UnityEngine.Playables;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class CameraShakeSignalReceiver : MonoBehaviour, INotificationReceiver
    {
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is CameraShakeSignalEmitter emitter)
            {
                var camera = Camera.main;

                if (camera == null)
                    return;

                if (camera.TryGetComponent(out ICameraController controller))
                {
                    controller.Shake(emitter.Preset);
                }
            }
        }
    }
}