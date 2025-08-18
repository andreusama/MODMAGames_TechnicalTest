using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class BuildVersionDisplay : MonoBehaviour
    {
        public int FontSize = 24;
        public float HeightOffset = 150f;
        public float WidthOffset = 400f;
        public string AlternativeText = "Build version: ";

        private int m_SafeZone = 0;

        /// <summary>
        /// Start
        /// </summary>
        void Start()
        {
            m_SafeZone = (int)(Screen.width * 0.05f);
        }

        /// <summary>
        /// On GUI
        /// </summary>
        void OnGUI()
        {
            GUIStyle style = GUI.skin.GetStyle("Label");
            style.fontSize = FontSize;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.LowerLeft;
            style.wordWrap = false;

            float height = style.lineHeight + 16;
            float width = WidthOffset - m_SafeZone;
            GUI.Label(new Rect(Screen.width - width, Screen.height - HeightOffset, width, height), AlternativeText + Application.version);
        }
    }

}
