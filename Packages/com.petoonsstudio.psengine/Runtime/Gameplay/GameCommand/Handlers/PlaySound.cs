using MoreMountains.Tools;
using PetoonsStudio.PSEngine.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class PlaySound : GameCommandReceiver
    {
        [SerializeField] protected AudioClip m_Audio;
        [SerializeField] protected MMSoundManagerPlayOptions m_Options;

        public override void Execute()
        {
            MMSoundManagerSoundPlayEvent.Trigger(m_Audio, m_Options);

            EndAction();
        }
    }
}