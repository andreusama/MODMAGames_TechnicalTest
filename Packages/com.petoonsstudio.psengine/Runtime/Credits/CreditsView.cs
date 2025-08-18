using MoreMountains.Feedbacks;
using PetoonsStudio.PSEngine.EnGUI;
using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Credits
{
    public class CreditsView : EnGUIScene
    {
        public enum CreditsViewMode { VerticalScroll, StaticPages };

        [SerializeField] private float m_InitalTime = 1f;

        [Header("Speed")]
        [SerializeField] private CreditsViewMode m_ViewMode = CreditsViewMode.VerticalScroll;
        [SerializeField] private float m_DefaultSpeed = 1f;
        [SerializeField] private float m_FastSpeed = 4f;
        [SerializeField] private float m_PageTime = 4f;
        [SerializeField] private UnityEvent m_OnSpeedStart;
        [SerializeField] private UnityEvent m_OnSpeedEnd;

        [Header("Skip")]
        [SerializeField] private Image m_SkipImage;
        [SerializeField] private float m_SkipTime = 1f;
        [SerializeField] private UnityEvent m_OnSkipStart;
        [SerializeField] private UnityEvent m_OnSkipEnd;

        [Header("References")]
        [SerializeField] protected AssetReferenceT<SceneGroup> m_MenuScene;
        [SerializeField] protected float m_NextPageMargin = 150f;

        [SerializeField]
        protected CreditsChapterUI[] m_PageViewers;

        public UnityEvent OnFinish;

        protected bool m_IsSpeedUpHold;
        protected float m_SkipCurrentTime;
        protected bool m_IsSkipHold;
        protected bool m_IsExitingCredits;

        public float ScrollSpeed => m_IsSpeedUpHold ? m_FastSpeed : m_DefaultSpeed;
        public float NextPageMargin => m_NextPageMargin;
        public float PageTime => m_PageTime;
        public CreditsViewMode ViewMode => m_ViewMode;

        protected override void Start()
        {
            base.Start();

            StartCoroutine(PlayCredits());
        }

        void OnEnable()
        {
            m_SkipCurrentTime = 0f;
        }

        void Update()
        {
            if (m_IsExitingCredits)
                return;

            if (!m_IsSkipHold)
            {
                m_SkipCurrentTime -= Time.deltaTime;
                if (m_SkipCurrentTime < 0)
                    m_SkipCurrentTime = 0;
            }
            else
            {
                m_SkipCurrentTime += Time.deltaTime;
                if (m_SkipCurrentTime > m_SkipTime)
                    ExitCredits();
            }

            m_SkipImage.fillAmount = MMMaths.Remap(m_SkipCurrentTime, 0f, 1f, 0f, m_SkipTime);
        }

        public void ExitCredits()
        {
            m_IsExitingCredits = true;
            SceneLoaderManager.Instance.LoadSceneGroup(m_MenuScene, loadingScene : false);
        }

        public void UpdateChapters(CreditsChapterUI[] pageViewer)
        {
            m_PageViewers = new CreditsChapterUI[pageViewer.Length];
            m_PageViewers = pageViewer;
        }

        public void OnSpeed(InputValue value)
        {
            if (m_IsSpeedUpHold && !value.isPressed)
                m_OnSpeedEnd?.Invoke();
            else if (!m_IsSpeedUpHold && value.isPressed)
                m_OnSpeedStart?.Invoke();

            m_IsSpeedUpHold = value.isPressed;
        }

        public void OnSkip(InputValue value)
        {
            if (m_IsSkipHold && !value.isPressed)
                m_OnSkipEnd?.Invoke();
            else if (!m_IsSkipHold && value.isPressed)
                m_OnSkipStart?.Invoke();

            m_IsSkipHold = value.isPressed;
        }

        public void ClearChapters()
        {
            m_PageViewers = new CreditsChapterUI[0];
        }

        protected virtual IEnumerator PlayCredits()
        {
            if (m_PageViewers.Length == 0)
                yield break;

            yield return new WaitForSecondsRealtime(m_InitalTime);

            for (int i = 0; i < m_PageViewers.Length; i++)
            {
                yield return StartCoroutine(m_PageViewers[i].ViewPage());
            }
            
            OnFinish?.Invoke();
        }
    }
}
