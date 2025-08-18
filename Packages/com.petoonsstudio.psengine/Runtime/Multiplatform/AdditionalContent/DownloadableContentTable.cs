using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PetoonsStudio.PSEngine.Utils;
using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Multiplatform.PS4;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    [CreateAssetMenu(fileName = "AdditionalContentTable", menuName = "Petoons Studio/AdditionalContent Table")]
    public class DownloadableContentTable : AddressablesDatabase<DownloadableContentTable, DownloadableContent>
    {
        private readonly char REGIONAL_SEPARATOR = ';';

        public bool AdditionalContentExists(string platformAdditionalContentKey, out string additionalContentKey)
        {
            additionalContentKey = null;

            foreach(var downloadableContentKey in Keys)
            {
                if(IsSpecificPlatformIDOfAnyInternalDLC(platformAdditionalContentKey, downloadableContentKey))
                {
                    Debug.Log($"[ADD_CON] Additional content found with the specific key: {platformAdditionalContentKey} and general key: {downloadableContentKey}");
                    additionalContentKey = downloadableContentKey;
                    return true;
                }
            }
            Debug.Log($"[ADD_CON] NO additional content found with the specific key: {platformAdditionalContentKey}");
            return false;
        }

        public bool HasAdditionalContent()
        {
            return this.GetEntryCount() > 0;
        }

        public List<string> GetSelectedAddCon(int bitMask)
        {
            List<string> returnValues = new List<string>();

            var currentAddConIDs = Values.ToList();

            int[] setedBits = GetSetBits(bitMask);

            for (int i = 0; i < currentAddConIDs.Count && i < setedBits.Length; i++)
            {
                returnValues.Add(currentAddConIDs[setedBits[i]].ID);
            }

            return returnValues;
        }

        public int[] GetSetBits(int @byte)
        {
            List<int> setedBits = new List<int>();

            int maxSize = sizeof(int) * 8;

            int bitWiser = 1;

            for (int i = 0; i < maxSize; i++)
            {
                if ((@byte & bitWiser) == bitWiser)
                {
                    setedBits.Add(i);
                }
                bitWiser = bitWiser << 1;
            }

            return setedBits.ToArray();
        }

        public bool IsSpecificPlatformIDOfAnyInternalDLC(string dlcPlatformSpecificKey, string internalDLCID)
        {
            var dlc = LoadAsset(internalDLCID);
            var platformID = dlc[PlatformManager.Instance.CurrentPlatform];

#if UNITY_PS4
            var result = dlcPlatformSpecificKey.Equals(platformID.Split(REGIONAL_SEPARATOR)[(int)PS4Manager.Instance.ProductRegion];       
            ReleaseAsset(dlc);
            
            return result;
#else
            var result = dlcPlatformSpecificKey.Equals(platformID);
            ReleaseAsset(dlc);

            return result;
#endif
        }
    }
}