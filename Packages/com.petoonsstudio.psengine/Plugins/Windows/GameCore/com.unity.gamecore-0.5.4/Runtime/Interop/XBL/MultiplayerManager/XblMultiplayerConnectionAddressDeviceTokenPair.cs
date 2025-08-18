using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XblMultiplayerConnectionAddressDeviceTokenPair
    //{
    //    _Field_z_ const char* connectionAddress;
    //    _Field_z_ XblDeviceToken deviceToken;
    //} XblMultiplayerConnectionAddressDeviceTokenPair;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XblMultiplayerConnectionAddressDeviceTokenPair
    {
        internal readonly UTF8StringPtr connectionAddress;
        internal readonly XblDeviceToken deviceToken;

        internal XblMultiplayerConnectionAddressDeviceTokenPair(Unity.GameCore.XblMultiplayerConnectionAddressDeviceTokenPair publicObject, DisposableCollection disposableCollection)
        {
            this.connectionAddress = new UTF8StringPtr(publicObject.ConnectionAddress, disposableCollection);
            this.deviceToken = new XblDeviceToken(publicObject.DeviceToken);
        }
    }
}