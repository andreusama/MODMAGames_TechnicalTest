using UnityEngine;
using CrossSceneReference;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using PetoonsStudio.PSEngine.Utils;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public struct CollectEvent
    {
        public Collectible Go;

        public CollectEvent(Collectible go)
        {
            Go = go;
        }
    }

    [RequireComponent(typeof(GuidComponent))]
    public class Collectible : GameCommandReceiver
    {
        [SerializeField] private MMF_Player m_CollectFeedback;

        public override void Execute()
        {
            PickUp(m_InteractingActor);
            EndAction();
        }

        protected virtual void PickUp(Transform actor)
        {
            if (m_PersistenceParams.ShouldPersist)
                SaveBehaviour();

            PSEventManager.TriggerEvent(new CollectEvent(this));

            m_CollectFeedback.PlayFeedbacks();
        }
    }
}