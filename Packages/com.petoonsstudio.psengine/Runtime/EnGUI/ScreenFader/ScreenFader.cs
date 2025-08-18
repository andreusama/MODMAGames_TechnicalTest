using KBCore.Refs;
using UnityEngine;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField, Child] private InterfaceRef<IFader>[] m_Faders;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public T GetFader<T>() where T : IFader
        {
            foreach (var fader in m_Faders)
            {
                if (fader.Value is T)
                {
                    return (T)fader.Value;
                }
            }

            return default(T);
        }

        public void DoFadeIn<T>(float duration) where T : IFader
        {
            var fader = GetFader<T>();
            fader.StopAllFadeCoroutines();
            StartCoroutine(fader.FadeSceneIn(duration));
        }

        public void DoFadeOut<T>(float duration) where T : IFader
        {
            var fader = GetFader<T>();
            fader.StopAllFadeCoroutines();
            StartCoroutine(fader.FadeSceneOut(duration));
        }

        public void DoFadeIn<T>(float duration, Color color) where T : IFader
        {
            var fader = GetFader<T>();
            fader.StopAllFadeCoroutines();
            StartCoroutine(fader.FadeSceneIn(duration, color));
        }

        public void DoFadeOut<T>(float duration, Color color) where T : IFader
        {
            var fader = GetFader<T>();
            fader.StopAllFadeCoroutines();
            StartCoroutine(fader.FadeSceneOut(duration, color));
        }

        public void SetFade<T>(float value) where T : IFader
        {
            var fader = GetFader<T>();
            fader.SetFade(value);
        }

        public void SetFade<T>(float value, Color color) where T : IFader
        {
            var fader = GetFader<T>();
            fader.SetFade(value, color);
        }

        public void SetFade(string faderType, float value)
        {
            IFader fader = null;
            foreach (var faderComponent in m_Faders)
            {
                if (faderComponent.GetType().Name == faderType)
                    fader = faderComponent.Value;
            }

            fader.SetFade(value);
        }

        public void SetFade(string faderType, float value, Color color)
        {
            IFader fader = null;
            foreach (var faderComponent in m_Faders)
            {
                if (faderComponent.GetType().Name == faderType)
                    fader = faderComponent.Value;
            }

            fader.SetFade(value, color);
        }
    }
}