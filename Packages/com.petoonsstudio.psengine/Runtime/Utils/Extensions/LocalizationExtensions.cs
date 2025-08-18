using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class LocalizationExtensions
    {
        /// <summary>
        /// Release an asset on the AssetDatabase given a LocalizedAudioClip.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="localizedClip"></param>
        public static void ReleaseAsset(this LocalizedAssetDatabase database, LocalizedAudioClip localizedClip)
        {
            if (!localizedClip.IsEmpty)
                ReleaseAsset(database, localizedClip.TableReference, localizedClip.TableEntryReference);
        }

        /// <summary>
        /// Release an asset on the AssetDatabase given a TableReference and a TableEntry.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="tableReference">GUID of the table or the table name.</param>
        /// <param name="entryReference">GUID of the audio or name of the key.</param>
        public static void ReleaseAsset(this LocalizedAssetDatabase database, TableReference tableReference, TableEntryReference entryReference)
        {
            database.GetTable(tableReference).ReleaseAsset(entryReference);
        }
    }
}

