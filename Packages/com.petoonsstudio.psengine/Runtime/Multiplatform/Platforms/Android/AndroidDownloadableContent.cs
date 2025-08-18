using System;
using System.Collections.Generic;
using System.Linq;

namespace PetoonsStudio.PSEngine.Multiplatform.Android
{
    public class AndroidDownloadableContent : IDownloadableContentFinder
    {
        public void EnumerateDLC(Action<List<string>> callback)
        {
            callback(DownloadableContentTable.Instance.Keys.ToList());
        }
    }
}
