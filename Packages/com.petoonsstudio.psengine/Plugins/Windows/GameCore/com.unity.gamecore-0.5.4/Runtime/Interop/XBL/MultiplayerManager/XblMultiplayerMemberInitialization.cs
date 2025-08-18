using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XblMultiplayerMemberInitialization
    //{
    //    uint64_t JoinTimeout;
    //    uint64_t MeasurementTimeout;
    //    uint64_t EvaluationTimeout;
    //    bool ExternalEvaluation;
    //    uint32_t MembersNeededToStart;
    //} XblMultiplayerMemberInitialization;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XblMultiplayerMemberInitialization
    {
        internal readonly UInt64 JoinTimeout;
        internal readonly UInt64 MeasurementTimeout;
        internal readonly UInt64 EvaluationTimeout;
        internal readonly NativeBool ExternalEvaluation;
        internal readonly UInt32 MembersNeededToStart;

        internal XblMultiplayerMemberInitialization(Unity.GameCore.XblMultiplayerMemberInitialization publicObject)
        {
            this.JoinTimeout = publicObject.JoinTimeout;
            this.MeasurementTimeout = publicObject.MeasurementTimeout;
            this.EvaluationTimeout = publicObject.EvaluationTimeout;
            this.ExternalEvaluation = new NativeBool(publicObject.ExternalEvaluation);
            this.MembersNeededToStart = publicObject.MembersNeededToStart;
        }
    }
}