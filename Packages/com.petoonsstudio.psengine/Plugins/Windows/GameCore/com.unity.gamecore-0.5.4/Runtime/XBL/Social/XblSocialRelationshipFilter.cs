using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.GameCore
{
    //enum class XblSocialRelationshipFilter : uint32_t
    //{
    //    All,
    //    Favorite,
    //    LegacyXboxLiveFriends
    //};

    public enum XblSocialRelationshipFilter : UInt32
    {
        All = 0,
        Favorite,
        LegacyXboxLiveFriends
    }
}
