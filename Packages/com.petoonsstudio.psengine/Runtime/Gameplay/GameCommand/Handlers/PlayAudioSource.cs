using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PetoonsStudio.PSEngine.Gameplay
{
    public class PlayAudioSource : GameCommandReceiver
    {
        [SerializeField] private AudioSource[] m_AudioSources;

        public override void Execute()
        {
            foreach (var a in m_AudioSources)
                a.Play();

            EndAction();
        }
    }
}
