using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace PetoonsStudio.PSEngine.Timeline
{
    public interface IHandledNotification : INotification
    {
        public void Resolve(Playable origin, INotification notification, object context);
    }
}
