using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblMultiplayerPerformQoSMeasurementsArgs
    {
        internal XblMultiplayerPerformQoSMeasurementsArgs(Interop.XblMultiplayerPerformQoSMeasurementsArgs interopStruct)
        {
            this.RemoteClients = interopStruct.GetRemoteClients(x => new XblMultiplayerConnectionAddressDeviceTokenPair(x));
        }

        public XblMultiplayerConnectionAddressDeviceTokenPair[] RemoteClients { get; }
    }
}