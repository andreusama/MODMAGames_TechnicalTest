using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace PetoonsStudio.PSEngine.EnGUI
{
    [Serializable]
    public class SubtitleMessage
    {
        public float MinTime;
        public LocalizedString Message;
    }

    public abstract class GUISubtitles : MonoBehaviour
    {
        [SerializeField]
        private float m_ExtraTime = 1f;

        public void NewSubtitle(SubtitleMessage message)
        {
            StopAllCoroutines();
            StartCoroutine(NewSubtitleInternal(message));
        }

        private IEnumerator NewSubtitleInternal(SubtitleMessage message)
        {
            var asyncOperation = message.Message.GetLocalizedStringAsync();
            yield return asyncOperation;

            if (!asyncOperation.IsValid() || string.IsNullOrEmpty(asyncOperation.Result))
            {
                Debug.Log($"Invalid text operation {message.Message.TableReference}");
                yield break;
            }

            ShowText(asyncOperation.Result);

            yield return new WaitForSeconds(message.MinTime + m_ExtraTime);

            HideText();
        }

        public abstract void ShowText(string text);

        public abstract void HideText();
    }
}

