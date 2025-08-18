using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblMultiplayerMemberInitialization
    {
        internal XblMultiplayerMemberInitialization(Interop.XblMultiplayerMemberInitialization interopStruct)
        {
            this.JoinTimeout = interopStruct.JoinTimeout;
            this.MeasurementTimeout = interopStruct.MeasurementTimeout;
            this.EvaluationTimeout = interopStruct.EvaluationTimeout;
            this.ExternalEvaluation = interopStruct.ExternalEvaluation.Value;
            this.MembersNeededToStart = interopStruct.MembersNeededToStart;
        }

        public UInt64 JoinTimeout { get; }
        public UInt64 MeasurementTimeout { get; }
        public UInt64 EvaluationTimeout { get; }
        public bool ExternalEvaluation { get; }
        public UInt32 MembersNeededToStart { get; }
    }
}