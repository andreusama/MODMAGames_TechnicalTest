using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    public interface IDownloadableContentFinder
    {
        public void EnumerateDLC(Action<List<string>> callback);
    }
}
