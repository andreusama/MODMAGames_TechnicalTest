using System.Collections;
using UnityEngine;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public interface IFader
    {
        IEnumerator FadeSceneIn(float duration, Color color);

        IEnumerator FadeSceneOut(float duration, Color color);

        IEnumerator FadeSceneIn(float duration);

        IEnumerator FadeSceneOut(float duration);

        void SetFade(float value);

        void SetFade(float value, Color color);

        void StopAllFadeCoroutines();

        bool IsActive();
    }
}