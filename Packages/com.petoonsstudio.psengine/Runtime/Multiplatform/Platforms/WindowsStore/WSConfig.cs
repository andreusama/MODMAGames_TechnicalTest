#if MICROSOFT_GAME_CORE
using Microsoft.GameCore.Utilities;
#endif
using System.Linq;
using UnityEngine;
using System.Xml.Linq;
using System.Xml;

namespace PetoonsStudio.PSEngine.Multiplatform.WindowsStore
{
    [CreateAssetMenu(menuName = "Petoons Studio/PSEngine/Multiplatform/WindowsStore/Configuration", fileName = "WSConfiguration")]
    public class WSConfig : PlatformBaseConfiguration
    {
        [Header("Aplication")]
        [Header("Changing the SCID here will also change the value in your MicrosoftGame.config")]
        [Tooltip("Service Configuration GUID in the form: 12345678-1234-1234-1234-123456789abc, you can obtain it from portal or provided by the publisher.")]
        [Delayed]
        public string Scid;

        [Tooltip("Will automatically sign the user in after XGameRuntime initialization if checked")]
        public bool SignInOnStart = true;


        private string m_LastScid = string.Empty;

#if MICROSOFT_GAME_CORE
        private bool ValidateGuid(string guid)
        {
            var groups = guid.Split('-');
            if (groups.Length != 5) return false;

            if (!groups.Select(str => str.Length).SequenceEqual(new[] { 8, 4, 4, 4, 12 })) return false;

            if (!guid.All(c => "1234567890abcdef-".Contains(c))) return false;

            return true;
        }

        private void OnValidate()
        {
            if (Scid == m_LastScid) return;

            // Ensure guid formatted with only dashes
            if (Scid.Length != 36 ||
                !ValidateGuid(Scid))
            {
                Debug.LogError("Invalid SCID given");
                Scid = m_LastScid;
                return;
            }

            m_LastScid = Scid;

            var gameConfigDoc = XDocument.Load(GdkUtilities.GameConfigPath);
            try
            {
                var scidNode = (from node in gameConfigDoc.Descendants("ExtendedAttribute")
                                where node.Attribute("Name").Value == "Scid"
                                select node).First();

                scidNode.Attribute("Value").Value = Scid;

                var xmlWriterSettings = new XmlWriterSettings()
                {
                    Indent = true,
                    NewLineOnAttributes = true
                };

                using (XmlWriter xmlWriter = XmlWriter.Create(GdkUtilities.GameConfigPath, xmlWriterSettings))
                {
                    gameConfigDoc.WriteTo(xmlWriter);
                }
            }
            catch
            {
                Debug.LogError("Malformed MicrosoftGame.Config. Try associating with the Micosoft Store again or re-import the plugin.");
            }
        }
#endif
    }
}