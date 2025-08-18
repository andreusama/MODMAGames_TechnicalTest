using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Timeline
{
    [Serializable]
    public class SubtitleBehaviour : PlayableBehaviour
    {
        public LocalizedString Text;

        public string TextLocalized;

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);

            if (!Application.isPlaying)
            {
                TextLocalized = $"Table: {Text.TableReference} Entry: { Text.TableEntryReference}";
            }
            else
            {
                Text.StringChanged += Text_StringChanged;
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);

            if (Application.isPlaying)
            {
                Text.StringChanged -= Text_StringChanged;
            }
        }

        private void Text_StringChanged(string value)
        {
            TextLocalized = value;
        }
    }
}