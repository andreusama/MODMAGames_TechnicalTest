using CrossSceneReference;
using KBCore.Refs;
using MoreMountains.Feedbacks;
using PetoonsStudio.PSEngine.EnGUI;
using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Input;
using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace PetoonsStudio.PSEngine.Timeline
{
    public struct PSCutsceneEvent
    {
        public enum Type { Start, Stop }

        public Type CutsceneType;
        public Cutscene Cutscene;

        public PSCutsceneEvent(Type cutsceneType, Cutscene cutscene)
        {
            CutsceneType = cutsceneType;
            Cutscene = cutscene;
        }
    }

    public class CutsceneData : BehaviourPersistentData
    {
        public int ExecutedTimes;

        public CutsceneData(string guid, int executedTimes) : base(guid)
        {
            ExecutedTimes = executedTimes;
        }
    }

    [RequireComponent(typeof(GuidComponent))]
    [RequireComponent(typeof(PlayableDirector))]
    public class Cutscene : MonoBehaviour, INotificationReceiver, IPersistentBehaviour<CutsceneData>
    {
        public enum StackBehaviour
        {
            Stack, Skip
        }

        [SerializeField] private bool m_StackIfCutsceneRunning = true;
        [SerializeField] private bool m_IsSkippable = true;
        [SerializeField, ConditionalHide("m_IsSkippable")] private bool m_FadeWhenSkip = true;
        [SerializeField] private bool m_IsOneShot = false;

        [Header("Persistence")]
        [SerializeField] private PersistenceParams m_PersistenceParams;

        [Header("Events")]
        public UnityEvent OnCutsceneStart;
        public UnityEvent OnCutsceneStop;

        [Header("Audio")]
        [SerializeField] private MMF_Player m_StartFB;
        [SerializeField] private MMF_Player m_EndFB;

        /// Cache components
        [SerializeField, Self] private PlayableDirector m_Director;
        [SerializeField, Self(Flag.Optional)] private InterfaceRef<ICutsceneBinder>[] m_Binders;
        [SerializeField, Self] private GuidComponent m_Guid;

        /// Variables
        private int m_ExecutedTimes;
        private bool m_HasBeenSkipped;

        /// Accessors
        public int ExecutedTimes => m_ExecutedTimes;
        public bool IsRunning => m_Director.state == PlayState.Playing;
        public PlayableDirector Director => m_Director;
        public bool StackIfCutsceneRunning => m_StackIfCutsceneRunning;
        public bool IsSkippable => m_IsSkippable;
        public bool HasBeenSkipped => m_HasBeenSkipped;

        /// Helpers
        protected CutsceneNotificationHandler m_CutsceneNotificationHandler;

        public bool IsAvailable
        {
            get
            {
                if (m_IsOneShot && m_ExecutedTimes > 0)
                    return false;

                return true;
            }
        }

        public string Guid
        {
            get
            {
                if (PersistenceParams.IsCustomGuid)
                    return PersistenceParams.CustomGuid;
                else
                    return m_Guid.GetGuid().ToString();
            }
        }

        public PersistenceParams PersistenceParams => m_PersistenceParams;

#if UNITY_EDITOR
        [ContextMenu("Play Cutscene")]
        public virtual void PlayCutsceneContext()
        {
            if (!IsAvailable) return;

            if (CutsceneController.InstanceExists)
                CutsceneController.Instance.EnqueueCutscene(this);
        }
#endif

        #region Monobehaviour 
        protected virtual void Awake()
        {
            m_CutsceneNotificationHandler = new();
        }

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        protected virtual void Start()
        {
            if (m_PersistenceParams.ShouldPersist) LoadBehaviour();
        }

        protected virtual void OnEnable()
        {
            m_Director.stopped += OnCutsceneStopped;
        }

        protected virtual void OnDisable()
        {
            m_Director.stopped -= OnCutsceneStopped;
        }

        #endregion

        protected virtual void OnCutsceneStopped(PlayableDirector obj)
        {
            if (InputManager.InstanceExists)
                InputManager.Instance.InputMapsController.RestorePreviousState();

            OnCutsceneStop?.Invoke();

            m_EndFB?.PlayFeedbacks();
        }

        public virtual bool Play()
        {
            if (!IsAvailable) return false;

            if (InputManager.InstanceExists)
                InputManager.Instance.InputMapsController.GoCutsceneState(addToStack: true);

            StartCoroutine(Play_Internal());

            m_HasBeenSkipped = false;

            return true;
        }

        protected virtual IEnumerator Play_Internal()
        {
            if (m_Binders != null)
            {
                foreach (var binder in m_Binders)
                    yield return binder.Value.SetTrackBindings(m_Director);
            }

            OnCutsceneStart?.Invoke();

            m_StartFB?.PlayFeedbacks();

            m_Director.Play();
            m_ExecutedTimes++;

            if (m_PersistenceParams.ShouldPersist) SaveBehaviour();
        }

        public virtual async void Skip()
        {
            m_HasBeenSkipped = true;

            m_Director.RebuildGraph();

            m_Director.time = m_Director.playableAsset.duration;

            await Task.Yield();

            m_Director.Evaluate();

            if (m_FadeWhenSkip)
                StartCoroutine(SkipFade<AlphaFader>());
        }

        protected virtual IEnumerator SkipFade<T>() where T : IFader
        {
            yield return EnGUIManager.Instance.FadeOut<T>(0f);
            yield return null;
            yield return EnGUIManager.Instance.FadeIn<T>(0.1f);
        }

        public virtual void OnNotify(Playable origin, INotification notification, object context)
        {
            m_CutsceneNotificationHandler.ResolveNotification(origin, notification, context);
        }

        #region Persistence

        public virtual void SaveBehaviour()
        {
            if (GameManager.Instance.CurrentGameMode.Progression == null) return;

            GameManager.Instance.CurrentGameMode.Progression.TrySaveBehaviour(new CutsceneData(Guid, m_ExecutedTimes));
        }

        public virtual void LoadBehaviour()
        {
            if (GameManager.Instance.CurrentGameMode.Progression == null) return;

            if (GameManager.Instance.CurrentGameMode.Progression.TryLoadBehaviour(Guid, out CutsceneData data))
            {
                m_ExecutedTimes = data.ExecutedTimes;
            }
        }

        #endregion
    }
}