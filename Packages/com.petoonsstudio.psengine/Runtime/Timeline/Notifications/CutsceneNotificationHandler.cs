using PetoonsStudio.PSEngine.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class CutsceneNotificationHandler
    {
        public virtual void ResolveNotification(Playable origin, INotification notification, object context)
        {
            if (!(notification is IHandledNotification)) return;

            var handledNotification = notification as IHandledNotification;
            handledNotification.Resolve(origin, notification, context);
        }
    }
}
