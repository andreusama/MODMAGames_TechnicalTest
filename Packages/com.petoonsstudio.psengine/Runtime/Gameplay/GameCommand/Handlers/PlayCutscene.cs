using PetoonsStudio.PSEngine.Timeline;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class PlayCutscene : GameCommandReceiver
    {
        [SerializeField] protected Cutscene m_Cutscene;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Cutscene.Director.stopped += OnCutsceneEnd;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            m_Cutscene.Director.stopped -= OnCutsceneEnd;
        }

        public override void Execute()
        {
            if (CutsceneController.InstanceExists)
                CutsceneController.Instance.EnqueueCutscene(m_Cutscene);
        }

        protected virtual void OnCutsceneEnd(PlayableDirector cutscene)
        {
            if (!Application.isPlaying)
                return;

            EndAction();
        }
    }
}