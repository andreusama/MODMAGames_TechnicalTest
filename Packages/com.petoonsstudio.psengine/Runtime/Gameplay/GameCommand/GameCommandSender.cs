using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class GameCommandSender : MonoBehaviour, IGameCommandSender
    {
        public delegate void SendNotification(Transform source);
        public event SendNotification OnNotificationSend;

        public virtual void Send(Transform source)
        {
            OnNotificationSend?.Invoke(source);
        }
    }

    public interface IGameCommandSender
    {
        public abstract void Send(Transform source);
    }
}