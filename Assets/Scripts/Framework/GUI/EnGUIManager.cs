using KBCore.Refs;
using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EnGUI
{
    public class EnGUIManager : PersistentSingleton<EnGUIManager>
    {
        public enum PreviousBehaviour
        {
            Close, CloseImmediately, DoNothing, Remove
        }

        public enum ContentBehaviour
        {
            Hidden, Open, OpenImmediately
        }

        public interface IRequest { }

        #region Request Types
        private struct PushRequest : IRequest
        {
            public EnGUIContent Content;
            public EventSystem EventSystem;
            public PreviousBehaviour PreviousBehaviour;
            public ContentBehaviour ContentBehaviour;

            public PushRequest(EnGUIContent content, EventSystem eventSystem, PreviousBehaviour prevBehaviour, ContentBehaviour contentBehaviour)
            {
                Content = content;
                EventSystem = eventSystem;
                PreviousBehaviour = prevBehaviour;
                ContentBehaviour = contentBehaviour;
            }
        }

        private struct RemoveLastRequest : IRequest
        {
            public bool Immediately;

            public RemoveLastRequest(bool immediately)
            {
                Immediately = immediately;
            }
        }

        private struct RemoveAllRequest : IRequest
        {
            public bool Immediately;

            public RemoveAllRequest(bool immediately)
            {
                Immediately = immediately;
            }
        }

        private struct RemoveUntilRequest : IRequest
        {
            public EnGUIContent Content;
            public bool Immediately;

            public RemoveUntilRequest(EnGUIContent content, bool immediately)
            {
                Content = content;
                Immediately = immediately;
            }
        }
        #endregion

        [SerializeField] private Transform m_GUISubtitles;
        [SerializeField] private Transform m_GUICutscenes;

        [SerializeField, Child] private ScreenFader m_ScreenFader;

        public ScreenFader ScreenFader => m_ScreenFader;

        private Stack<EnGUIContent> m_Content;
        private Queue<IRequest> m_Requests;

        private bool m_IsProcessing;

        public EnGUIContent CurrentContent => m_Content.TryPeek(out var content) ? content : null;
        public bool IsEmpty => !m_IsProcessing && m_Content.Count == 0;

        public Transform GUISubtitles => m_GUISubtitles;
        public Transform GUICutscenes => m_GUICutscenes;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this) return;

            m_Content = new();
            m_Requests = new();
        }

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void PushContent(EnGUIContent content, PreviousBehaviour prevBehaviour = PreviousBehaviour.Close, EventSystem eventSystem = null, ContentBehaviour cBehaviour = ContentBehaviour.Open)
        {
            m_Requests.Enqueue(new PushRequest(content, eventSystem, prevBehaviour, cBehaviour));
            ProcessRequests();
        }

        /// <summary>
        /// Pushes multiple contents to the stack, but only calls OpenAnimation on the last one.
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="eventSystem"></param>
        public void PushContents(EnGUIContent[] contents, PreviousBehaviour prevBehaviour = PreviousBehaviour.Close, EventSystem eventSystem = null, ContentBehaviour cBehaviour = ContentBehaviour.Open)
        {
            for (int i = 0; i < contents.Length - 1; i++)
            {
                m_Requests.Enqueue(new PushRequest(contents[i], eventSystem, prevBehaviour, ContentBehaviour.Hidden));
            }

            m_Requests.Enqueue(new PushRequest(contents[contents.Length - 1], eventSystem, prevBehaviour, cBehaviour));
            ProcessRequests();
        }

        public void RemoveLastContent(bool immediately = false)
        {
            m_Requests.Enqueue(new RemoveLastRequest(immediately));
            ProcessRequests();
        }

        public void RemoveAllContents(bool immediately = false)
        {
            m_Requests.Enqueue(new RemoveAllRequest(immediately));
            ProcessRequests();
        }

        public void CloseAllUntilFind(EnGUIContent target, bool immediately = false)
        {
            m_Requests.Enqueue(new RemoveUntilRequest(target, immediately));
            ProcessRequests();
        }

        private void ProcessRequests()
        {
            if (m_Requests.Count > 0 && !m_IsProcessing)
                StartCoroutine(ProcessNextRequest());
        }

        private IEnumerator ProcessNextRequest()
        {
            m_IsProcessing = true;

            var request = m_Requests.Peek();

            switch (request)
            {
                case PushRequest pushRequest:
                    yield return PushContent_Internal(pushRequest.Content, pushRequest.EventSystem, pushRequest.PreviousBehaviour, pushRequest.ContentBehaviour);
                    break;
                case RemoveAllRequest removeAllRequest:
                    yield return RemoveAllContents_Internal(removeAllRequest.Immediately);
                    break;
                case RemoveLastRequest removeLastRequest:
                    yield return RemoveLastContent_Internal(removeLastRequest.Immediately);
                    break;
                case RemoveUntilRequest removeUntilRequest:
                    yield return RemoveUntilContent_Internal(removeUntilRequest.Content, removeUntilRequest.Immediately);
                    break;
            }

            m_Requests.Dequeue();
            m_IsProcessing = false;

            ProcessRequests();
        }

        private IEnumerator PushContent_Internal(EnGUIContent content, EventSystem eventSystem, PreviousBehaviour previousBehaviour, ContentBehaviour behaviour)
        {
            if (m_Content.Count > 0 && CurrentContent.HasControl)
            {
                CurrentContent.LoseControl();

                switch (previousBehaviour)
                {
                    case PreviousBehaviour.Close:
                        yield return CurrentContent.CloseAnimation(newContentPushed: true);
                        break;
                    case PreviousBehaviour.CloseImmediately:
                        yield return CurrentContent.CloseImmediately(newContentPushed: true);
                        break;
                    case PreviousBehaviour.Remove:
                        var currentContent = m_Content.Pop();
                        yield return currentContent.CloseImmediately(newContentPushed: false);
                        currentContent.DisableContent();
                        currentContent.RestoreTimeScale();
                        break;
                }
            }

            yield return content.Initialize();

            content.SetTimeScale();

            switch (behaviour)
            {
                case ContentBehaviour.Open:
                    yield return content.OpenAnimation(lastContentRemoved: false);
                    break;
                case ContentBehaviour.OpenImmediately:
                    yield return content.OpenImmediately(lastContentRemoved: false);
                    break;
            }

            if (behaviour != ContentBehaviour.Hidden)
            {
                content.GainControl(eventSystem);
            }

            m_Content.Push(content);
        }

        private IEnumerator RemoveLastContent_Internal(bool immediately)
        {
            if (m_Content.Count == 0)
                yield break;

            var content = m_Content.Pop();

            content.LoseControl();

            if (immediately)
                yield return content.CloseImmediately(false);
            else
                yield return content.CloseAnimation(false);

            content.DisableContent();
            content.RestoreTimeScale();

            if (m_Content.Count == 0)
                yield break;

            content = m_Content.Peek();

            if (immediately)
                yield return content.OpenImmediately(false);
            else
                yield return content.OpenAnimation(false);


            content.GainControl();
        }

        private IEnumerator RemoveUntilContent_Internal(EnGUIContent stopContent, bool immediately)
        {
            if (m_Content.Count == 0)
                yield break;

            bool found = false;
            bool isFirst = true;

            while (!found)
            {
                var content = m_Content.Pop();
                content.LoseControl();

                if (isFirst)
                {
                    if (immediately)
                        yield return content.CloseImmediately(false);
                    else
                        yield return content.CloseAnimation(false);

                    isFirst = false;
                }

                content.DisableContent();
                content.RestoreTimeScale();

                found = stopContent == content;

                if (m_Content.Count == 0)
                    yield break;

                content = m_Content.Peek();

                if (found)
                {
                    if (immediately)
                        yield return content.OpenImmediately(false);
                    else
                        yield return content.OpenAnimation(false);
                }

                content.GainControl();
            }
        }

        private IEnumerator RemoveAllContents_Internal(bool immediately)
        {
            int counter = 0;
            while (m_Content.Count > 0)
            {
                var content = m_Content.Pop();
                content.LoseControl();

                if (counter == 0)
                {
                    if (immediately)
                        yield return content.CloseImmediately(newContentPushed: false);
                    else
                        yield return content.CloseAnimation(newContentPushed: false);
                }

                content.DisableContent();
                content.RestoreTimeScale();

                if (m_Content.Count == 0)
                    yield break;

                content = m_Content.Peek();
                content.GainControl();
                counter++;
            }
        }

        public bool ContentInStack(EnGUIContent content)
        {
            return m_Content.Contains(content);
        }

        public IEnumerator FadeOut<T>(float duration) where T : IFader
        {
            if (!m_ScreenFader.GetFader<T>().IsActive())
            {
                m_ScreenFader.DoFadeOut<T>(duration);
                yield return new WaitForSecondsRealtime(duration);
            }
        }

        public IEnumerator FadeIn<T>(float duration) where T : IFader
        {
            if (m_ScreenFader.GetFader<T>().IsActive())
            {
                m_ScreenFader.DoFadeIn<T>(duration);
                yield return new WaitForSecondsRealtime(duration);
            }
        }

        public void UpdateFader(string type, float value, Color newColor)
        {
            m_ScreenFader.SetFade(type, value, newColor);
        }
    }
}