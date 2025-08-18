using System;
using System.Collections.Generic;
using System.Linq;

namespace PetoonsStudio.PSEngine.Multiplatform.Standalone
{
    public class StandaloneDownloadableContent : IDownloadableContentFinder
    {
        public void EnumerateDLC(Action<List<string>> callback)
        {
            callback(DownloadableContentTable.Instance.Keys.ToList());
        }
    }
}
