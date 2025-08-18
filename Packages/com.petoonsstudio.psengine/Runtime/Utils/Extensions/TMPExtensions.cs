using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class TMPExtensions
    {
        private static readonly char RICH_TEXT_TAG_START = '<';
        private static readonly char RICH_TEXT_TAG_END = '>';

        public static IEnumerator PlayLetterByLetter(this TMP_Text textBox, string text, float characterDelay, Func<bool> forceComplete = null)
        {
            textBox.text = text;

            // Need to force the text object to be generated so we have valid data to work with right from the start.
            textBox.ForceMeshUpdate();

            TMP_TextInfo textInfo = textBox.textInfo;
            bool writingRichTextTag = false;

            // Set the whole text transparent
            // Not using textBox.color because when game is resized, already shown characters go back to transparent
            textInfo.SetAllVertexsAlpha(0f);

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var characterInfo = textInfo.characterInfo[i];

                if (characterInfo.character.Equals(RICH_TEXT_TAG_START))
                {
                    writingRichTextTag = true;
                }
                else if (characterInfo.character.Equals(RICH_TEXT_TAG_END))
                {
                    writingRichTextTag = false;
                }

                if (forceComplete != null && forceComplete.Invoke())
                {
                    textInfo.SetAllVertexsAlpha(1f);

                    textBox.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                    break;
                }

                if (!characterInfo.isVisible) continue;

                SetVertexColors(textInfo, characterInfo, 1f);

                // Update mesh vertex data one last time.
                textBox.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                if (!writingRichTextTag)
                    yield return new WaitForSeconds(characterDelay);
            }
        }

        private static void SetAllVertexsAlpha(this TMP_TextInfo textInfo, float alpha)
        {
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                SetVertexColors(textInfo, textInfo.characterInfo[i], alpha);
            }
        }

        private static void SetVertexColors(this TMP_TextInfo textInfo, TMP_CharacterInfo characterInfo, float alpha)
        {
            // Get the index of the material used by the current character.
            int materialIndex = characterInfo.materialReferenceIndex;

            // Get the vertex colors of the mesh used by this text element (character or sprite).
            Color32[] newVertexColors;
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = characterInfo.vertexIndex;

            // Get the current character's alpha value.
            byte byteAlpha = (byte)MMMaths.Remap(alpha, 0f, 1f, 0f, 255f);

            // Set new alpha values.
            if (vertexIndex + 3 < newVertexColors.Length)
            {
                newVertexColors[vertexIndex + 0].a = byteAlpha;
                newVertexColors[vertexIndex + 1].a = byteAlpha;
                newVertexColors[vertexIndex + 2].a = byteAlpha;
                newVertexColors[vertexIndex + 3].a = byteAlpha;
            }
        }
    }
}
