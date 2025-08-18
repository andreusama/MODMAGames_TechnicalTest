using System;

namespace Unity.GameCore
{
    public enum XUserChangeEvent : UInt32
    {
        SignedInAgain = 0,
        SigningOut = 1,
        SignedOut = 2,
        Gamertag = 3,
        GamerPicture = 4,
        Privileges = 5,
    }
}
