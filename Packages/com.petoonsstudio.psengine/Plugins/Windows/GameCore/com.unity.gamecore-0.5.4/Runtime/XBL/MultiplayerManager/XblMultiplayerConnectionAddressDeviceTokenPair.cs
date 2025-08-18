using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblMultiplayerConnectionAddressDeviceTokenPair
    {
        internal XblMultiplayerConnectionAddressDeviceTokenPair(Interop.XblMultiplayerConnectionAddressDeviceTokenPair interopStruct)
        {
            this.ConnectionAddress = interopStruct.connectionAddress.GetString();
            this.DeviceToken = new XblDeviceToken(interopStruct.deviceToken);
        }

        public string ConnectionAddress { get; }
        public XblDeviceToken DeviceToken { get; }
    }
}