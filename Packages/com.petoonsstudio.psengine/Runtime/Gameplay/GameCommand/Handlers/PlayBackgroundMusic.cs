using MoreMountains.Tools;
using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class PlayBackgroundMusic : GameCommandReceiver
    {
        [Header("Parameters")]
        [SerializeField] protected MMSoundManagerPlayOptions m_MusicOptions;
        [SerializeField] protected AudioClip m_MusicClip;

        public override void Execute()
        {
            MMSoundManagerSoundPlayEvent.Trigger(m_MusicClip, m_MusicOptions);
            EndAction();
        }
    }
}