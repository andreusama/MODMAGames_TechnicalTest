using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.GameCore
{
    public class XblAchievementMediaAsset
    {
        internal XblAchievementMediaAsset(Interop.XblAchievementMediaAsset mediaAsset)
        {
            this.Name = mediaAsset.name.GetString();
            this.MediaAssetType = mediaAsset.mediaAssetType;
            this.Url = mediaAsset.url.GetString();
        }

        public string Name { get; }
        public XblAchievementMediaAssetType MediaAssetType { get; }
        public string Url { get; }
    }
}
