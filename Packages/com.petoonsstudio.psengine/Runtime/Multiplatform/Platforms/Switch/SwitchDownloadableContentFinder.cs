using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_SWITCH
using nn.aoc;
using nn.fs;

namespace PetoonsStudio.PSEngine.Multiplatform.Switch
{
    public class SwitchDownloadableContentFinder : IDownloadableContentFinder
    {
        public static Dictionary<int, byte[]> mountCacheBufferDictionary = new Dictionary<int, byte[]>();
        //IMPORTANT: The script will use aocRoot{DLCIndex} for the first DLC Mount Point name
        //After the first one will use aocRoot2, aocRoot3...
        //Remember that Switch aocIndex starts with 1
        private static readonly string MountName = "aocRoot";

        public void EnumerateDLC(Action<List<string>> callback)
        {
            List<string> returnKeys = new List<string>();

            callback?.Invoke(LoadAoc());
        }

        private List<string> LoadAoc()
        {
            List<string> returnStrings = new List<string>();

            const int MaxListupCount = 256;
            int[] aocListupBuffer = new int[MaxListupCount];

            int accCount = Aoc.CountAddOnContent();
            int listupCount = Aoc.ListAddOnContent(aocListupBuffer, 0, MaxListupCount);
            Debug.Log($"[ADD_CON] Listed: {listupCount} / Count:{accCount} AdditionalContents:");

            for (int i = 0; i < listupCount; ++i)
            {
                if (MountAoc(aocListupBuffer[i]))
                {
                    returnStrings.Add(aocListupBuffer[i].ToString());
                }
            }

            return returnStrings;
        }

        private bool MountAoc(int aocIndex)
        {
            Debug.Log($"[ADD_CON] Mount AdditionalContent with id: {aocIndex}");

            long cacheSize = 0;
            nn.Result result = AddOnContent.QueryMountCacheSize(ref cacheSize, aocIndex);

            if (!result.IsSuccess())
            {
                result.abortUnlessSuccess();
                return false;
            }

            mountCacheBufferDictionary.Add(aocIndex, new byte[cacheSize]);

            string mountName = DetermineMountName(aocIndex);
            result = AddOnContent.Mount(mountName, aocIndex, mountCacheBufferDictionary[aocIndex], cacheSize);
            if (!result.IsSuccess())
            {
                result.abortUnlessSuccess();
                return false;
            }

            Debug.Log($"[ADD_CON] Mount result: {result.IsSuccess()}, {result.GetDescription()}");
            return true;
        }

        private string DetermineMountName(int aocIndex)
        {
            return MountName + $"{aocIndex}";
        }
    }
}

#endif