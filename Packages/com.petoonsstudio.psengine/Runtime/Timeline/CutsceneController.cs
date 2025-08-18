using CrossSceneReference;
using MoreMountains.Tools;
using PetoonsStudio.PSEngine.EnGUI;
using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class CutsceneController : PersistentSingleton<CutsceneController>
    {
        [SerializeField] private AssetReferenceGameObject m_GUI;

        private Cutscene m_RunningCutscene;
        private Queue<Cutscene> m_PendingCutscenes;
        private GameObject m_CurrentGUI;
        private bool m_AsyncGUIRequest;

        public Cutscene RunningCutscene => m_RunningCutscene;
        public Queue<Cutscene> PendingCutscenes => m_PendingCutscenes;

        public bool IsAvailable
        {
            get
            {
                if (PauseManager.InstanceExists && PauseManager.Instance.GamePaused)
                    return false;

#if PETOONS_DEBUG
                if (DebugManager.InstanceExists && DebugManager.Instance.GamePaused)
                    return false;
#endif

                return true;
            }
        }

        public bool CutsceneRunning => m_RunningCutscene != null;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this) return;

            m_PendingCutscenes = new Queue<Cutscene>();
        }

        void OnDestroy()
        {
            if (Instance != this) return;

            if (m_RunningCutscene != null)
                CutsceneRelease();

            m_PendingCutscenes.Clear();
        }

        public virtual void EnqueueCutscene(Cutscene cutscene)
        {
            if (m_RunningCutscene != null)
            {
                if (cutscene.StackIfCutsceneRunning)
                    m_PendingCutscenes.Enqueue(cutscene);
            }
            else
            {
                StartCoroutine(PlayCutscene(cutscene));
            }
        }

        protected virtual void OnCutsceneStopped(UnityEngine.Playables.PlayableDirector obj)
        {
            obj.stopped -= OnCutsceneStopped;

            if (!Application.isPlaying)
                return;

            if (m_PendingCutscenes.Count > 0)
                StartCoroutine(PlayCutscene(m_PendingCutscenes.Dequeue()));
            else
                CutsceneRelease();
        }

        protected IEnumerator PlayCutscene(Cutscene cutscene)
        {
            if (cutscene == null)
            {
                if (m_PendingCutscenes.Count > 0)
                    yield return PlayCutscene(m_PendingCutscenes.Dequeue());
                else
                    CutsceneRelease();
            }
            else
            {
                m_RunningCutscene = cutscene;

                yield return new WaitUntil(() => IsAvailable);

                var success = m_RunningCutscene.Play();

                if (success)
                {
                    m_RunningCutscene.Director.stopped += OnCutsceneStopped;
                    PSEventManager.TriggerEvent(new PSCutsceneEvent(PSCutsceneEvent.Type.Start, m_RunningCutscene));
                    CreateGUI();
                }
                else
                {
                    if (m_PendingCutscenes.Count > 0)
                        yield return PlayCutscene(m_PendingCutscenes.Dequeue());
                    else
                        CutsceneRelease();
                }
            }
        }

        public virtual void SkipAllCutscenes()
        {
            if (m_RunningCutscene != null)
            {
                if (m_RunningCutscene.IsSkippable)
                    m_RunningCutscene.Skip();
                else
                    return;
            }

            while (m_PendingCutscenes.Count > 0)
            {
                var cutscene = m_PendingCutscenes.Dequeue();

                // We should stop skipping when found a non skippable cutscene in our stack
                if (cutscene.IsSkippable)
                    cutscene.Skip();
                else
                    return;
            }
        }

        public virtual void SkipCurrentCutscene()
        {
            if (m_RunningCutscene != null)
            {
                if (m_RunningCutscene.IsSkippable)
                    m_RunningCutscene.Skip();
            }
        }

        public void CreateGUI()
        {
            if (m_AsyncGUIRequest)
                return;

            if (m_GUI == null || !m_GUI.RuntimeKeyIsValid())
                return;

            StartCoroutine(CreateGUI_Internal());
        }

        private IEnumerator CreateGUI_Internal()
        {
            m_AsyncGUIRequest = true;

            var asyncOp = Addressables.InstantiateAsync(m_GUI, EnGUIManager.Instance.GUICutscenes);

            yield return asyncOp;

            m_CurrentGUI = asyncOp.Result;

            m_AsyncGUIRequest = false;
        }

        public void DestroyGUI()
        {
            if (m_AsyncGUIRequest)
            {
                StopCoroutine(nameof(CreateGUI_Internal));
                return;
            }

            if (m_CurrentGUI != null)
                Addressables.ReleaseInstance(m_CurrentGUI);
        }

        private void CutsceneRelease()
        {
            var cutscene = m_RunningCutscene;

            m_RunningCutscene = null;

            PSEventManager.TriggerEvent(new PSCutsceneEvent(PSCutsceneEvent.Type.Stop, cutscene));
            DestroyGUI();
        }
    }
}