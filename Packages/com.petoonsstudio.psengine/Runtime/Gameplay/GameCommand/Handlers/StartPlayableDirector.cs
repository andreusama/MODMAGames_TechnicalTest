using KBCore.Refs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class StartPlayableDirector : GameCommandReceiver
    {
        [SerializeField, Self] private PlayableDirector m_Director;

        [SerializeField] private UnityEvent m_OnDirectorPlay;
        [SerializeField] private UnityEvent m_OnDirectorFinish;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Director.played += OnCutsceneStart;
            m_Director.stopped += OnCutsceneEnd;
        }

        protected override void OnDisable()
        {
            base.OnEnable();

            m_Director.played -= OnCutsceneStart;
            m_Director.stopped -= OnCutsceneEnd;
        }

        private void OnCutsceneStart(PlayableDirector obj)
        {
            m_OnDirectorPlay?.Invoke();
        }

        private void OnCutsceneEnd(PlayableDirector obj)
        {
            if (!Application.isPlaying)
                return;

            m_OnDirectorFinish?.Invoke();

            EndAction();
        }

        public override void Execute()
        {
            if (m_Director)
                m_Director.Play();
            else
                EndAction();
        }
    }
}
