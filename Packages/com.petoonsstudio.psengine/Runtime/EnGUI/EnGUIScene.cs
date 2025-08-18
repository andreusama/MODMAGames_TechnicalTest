using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;
using UnityEngine;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class EnGUIScene : MonoBehaviour
    {
        [SerializeField] protected MMF_Player m_StartFB;

        [SerializeField] protected EnGUIContent m_DefaultContent;
        [SerializeField] protected bool m_EnableOnStart;

        protected virtual void Awake()
        {
            m_StartFB?.PlayFeedbacks();
        }

        protected virtual void Start()
        {
            if (m_EnableOnStart && m_DefaultContent != null)
                EnGUIManager.Instance.PushContent(m_DefaultContent);
        }
    }
}