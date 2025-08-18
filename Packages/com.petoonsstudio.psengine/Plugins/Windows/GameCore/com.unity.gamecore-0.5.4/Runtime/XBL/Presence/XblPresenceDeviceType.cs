using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.GameCore
{
    public enum XblPresenceDeviceType : UInt32
    {
        /// <summary>Unknown device</summary>
        Unknown,

        /// <summary>Windows Phone device</summary>
        WindowsPhone,

        /// <summary>Windows Phone 7 device</summary>
        WindowsPhone7,

        /// <summary>Web device, like Xbox.com</summary>
        Web,

        /// <summary>Xbox360 device</summary>
        Xbox360,

        /// <summary>Games for Windows LIVE device</summary>
        PC,

        /// <summary>Xbox LIVE for Windows device</summary>
        Windows8,

        /// <summary>Xbox One device</summary>
        XboxOne,

        /// <summary>Windows One Core devices</summary>
        WindowsOneCore,

        /// <summary>Windows One Core Mobile devices</summary>
        WindowsOneCoreMobile
    }
}
