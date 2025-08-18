using System;

namespace Unity.GameCore
{
    public class XblSocialRelationship
    {
        internal XblSocialRelationship(Interop.XblSocialRelationship interopHandle)
        {
            this.XboxUserId = interopHandle.xboxUserId;
            this.IsFavourite = interopHandle.isFavorite;
            this.IsFollowingCaller = interopHandle.isFollowingCaller;
            this.SocialNetworks = interopHandle.GetSocialNetworks();
        }

        public UInt64 XboxUserId { get; }

        public bool IsFavourite { get; }

        public bool IsFollowingCaller { get; }

        public string[] SocialNetworks { get; }
    }
}
