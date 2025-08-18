using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    /// <summary>
    /// This class displays an on-screen console for easier debugging
    /// DO NOT ADD THIS CLASS AS A COMPONENT.
    /// Instead, use the MMDebug.DebugOnScreen methods that will take care of everything
    /// </summary>
    public class MMConsole : MonoBehaviour
    {
        private Queue<string> m_MessageStack = new Queue<string>();
        private string m_DisplayMessage;

        private int m_LargestMessageLength = 0;

        private int m_MarginBot = 120;
        private int m_MarginLeft = 10;
        private int m_Padding = 10;

        private int m_FontSize = 10;
        private int m_CharacterHeight = 16;
        private int m_CharacterWidth = 6;

        private int m_StackLimit = 5;

        /// <summary>
        /// Draws a box containing the current stack of messages on top of the screen.
        /// </summary>
        void OnGUI()
        {
            // we define the style to use and the font size
            GUIStyle style = GUI.skin.GetStyle("label");
            style.fontSize = m_FontSize;

            // we determine our box dimension based on the number of lines and the length of the longest line
            int boxHeight = m_StackLimit * m_CharacterHeight;
            int boxWidth = m_LargestMessageLength * m_CharacterWidth;

            // we draw a box and the message on top of it
            GUI.Box(new Rect(m_MarginLeft, Screen.height - m_MarginBot, boxWidth + m_Padding * 2, boxHeight + m_Padding * 2), "");
            GUI.Label(new Rect(m_MarginLeft + m_Padding, Screen.height - m_MarginBot + m_Padding, boxWidth, boxHeight), m_DisplayMessage);
        }

        /// <summary>
        /// Sets the size of the font, and automatically deduces the character's height and width.
        /// </summary>
        /// <param name="fontSize">Font size.</param>
        public void SetFontSize(int fontSize)
        {
            Mathf.Clamp(fontSize, 10, 100);

            m_FontSize = fontSize;
            m_CharacterHeight = (int)Mathf.Round(1.6f * fontSize + 0.49f);
            m_CharacterWidth = (int)Mathf.Round(0.6f * fontSize + 0.49f);

        }

        /// <summary>
        /// Replaces the content of the current message stack with the specified string 
        /// </summary>
        /// <param name="newMessage">New message.</param>
        public void SetMessage(string newMessage)
        {
            m_MessageStack.Clear();
            m_MessageStack.Enqueue(newMessage);
        }

        /// <summary>
        /// Adds the specified message to the message stack.
        /// </summary>
        /// <param name="newMessage">New message.</param>
        public void AddMessage(string newMessage)
        {
            if (m_MessageStack.Count >= m_StackLimit)
            {
                m_MessageStack.Dequeue();
            }

            m_DisplayMessage = string.Empty;
            m_MessageStack.Enqueue(newMessage);

            m_LargestMessageLength = 0;
            foreach (var item in m_MessageStack)
            {
                m_DisplayMessage += item + "\n";
                if (item.Length > m_LargestMessageLength)
                {
                    m_LargestMessageLength = item.Length;
                }
            }
        }
    }
}
