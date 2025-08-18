using System;
using System.Runtime.InteropServices;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public partial class SDK
    {
        public partial class XBL
        {
            public delegate void XblMultiplayerWriteSessionHandleResult(Int32 hresult, XblMultiplayerSessionHandle handle);
            public delegate void XblMultiplayerGetSessionHandleResult(Int32 hresult, XblMultiplayerSessionHandle handle);
            public delegate void XblMultiplayerSessionQueryHandleResult(Int32 hresult, XblMultiplayerSessionQueryResult[] sessions);
            public delegate void XblMultiplayerSetActivityHandleResult(Int32 hresult);
            public delegate void XblMultiplayerCreateSearchHandleResult(Int32 hresult, XblMultiplayerSearchHandle handle);
            public delegate void XblMultiplayerDeleteSearchHandleResult(Int32 hresult);
            public delegate void XblMultiplayerClearActivityHandleResult(Int32 hresult);
            public delegate void XblMultiplayerGetSearchHandlesResult(Int32 hresult, XblMultiplayerSearchHandle[] searchHandles);

            public delegate void XblMultiplayerSessionChangedHandler(XblMultiplayerSessionChangeEventArgs args);
            public delegate void XblMultiplayerSessionSubscriptionLostHandler();
            public delegate void XblMultiplayerConnectionIdChangedHandler();

            public delegate void XblMultiplayerSendInvitesResult(Int32 hresult, XblMultiplayerInviteHandle[] handles);
            public delegate void XblMultiplayerGetActivitiesResult(Int32 hresult, XblMultiplayerActivityDetails[] activities);

            public static XblMultiplayerSessionHandle XblMultiplayerSessionCreateHandle(
                ulong xboxUserId,
                XblMultiplayerSessionReference sessionRef,
                XblMultiplayerSessionInitArgs initArgs
                )
            {
                using (DisposableCollection disposableCollection = new DisposableCollection())
                {
                    var interopSessionRef = new Interop.XblMultiplayerSessionReference(sessionRef);
                    var interopInitArgs = new Interop.XblMultiplayerSessionInitArgs(initArgs, disposableCollection);
                    return new XblMultiplayerSessionHandle(XblInterop.XblMultiplayerSessionCreateHandle(xboxUserId, ref interopSessionRef, ref interopInitArgs));
                }
            }

            public static void XblMultiplayerSessionCloseHandle(
                XblMultiplayerSessionHandle handle
                )
            {
                if (handle != null)
                {
                    XblInterop.XblMultiplayerSessionCloseHandle(handle.InteropHandle);
                }
            }

            public static DateTime XblMultiplayerSessionTimeOfSession(
                XblMultiplayerSessionHandle handle
                )
            {
                if (handle == null)
                    return default;

                TimeT interop = XblInterop.XblMultiplayerSessionTimeOfSession(handle.InteropHandle);

                return interop.DateTime;
            }

            public static XblMultiplayerSessionInitializationInfo XblMultiplayerSessionGetInitializationInfo(
                XblMultiplayerSessionHandle handle
                )
            {
                unsafe
                {
                    Interop.XblMultiplayerSessionInitializationInfo* interop = null;
                    if (handle != null)
                    {
                        interop = XblInterop.XblMultiplayerSessionGetInitializationInfo(handle.InteropHandle);
                    }

                    if (interop == null)
                    {
                        return null;
                    }

                    return new XblMultiplayerSessionInitializationInfo(*interop);
                }
            }

            public static XblMultiplayerSessionChangeTypes XblMultiplayerSessionSubscribedChangeTypes(
                XblMultiplayerSessionHandle handle
                )
            {
                if (handle == null)
                    return XblMultiplayerSessionChangeTypes.None;

                return XblInterop.XblMultiplayerSessionSubscribedChangeTypes(handle.InteropHandle);
            }

            public static Int32 XblMultiplayerSessionHostCandidates(
                XblMultiplayerSessionHandle handle,
                out XblDeviceToken[] deviceTokens
                )
            {
                deviceTokens = null;
                if (handle == null)
                    return HR.E_INVALIDARG;

                Int32 hresult = XblInterop.XblMultiplayerSessionHostCandidates(handle.InteropHandle,
                    out IntPtr deviceTokensInterop,
                    out SizeT deviceTokensCount);

                if (HR.SUCCEEDED(hresult))
                {
                    deviceTokens = Converters.PtrToClassArray<XblDeviceToken, Interop.XblDeviceToken>(deviceTokensInterop, deviceTokensCount, x => new XblDeviceToken(x));
                }

                return hresult;
            }

            public static XblMultiplayerSessionReference XblMultiplayerSessionSessionReference(
                XblMultiplayerSessionHandle handle
                )
            {
                unsafe
                {
                    Interop.XblMultiplayerSessionReference* interop = null;
                    if (handle != null)
                    {
                        interop = XblInterop.XblMultiplayerSessionSessionReference(handle.InteropHandle);
                    }

                    if (interop == null)
                    {
                        return null;
                    }

                    return new XblMultiplayerSessionReference(*interop);
                }
            }

            public static XblMultiplayerSessionConstants XblMultiplayerSessionSessionConstants(
                XblMultiplayerSessionHandle handle
                )
            {
                if (handle == null)
                {
                    return null;
                }

                unsafe
                {
                    var interop = XblInterop.XblMultiplayerSessionSessionConstants(handle.InteropHandle);

                    if (interop == null)
                        return null;

                    return new XblMultiplayerSessionConstants(*interop);
                }
            }

            public static void XblMultiplayerSessionConstantsSetMaxMembersInSession(
                XblMultiplayerSessionHandle handle,
                UInt32 maxMembersInSession
                )
            {
                if (handle != null)
                {
                    XblInterop.XblMultiplayerSessionConstantsSetMaxMembersInSession(
                        handle.InteropHandle,
                        maxMembersInSession
                        );
                }
            }

            public static void XblMultiplayerSessionConstantsSetVisibility(
                XblMultiplayerSessionHandle handle,
                XblMultiplayerSessionVisibility visibility
                )
            {
                if (handle != null)
                {
                    XblInterop.XblMultiplayerSessionConstantsSetVisibility(
                        handle.InteropHandle,
                        visibility
                        );
                }
            }

            public static Int32 XblMultiplayerSessionConstantsSetTimeouts(
                XblMultiplayerSessionHandle handle,
                UInt64 memberReservedTimeout,
                UInt64 memberInactiveTimeout,
                UInt64 memberReadyTimeout,
                UInt64 sessionEmptyTimeout
                )
            {
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                return XblInterop.XblMultiplayerSessionConstantsSetTimeouts(
                    handle.InteropHandle,
                    memberReservedTimeout,
                    memberInactiveTimeout,
                    memberReadyTimeout,
                    sessionEmptyTimeout
                    );
            }

            public static Int32 XblMultiplayerSessionConstantsSetQosConnectivityMetrics(
                XblMultiplayerSessionHandle handle,
                bool enableLatencyMetric,
                bool enableBandwidthDownMetric,
                bool enableBandwidthUpMetric,
                bool enableCustomMetric
                )
            {
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                return XblInterop.XblMultiplayerSessionConstantsSetQosConnectivityMetrics(
                    handle.InteropHandle,
                    new NativeBool(enableLatencyMetric),
                    new NativeBool(enableBandwidthDownMetric),
                    new NativeBool(enableBandwidthUpMetric),
                    new NativeBool(enableCustomMetric)
                    );
            }

            public static Int32 XblMultiplayerSessionConstantsSetMemberInitialization(
                XblMultiplayerSessionHandle handle,
                XblMultiplayerMemberInitialization memberInitialization
                )
            {
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                return XblInterop.XblMultiplayerSessionConstantsSetMemberInitialization(
                    handle.InteropHandle,
                    new Interop.XblMultiplayerMemberInitialization(memberInitialization)
                    );
            }

            public static Int32 XblMultiplayerSessionConstantsSetPeerToPeerRequirements(
                XblMultiplayerSessionHandle handle,
                XblMultiplayerPeerToPeerRequirements requirements
                )
            {
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                return XblInterop.XblMultiplayerSessionConstantsSetPeerToPeerRequirements(
                    handle.InteropHandle,
                    new Interop.XblMultiplayerPeerToPeerRequirements(requirements)
                    );
            }

            public static Int32 XblMultiplayerSessionConstantsSetMeasurementServerAddressesJson(
                XblMultiplayerSessionHandle handle,
                string measurementServerAddressesJson
                )
            {
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                return XblInterop.XblMultiplayerSessionConstantsSetMeasurementServerAddressesJson(
                    handle.InteropHandle,
                    Converters.StringToNullTerminatedUTF8ByteArray(measurementServerAddressesJson)
                    );
            }

            public static Int32 XblMultiplayerSessionConstantsSetCapabilities(
                XblMultiplayerSessionHandle handle,
                XblMultiplayerSessionCapabilities capabilities
                )
            {
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                return XblInterop.XblMultiplayerSessionConstantsSetCapabilities(
                    handle.InteropHandle,
                    capabilities == null? default : new Interop.XblMultiplayerSessionCapabilities(capabilities)
                    );
            }

            public static XblMultiplayerSessionProperties XblMultiplayerSessionSessionProperties(
                XblMultiplayerSessionHandle handle
                )
            {
                if (handle == null)
                {
                    return null;
                }

                unsafe
                {
                    var interop = XblInterop.XblMultiplayerSessionSessionProperties(handle.InteropHandle);

                    if (interop == null)
                        return null;

                    return new XblMultiplayerSessionProperties(*interop);
                }
            }

            public static Int32 XblMultiplayerSessionPropertiesSetKeywords(
                XblMultiplayerSessionHandle handle,
                string[] keywords
                )
            {
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                using (DisposableBuffer keywordsBuffer = Converters.StringArrayToUTF8StringArray(keywords))
                {
                    return XblInterop.XblMultiplayerSessionPropertiesSetKeywords(handle.InteropHandle, keywordsBuffer.IntPtr, new SizeT(keywords.Length));
                }
            }

            public static void XblMultiplayerSessionPropertiesSetJoinRestriction(
                XblMultiplayerSessionHandle handle,
                XblMultiplayerSessionRestriction joinRestriction
                )
            {
                if (handle == null)
                    return;

                XblInterop.XblMultiplayerSessionPropertiesSetJoinRestriction(handle.InteropHandle, joinRestriction);
            }

            public static void XblMultiplayerSessionPropertiesSetReadRestriction(
                XblMultiplayerSessionHandle handle,
                XblMultiplayerSessionRestriction readRestriction
                )
            {
                if (handle == null)
                    return;

                XblInterop.XblMultiplayerSessionPropertiesSetReadRestriction(handle.InteropHandle, readRestriction);
            }

            public static Int32 XblMultiplayerSessionPropertiesSetTurnCollection(
                XblMultiplayerSessionHandle handle,
                UInt32[] turnCollectionMemberIds
                )
            {
                if (handle != null)
                {
                    return XblInterop.XblMultiplayerSessionPropertiesSetTurnCollection(handle.InteropHandle, turnCollectionMemberIds, new SizeT(turnCollectionMemberIds.Length));
                }

                return HR.E_INVALIDARG;
            }

            public static Int32 XblMultiplayerSessionMembers(
                XblMultiplayerSessionHandle handle,
                out XblMultiplayerSessionMember[] members
                )
            {
                IntPtr interopMembers;
                SizeT membersCount;

                int hr = XblInterop.XblMultiplayerSessionMembers(handle.InteropHandle, out interopMembers, out membersCount);

                if (HR.FAILED(hr) || membersCount.IsZero)
                {
                    members = null;
                    return hr;
                }

                members = Converters.PtrToClassArray<XblMultiplayerSessionMember, Interop.XblMultiplayerSessionMember>(interopMembers, membersCount, (x => new XblMultiplayerSessionMember(x)));
                return hr;
            }

            public static XblMultiplayerMatchmakingServer XblMultiplayerSessionMatchmakingServer(
                XblMultiplayerSessionHandle handle
                )
            {
                if (handle == null)
                {
                    return null;
                }

                unsafe
                {
                    var interop = XblInterop.XblMultiplayerSessionMatchmakingServer(handle.InteropHandle);

                    if (interop == null)
                        return null;

                    return new XblMultiplayerMatchmakingServer(*interop);
                }
            }


            public static XblMultiplayerSessionMember XblMultiplayerSessionCurrentUser(
                XblMultiplayerSessionHandle handle
                )
            {
                if (handle == null)
                {
                    return null;
                }

                unsafe
                {
                    var interop = XblInterop.XblMultiplayerSessionCurrentUser(handle.InteropHandle);

                    if (interop == null)
                        return null;

                    return new XblMultiplayerSessionMember(*interop);
                }
            }

            public static Int32 XblMultiplayerSessionCurrentUserSetRoles(
                XblMultiplayerSessionHandle handle,
                XblMultiplayerSessionMemberRole[] roles
            )
            {
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                using (DisposableCollection disposableCollection = new DisposableCollection())
                {
                    var rolesBuffer = Converters.ClassArrayToPtr(
                        roles,
                        (role, collection) => new Interop.XblMultiplayerSessionMemberRole( role, disposableCollection),
                        disposableCollection,
                        out var rolesCount);

                    return XblInterop.XblMultiplayerSessionCurrentUserSetRoles(handle.InteropHandle, rolesBuffer, rolesCount);
                }
            }

            public static Int32 XblMultiplayerSessionCurrentUserSetEncounters(
                XblMultiplayerSessionHandle handle,
                string[] encounters
                )
            {
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                using (DisposableBuffer encounterssBuffer = Converters.StringArrayToUTF8StringArray(encounters))
                {
                    return XblInterop.XblMultiplayerSessionCurrentUserSetEncounters(handle.InteropHandle, encounterssBuffer.IntPtr, new SizeT(encounters.Length));
                }
            }

            public static Int32 XblMultiplayerSessionCurrentUserSetMembersInGroup(
                XblMultiplayerSessionHandle handle,
                UInt32[] memberIds
            )
            {
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                return XblInterop.XblMultiplayerSessionCurrentUserSetMembersInGroup(handle.InteropHandle, memberIds, new SizeT(memberIds.Length));
            }

            public static Int32 XblMultiplayerSessionCurrentUserSetGroups(
                XblMultiplayerSessionHandle handle,
                string[] groups
                )
            {
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                using (DisposableBuffer groupssBuffer = Converters.StringArrayToUTF8StringArray(groups))
                {
                    return XblInterop.XblMultiplayerSessionCurrentUserSetGroups(handle.InteropHandle, groupssBuffer.IntPtr, new SizeT(groups.Length));
                }
            }

            public static Int32 XblMultiplayerSessionCurrentUserSetCustomPropertyJson(
                XblMultiplayerSessionHandle handle,
                string name,
                string valueJson
            )
            {
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                return XblInterop.XblMultiplayerSessionCurrentUserSetCustomPropertyJson(handle.InteropHandle, Converters.StringToNullTerminatedUTF8ByteArray(name), Converters.StringToNullTerminatedUTF8ByteArray(valueJson));
            }

            public static Int32 XblMultiplayerSessionCurrentUserDeleteCustomPropertyJson(
                XblMultiplayerSessionHandle handle,
                string name
            )
            {
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                return XblInterop.XblMultiplayerSessionCurrentUserDeleteCustomPropertyJson(handle.InteropHandle, Converters.StringToNullTerminatedUTF8ByteArray(name));
            }

            public static XblWriteSessionStatus XblMultiplayerSessionWriteStatus(
                XblMultiplayerSessionHandle handle
                )
            {
                return XblInterop.XblMultiplayerSessionWriteStatus(handle.InteropHandle);
            }

            public static Int32 XblMultiplayerSessionJoin(
                XblMultiplayerSessionHandle handle,
                string memberCustomConstantsJson,
                bool initializeRequested,
                bool joinWithActiveStatus
                )
            {
                return XblInterop.XblMultiplayerSessionJoin(handle.InteropHandle, Converters.StringToNullTerminatedUTF8ByteArray(memberCustomConstantsJson), initializeRequested, joinWithActiveStatus);
            }

            public static void XblMultiplayerSessionSetHostDeviceToken(
                XblMultiplayerSessionHandle handle,
                XblDeviceToken hostDeviceToken
                )
            {
                if (handle == null)
                    return;

                XblInterop.XblMultiplayerSessionSetHostDeviceToken(handle.InteropHandle, new Interop.XblDeviceToken(hostDeviceToken));
            }

            public static void XblMultiplayerSessionSetClosed(
                XblMultiplayerSessionHandle handle,
                bool closed
                )
            {
                if (handle != null)
                {
                    XblInterop.XblMultiplayerSessionSetClosed(handle.InteropHandle, closed);
                }
            }

            //public static extern Int32 XblMultiplayerSessionSetSessionChangeSubscription(
            //    XblMultiplayerSessionHandle handle,
            //    XblMultiplayerSessionChangeTypes changeTypes
            //    );
            public static Int32 XblMultiplayerSessionSetSessionChangeSubscription(
                XblMultiplayerSessionHandle handle,
                XblMultiplayerSessionChangeTypes changeTypes
                )
            {
                if (handle != null)
                {
                    return XblInterop.XblMultiplayerSessionSetSessionChangeSubscription(handle.InteropHandle, changeTypes);
                }

                return HR.E_INVALIDARG;
            }

            public static Int32 XblMultiplayerSessionLeave(
                XblMultiplayerSessionHandle handle
                )
            {
                if (handle != null)
                {
                    return XblInterop.XblMultiplayerSessionLeave(handle.InteropHandle);
                }

                return HR.E_INVALIDARG;
            }

            public static Int32 XblMultiplayerSessionCurrentUserSetStatus(
                XblMultiplayerSessionHandle handle,
                XblMultiplayerSessionMemberStatus status
                )
            {
                if (handle != null)
                {
                    return XblInterop.XblMultiplayerSessionCurrentUserSetStatus(handle.InteropHandle, status);
                }

                return HR.E_INVALIDARG;
            }

            public static Int32 XblMultiplayerSessionCurrentUserSetSecureDeviceAddressBase64(
                XblMultiplayerSessionHandle handle,
                string value
                )
            {
                if (handle != null)
                {
                    return XblInterop.XblMultiplayerSessionCurrentUserSetSecureDeviceAddressBase64(handle.InteropHandle, Converters.StringToNullTerminatedUTF8ByteArray(value));
                }

                return HR.E_INVALIDARG;
            }

            public static Int32 XblFormatSecureDeviceAddress(
                string deviceId,
                out string address
                )
            {
                if (deviceId != null)
                {
                    Interop.XblFormattedSecureDeviceAddress result;
                    Int32 hr = XblInterop.XblFormatSecureDeviceAddress(Converters.StringToNullTerminatedUTF8ByteArray(deviceId), out result);
                    address = result.GetValue();
                    return hr;
                }

                address = null;
                return HR.E_INVALIDARG;
            }

            public static void XblMultiplayerSearchHandleCloseHandle(XblMultiplayerSearchHandle handle)
            {
                if (handle != null)
                {
                    XblInterop.XblMultiplayerSearchHandleCloseHandle(handle.InteropHandle);
                }
            }

            public static Int32 XblMultiplayerSearchHandleGetSessionReference(
                XblMultiplayerSearchHandle handle,
                out XblMultiplayerSessionReference sessionRef
                )
            {
                Interop.XblMultiplayerSessionReference interopSessionRef;
                Int32 hr = XblInterop.XblMultiplayerSearchHandleGetSessionReference(handle.InteropHandle, out interopSessionRef);

                if (HR.FAILED(hr))
                {
                    sessionRef = null;
                }
                else
                {
                    sessionRef = new XblMultiplayerSessionReference(interopSessionRef);
                }

                return hr;
            }

            public static Int32 XblMultiplayerSearchHandleGetId(
                XblMultiplayerSearchHandle handle,
                out string id
                )
            {
                int hr = XblInterop.XblMultiplayerSearchHandleGetId(handle.InteropHandle, out UTF8StringPtr interopId);

                if (HR.FAILED(hr))
                {
                    id = null;
                }
                else
                {
                    id = interopId.GetString();
                }

                return hr;
            }

            public static Int32 XblMultiplayerSearchHandleGetSessionOwnerXuids(
                XblMultiplayerSearchHandle handle,
                out ulong[] xuids
                )
            {
                IntPtr interopXuids;
                SizeT xuidsCount;

                int hr = XblInterop.XblMultiplayerSearchHandleGetSessionOwnerXuids(handle.InteropHandle, out interopXuids, out xuidsCount);

                if (HR.FAILED(hr) || xuidsCount.IsZero)
                {
                    xuids = null;
                    return hr;
                }

                xuids = Converters.PtrToClassArray<ulong, ulong>(interopXuids, xuidsCount.ToUInt32(), (x => x));
                return hr;
            }

            public static Int32 XblMultiplayerSearchHandleGetTags(
                XblMultiplayerSearchHandle handle,
                out XblMultiplayerSessionTag[] tags
                )
            {
                IntPtr interopTags;
                SizeT tagsCount;

                int hr = XblInterop.XblMultiplayerSearchHandleGetTags(handle.InteropHandle, out interopTags, out tagsCount);

                if (HR.FAILED(hr) || tagsCount.IsZero)
                {
                    tags = null;
                    return hr;
                }

                tags = Converters.PtrToClassArray<XblMultiplayerSessionTag, Interop.XblMultiplayerSessionTag>(interopTags, tagsCount, (x => new XblMultiplayerSessionTag(x)));
                return hr;
            }

            public static Int32 XblMultiplayerSearchHandleGetStringAttributes(
                XblMultiplayerSearchHandle handle,
                out XblMultiplayerSessionStringAttribute[] attributes
                )
            {
                IntPtr interopAttributes;
                SizeT attributesCount;

                int hr = XblInterop.XblMultiplayerSearchHandleGetStringAttributes(handle.InteropHandle, out interopAttributes, out attributesCount);

                if (HR.FAILED(hr) || attributesCount.IsZero)
                {
                    attributes = null;
                    return hr;
                }

                attributes = Converters.PtrToClassArray<XblMultiplayerSessionStringAttribute, Interop.XblMultiplayerSessionStringAttribute>(interopAttributes, attributesCount, (x => new XblMultiplayerSessionStringAttribute(x)));
                return hr;
            }

            public static Int32 XblMultiplayerSearchHandleGetNumberAttributes(
                XblMultiplayerSearchHandle handle,
                out XblMultiplayerSessionNumberAttribute[] attributes
                )
            {
                IntPtr interopAttributes;
                SizeT attributesCount;

                int hr = XblInterop.XblMultiplayerSearchHandleGetNumberAttributes(handle.InteropHandle, out interopAttributes, out attributesCount);

                if (HR.FAILED(hr) || attributesCount.IsZero)
                {
                    attributes = null;
                    return hr;
                }

                attributes = Converters.PtrToClassArray<XblMultiplayerSessionNumberAttribute, Interop.XblMultiplayerSessionNumberAttribute>(interopAttributes, attributesCount, (x => new XblMultiplayerSessionNumberAttribute(x)));
                return hr;
            }

            public static Int32 XblMultiplayerSearchHandleGetVisibility(
                XblMultiplayerSearchHandle handle,
                out XblMultiplayerSessionVisibility visibility
                )
            {
                Int32 hr = XblInterop.XblMultiplayerSearchHandleGetVisibility(handle.InteropHandle, out visibility);

                return hr;
            }

            public static Int32 XblMultiplayerSearchHandleGetJoinRestriction(
                XblMultiplayerSearchHandle handle,
                out XblMultiplayerSessionRestriction joinRestriction
                )
            {
                Int32 hr = XblInterop.XblMultiplayerSearchHandleGetJoinRestriction(handle.InteropHandle, out joinRestriction);

                return hr;
            }

            public static Int32 XblMultiplayerSearchHandleGetSessionClosed(
                XblMultiplayerSearchHandle handle,
                out bool closed
                )
            {
                Int32 hr = XblInterop.XblMultiplayerSearchHandleGetSessionClosed(handle.InteropHandle, out closed);

                return hr;
            }

            public static Int32 XblMultiplayerSearchHandleGetSessionClosed(
                XblMultiplayerSearchHandle handle,
                out uint maxMembers,
                out uint currentMembers
                )
            {
                maxMembers = default;
                currentMembers = default;
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                int hr = XblInterop.XblMultiplayerSearchHandleGetSessionClosed(handle.InteropHandle, out SizeT interopMaxMembers, out SizeT interopCurrentMembers);
                if (HR.SUCCEEDED(hr))
                {
                    maxMembers = interopMaxMembers.ToUInt32();
                    currentMembers = interopCurrentMembers.ToUInt32();
                }

                return hr;
            }

            public static Int32 XblMultiplayerSearchHandleGetCreationTime(
                XblMultiplayerSearchHandle handle,
                out DateTime creationTime
                )
            {
                creationTime = default(DateTime);
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                int hr = XblInterop.XblMultiplayerSearchHandleGetCreationTime(handle.InteropHandle, out TimeT creationTimeT);
                if (HR.SUCCEEDED(hr))
                {
                    creationTime = creationTimeT.DateTime;
                }

                return hr;
            }

            public static Int32 XblMultiplayerSearchHandleGetCustomSessionPropertiesJson(
                XblMultiplayerSearchHandle handle,
                out string customPropertiesJson
                )
            {
                customPropertiesJson = default;
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                int hr = XblInterop.XblMultiplayerSearchHandleGetCustomSessionPropertiesJson(handle.InteropHandle, out UTF8StringPtr interopCustomPropertiesJson);
                if (HR.SUCCEEDED(hr))
                {
                    customPropertiesJson = interopCustomPropertiesJson.GetString();
                }

                return hr;
            }

            public static void XblMultiplayerWriteSessionAsync(
                XblContextHandle xblContext,
                XblMultiplayerSessionHandle handle,
                XblMultiplayerSessionWriteMode writeMode,
                XblMultiplayerWriteSessionHandleResult completionRoutine
                )
            {
                if (xblContext == null || handle == null)
                {
                    completionRoutine(HR.E_INVALIDARG, default(XblMultiplayerSessionHandle));
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    Interop.XblMultiplayerSessionHandle result;
                    Int32 hresult = XblInterop.XblMultiplayerWriteSessionResult(block, out result);
                    if (HR.FAILED(hresult))
                    {
                        completionRoutine(hresult, default(XblMultiplayerSessionHandle));
                        return;
                    }

                    completionRoutine(hresult, new XblMultiplayerSessionHandle(result));
                });

                Int32 hr = XblInterop.XblMultiplayerWriteSessionAsync(
                    xblContext.InteropHandle,
                    handle.InteropHandle,
                    writeMode,
                    asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, default(XblMultiplayerSessionHandle));
                }
            }

            public static void XblMultiplayerWriteSessionByHandleAsync(
                XblContextHandle xblContext,
                XblMultiplayerSessionHandle handle,
                XblMultiplayerSessionWriteMode writeMode,
                string handleId,
                XblMultiplayerWriteSessionHandleResult completionRoutine
                )
            {
                if (xblContext == null || handle == null || handleId == null)
                {
                    completionRoutine(HR.E_INVALIDARG, default(XblMultiplayerSessionHandle));
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    Interop.XblMultiplayerSessionHandle result;
                    Int32 hresult = XblInterop.XblMultiplayerWriteSessionByHandleResult(block, out result);
                    if (HR.FAILED(hresult))
                    {
                        completionRoutine(hresult, default(XblMultiplayerSessionHandle));
                        return;
                    }

                    completionRoutine(hresult, new XblMultiplayerSessionHandle(result));
                });

                Int32 hr = XblInterop.XblMultiplayerWriteSessionByHandleAsync(
                    xblContext.InteropHandle,
                    handle.InteropHandle,
                    writeMode,
                    Converters.StringToNullTerminatedUTF8ByteArray(handleId),
                    asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, default(XblMultiplayerSessionHandle));
                }
            }

            public static void XblMultiplayerGetSessionAsync(
                XblContextHandle xblContext,
                XblMultiplayerSessionReference sessionRef,
                XblMultiplayerGetSessionHandleResult completionRoutine
                )
            {
                if (xblContext == null || sessionRef == null)
                {
                    completionRoutine(HR.E_INVALIDARG, default(XblMultiplayerSessionHandle));
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    Interop.XblMultiplayerSessionHandle result;
                    Int32 hresult = XblInterop.XblMultiplayerGetSessionResult(block, out result);
                    if (HR.FAILED(hresult))
                    {
                        completionRoutine(hresult, default(XblMultiplayerSessionHandle));
                        return;
                    }

                    completionRoutine(hresult, new XblMultiplayerSessionHandle(result));
                });

                var interopSessionRef = new Interop.XblMultiplayerSessionReference(sessionRef);

                Int32 hr = XblInterop.XblMultiplayerGetSessionAsync(
                    xblContext.InteropHandle,
                    ref interopSessionRef,
                    asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, default(XblMultiplayerSessionHandle));
                }
            }

            public static void XblMultiplayerGetSessionByHandleAsync(
                XblContextHandle xblContext,
                string handleId,
                XblMultiplayerGetSessionHandleResult completionRoutine
                )
            {
                if (xblContext == null || handleId == null)
                {
                    completionRoutine(HR.E_INVALIDARG, default(XblMultiplayerSessionHandle));
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    Interop.XblMultiplayerSessionHandle result;
                    Int32 hresult = XblInterop.XblMultiplayerGetSessionByHandleResult(block, out result);
                    if (HR.FAILED(hresult))
                    {
                        completionRoutine(hresult, default(XblMultiplayerSessionHandle));
                        return;
                    }

                    completionRoutine(hresult, new XblMultiplayerSessionHandle(result));
                });

                Int32 hr = XblInterop.XblMultiplayerGetSessionByHandleAsync(
                    xblContext.InteropHandle,
                    Converters.StringToNullTerminatedUTF8ByteArray(handleId),
                    asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, default(XblMultiplayerSessionHandle));
                }
            }

            public static void XblMultiplayerQuerySessionsAsync(
                XblContextHandle xblContext,
                XblMultiplayerSessionQuery sessionQuery,
                XblMultiplayerSessionQueryHandleResult completionRoutine
                )
            {
                if (xblContext == null || sessionQuery == null)
                {
                    completionRoutine(HR.E_INVALIDARG, default(XblMultiplayerSessionQueryResult[]));
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    Int32 hr = XblInterop.XblMultiplayerQuerySessionsResultCount(block, out SizeT sessionCount);
                    if (HR.FAILED(hr))
                    {
                        completionRoutine(hr, default(XblMultiplayerSessionQueryResult[]));
                        return;
                    }

                    Interop.XblMultiplayerSessionQueryResult[] sessionHandles = new Interop.XblMultiplayerSessionQueryResult[sessionCount.ToInt32()];
                    hr = XblInterop.XblMultiplayerQuerySessionsResult(block, sessionCount, sessionHandles);
                    if (HR.FAILED(hr))
                    {
                        completionRoutine(HR.E_INVALIDARG, default(XblMultiplayerSessionQueryResult[]));
                        return;
                    }

                    completionRoutine(hr, Array.ConvertAll(sessionHandles, h => new XblMultiplayerSessionQueryResult(h)));
                });

                using (DisposableCollection disposableCollection = new DisposableCollection())
                {
                    var sessionQueryRef = new Interop.XblMultiplayerSessionQuery(sessionQuery, disposableCollection);

                    Int32 hr = XblInterop.XblMultiplayerQuerySessionsAsync(
                        xblContext.InteropHandle,
                        ref sessionQueryRef,
                        asyncBlock);

                    if (HR.FAILED(hr))
                    {
                        AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                        completionRoutine(HR.E_INVALIDARG, default(XblMultiplayerSessionQueryResult[]));
                    }
                }
            }

            public static void XblMultiplayerSetActivityAsync(
                XblContextHandle xblContext,
                XblMultiplayerSessionReference sessionReference,
                XblMultiplayerSetActivityHandleResult completionRoutine
                )
            {
                if (xblContext == null || sessionReference == null)
                {
                    completionRoutine(HR.E_INVALIDARG);
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(SDK.defaultQueue.handle, (XAsyncCompletionRoutine)(block => completionRoutine(XGRInterop.XAsyncGetStatus(block, false))));
                var sessionReferenceInterop = new Interop.XblMultiplayerSessionReference(sessionReference);
                int hr = XblInterop.XblMultiplayerSetActivityAsync(xblContext.InteropHandle, ref sessionReferenceInterop, asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr);
                }
            }

            public static void XblMultiplayerClearActivityAsync(
                XblContextHandle xblContext,
                string scid,
                XblMultiplayerClearActivityHandleResult completionRoutine
                )
            {
                if (xblContext == null || scid == null)
                {
                    completionRoutine(HR.E_INVALIDARG);
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(SDK.defaultQueue.handle, (XAsyncCompletionRoutine)(block => completionRoutine(XGRInterop.XAsyncGetStatus(block, false))));
                int hr = XblInterop.XblMultiplayerClearActivityAsync(xblContext.InteropHandle, Converters.StringToNullTerminatedUTF8ByteArray(scid), asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr);
                }
            }

            public static void XblMultiplayerCreateSearchHandleAsync(
                XblContextHandle xblContext,
                XblMultiplayerSessionReference sessionRef,
                XblMultiplayerSessionTag[] tags,
                XblMultiplayerSessionNumberAttribute[] numberAttributes,
                XblMultiplayerSessionStringAttribute[] stringAttributes,
                XblMultiplayerCreateSearchHandleResult completionRoutine
                )
            {
                if (xblContext == null || sessionRef == null)
                {
                    completionRoutine(HR.E_INVALIDARG, default(XblMultiplayerSearchHandle));
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    Interop.XblMultiplayerSearchHandle result;
                    Int32 hresult = XblInterop.XblMultiplayerCreateSearchHandleResult(block, out result);
                    if (HR.FAILED(hresult))
                    {
                        completionRoutine(hresult, default(XblMultiplayerSearchHandle));
                        return;
                    }

                    completionRoutine(hresult, new XblMultiplayerSearchHandle(result));
                });

                var interopSessionRef = new Interop.XblMultiplayerSessionReference(sessionRef);
                var interopTags = Converters.ConvertArrayToFixedLength(tags, tags.Length, r => new Interop.XblMultiplayerSessionTag(r));
                var interopNumberAttributes = Converters.ConvertArrayToFixedLength(numberAttributes, numberAttributes.Length, r => new Interop.XblMultiplayerSessionNumberAttribute(r));
                var interopStringAttributes = Converters.ConvertArrayToFixedLength(stringAttributes, stringAttributes.Length, r => new Interop.XblMultiplayerSessionStringAttribute(r));

                Int32 hr = XblInterop.XblMultiplayerCreateSearchHandleAsync(
                    xblContext.InteropHandle,
                    ref interopSessionRef,
                    interopTags,
                    new SizeT(interopTags.Length),
                    interopNumberAttributes,
                    new SizeT(interopNumberAttributes.Length),
                    interopStringAttributes,
                    new SizeT(interopStringAttributes.Length),
                    asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, default(XblMultiplayerSearchHandle));
                }
            }

            public static void XblMultiplayerDeleteSearchHandleAsync(
                XblContextHandle xblContext,
                string handleId,
                XblMultiplayerDeleteSearchHandleResult completionRoutine
                )
            {
                if (xblContext == null || handleId == null)
                {
                    completionRoutine(HR.E_INVALIDARG);
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(SDK.defaultQueue.handle, (XAsyncCompletionRoutine)(block => completionRoutine(XGRInterop.XAsyncGetStatus(block, false))));
                int hr = XblInterop.XblMultiplayerDeleteSearchHandleAsync(xblContext.InteropHandle, Converters.StringToNullTerminatedUTF8ByteArray(handleId), asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr);
                }
            }

            public static void XblMultiplayerGetSearchHandlesAsync(
                XblContextHandle xboxLiveContext,
                string scid,
                string sessionTemplateName,
                string orderByAttribute,
                bool orderAscending,
                string searchFilter,
                string socialGroup,
                XblMultiplayerGetSearchHandlesResult completionRoutine)
            {
                if (xboxLiveContext == null)
                {
                    completionRoutine(HR.E_INVALIDARG, new XblMultiplayerSearchHandle[0]);
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(SDK.defaultQueue.handle, (XAsyncCompletionRoutine)(block =>
                {
                    SizeT searchHandleCount;
                    int handlesResultCount = XblInterop.XblMultiplayerGetSearchHandlesResultCount(block, out searchHandleCount);
                    if (HR.FAILED(handlesResultCount) || searchHandleCount.IsZero)
                    {
                        completionRoutine(handlesResultCount, new XblMultiplayerSearchHandle[0]);
                    }
                    else
                    {
                        Interop.XblMultiplayerSearchHandle[] multiplayerSearchHandleArray = new Interop.XblMultiplayerSearchHandle[searchHandleCount.ToInt32()];
                        int hresult = XblInterop.XblMultiplayerGetSearchHandlesResult(block, multiplayerSearchHandleArray, searchHandleCount);
                        if (!HR.FAILED(hresult))
                            completionRoutine(hresult, Array.ConvertAll(multiplayerSearchHandleArray, h => new XblMultiplayerSearchHandle(h)));
                        else
                            completionRoutine(hresult, new XblMultiplayerSearchHandle[0]);
                    }
                }));

                int hr = XblInterop.XblMultiplayerGetSearchHandlesAsync(
                    xboxLiveContext.InteropHandle,
                    Converters.StringToNullTerminatedUTF8ByteArray(scid),
                    Converters.StringToNullTerminatedUTF8ByteArray(sessionTemplateName),
                    Converters.StringToNullTerminatedUTF8ByteArray(orderByAttribute),
                    orderAscending,
                    Converters.StringToNullTerminatedUTF8ByteArray(searchFilter),
                    Converters.StringToNullTerminatedUTF8ByteArray(socialGroup),
                    asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, new XblMultiplayerSearchHandle[0]);
                }
            }

            public static void XblMultiplayerSendInvitesAsync(
                XblContextHandle xboxLiveContext,
                XblMultiplayerSessionReference sessionRef,
                UInt64[] xboxUserIdList,
                UInt32 titleId,
                string contextStringId,
                string customActivationContext,
                XblMultiplayerSendInvitesResult completionRoutine
            )
            {
                if (xboxLiveContext == null)
                {
                    completionRoutine(HR.E_INVALIDARG, new XblMultiplayerInviteHandle[0]);
                    return;
                }

                SizeT xuidsCount = new SizeT(xboxUserIdList?.Length ?? 0);

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    Interop.XblMultiplayerInviteHandle[] handles = new Interop.XblMultiplayerInviteHandle[xuidsCount.ToInt32()];
                    int hresult = XblInterop.XblMultiplayerSendInvitesResult(block, xuidsCount, handles);
                    if (HR.FAILED(hresult))
                    {
                        completionRoutine(hresult, new XblMultiplayerInviteHandle[0]);
                    }
                    else
                    {
                       completionRoutine(hresult, Array.ConvertAll(handles, h => new XblMultiplayerInviteHandle(h)));
                    }
                });

                var interopSessionRef = new Interop.XblMultiplayerSessionReference(sessionRef);
                int hr = XblInterop.XblMultiplayerSendInvitesAsync(
                    xboxLiveContext.InteropHandle,
                    ref interopSessionRef,
                    xboxUserIdList,
                    xuidsCount,
                    titleId,
                    Converters.StringToNullTerminatedUTF8ByteArray(contextStringId),
                    Converters.StringToNullTerminatedUTF8ByteArray(customActivationContext),
                    asyncBlock
                );

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, new XblMultiplayerInviteHandle[0]);
                }
            }

            public static void XblMultiplayerGetActivitiesForSocialGroupAsync(
                XblContextHandle xboxLiveContext,
                string scid,
                UInt64 socialGroupOwnerXuid,
                string socialGroup,
                XblMultiplayerGetActivitiesResult completionRoutine
                )
            {
                if (xboxLiveContext == null)
                {
                    completionRoutine(HR.E_INVALIDARG, new XblMultiplayerActivityDetails[0]);
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    SizeT activityCount;
                    int activitiesResultCount = XblInterop.XblMultiplayerGetActivitiesForSocialGroupResultCount(block, out activityCount);
                    if (HR.FAILED(activitiesResultCount) || activityCount.IsZero)
                    {
                        completionRoutine(activitiesResultCount, new XblMultiplayerActivityDetails[0]);
                    }
                    else
                    {
                        Interop.XblMultiplayerActivityDetails[] activitiesArray = new Interop.XblMultiplayerActivityDetails[activityCount.ToInt32()];
                        int hresult = XblInterop.XblMultiplayerGetActivitiesForSocialGroupResult(block, activityCount, activitiesArray);
                        if (!HR.FAILED(hresult))
                            completionRoutine(hresult, Array.ConvertAll(activitiesArray, h => new XblMultiplayerActivityDetails(h)));
                        else
                            completionRoutine(hresult, new XblMultiplayerActivityDetails[0]);
                    }
                });

                int hr = XblInterop.XblMultiplayerGetActivitiesForSocialGroupAsync(
                    xboxLiveContext.InteropHandle,
                    Converters.StringToNullTerminatedUTF8ByteArray(scid),
                    socialGroupOwnerXuid,
                    Converters.StringToNullTerminatedUTF8ByteArray(socialGroup),
                    asyncBlock
                );

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, new XblMultiplayerActivityDetails[0]);
                }
            }

            public static void XblMultiplayerGetActivitiesWithPropertiesForSocialGroupAsync(
                XblContextHandle xboxLiveContext,
                string scid,
                UInt64 socialGroupOwnerXuid,
                string socialGroup,
                XblMultiplayerGetActivitiesResult completionRoutine
                )
            {
                if (xboxLiveContext == null)
                {
                    completionRoutine(HR.E_INVALIDARG, new XblMultiplayerActivityDetails[0]);
                    return;
                }

                int hr;

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    hr = XblInterop.XblMultiplayerGetActivitiesWithPropertiesForSocialGroupResultSize(
                        block,
                        out SizeT resultSizeInBytes
                        );

                    if (HR.FAILED(hr) || resultSizeInBytes.IsZero)
                    {
                        completionRoutine(hr, new XblMultiplayerActivityDetails[0]);
                        return;
                    }

                    using (DisposableBuffer buffer = new DisposableBuffer(resultSizeInBytes.ToInt32()))
                    {
                        hr = XblInterop.XblMultiplayerGetActivitiesWithPropertiesForSocialGroupResult(
                            block,
                            resultSizeInBytes,
                            buffer.IntPtr,
                            out IntPtr ptrToResults,
                            out SizeT resultCount,
                            out SizeT bufferUsed
                        );

                        if (HR.FAILED(hr))
                        {
                            completionRoutine(hr, null);
                            return;
                        }

                        completionRoutine(hr, Converters.PtrToClassArray<XblMultiplayerActivityDetails, Interop.XblMultiplayerActivityDetails>(ptrToResults, resultCount, r => new XblMultiplayerActivityDetails(r)));
                    }
                });

                hr = XblInterop.XblMultiplayerGetActivitiesWithPropertiesForSocialGroupAsync(
                    xboxLiveContext.InteropHandle,
                    Converters.StringToNullTerminatedUTF8ByteArray(scid),
                    socialGroupOwnerXuid,
                    Converters.StringToNullTerminatedUTF8ByteArray(socialGroup),
                    asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, new XblMultiplayerActivityDetails[0]);
                }
            }

            public static void XblMultiplayerGetActivitiesForUsersAsync(
                XblContextHandle xboxLiveContext,
                string scid,
                UInt64[] xuids,
                XblMultiplayerGetActivitiesResult completionRoutine
                )
            {
                if (xboxLiveContext == null)
                {
                    completionRoutine(HR.E_INVALIDARG, new XblMultiplayerActivityDetails[0]);
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    SizeT activityCount;
                    int activitiesResultCount = XblInterop.XblMultiplayerGetActivitiesForUsersResultCount(block, out activityCount);
                    if (HR.FAILED(activitiesResultCount) || activityCount.IsZero)
                    {
                        completionRoutine(activitiesResultCount, new XblMultiplayerActivityDetails[0]);
                    }
                    else
                    {
                        Interop.XblMultiplayerActivityDetails[] activitiesArray = new Interop.XblMultiplayerActivityDetails[activityCount.ToInt32()];
                        int hresult = XblInterop.XblMultiplayerGetActivitiesForUsersResult(block, activityCount, activitiesArray);
                        if (!HR.FAILED(hresult))
                            completionRoutine(hresult, Array.ConvertAll(activitiesArray, h => new XblMultiplayerActivityDetails(h)));
                        else
                            completionRoutine(hresult, new XblMultiplayerActivityDetails[0]);
                    }
                });

                int hr = XblInterop.XblMultiplayerGetActivitiesForUsersAsync(
                    xboxLiveContext.InteropHandle,
                    Converters.StringToNullTerminatedUTF8ByteArray(scid),
                    xuids,
                    new SizeT(xuids?.Length ?? 0),
                    asyncBlock
                );

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, new XblMultiplayerActivityDetails[0]);
                }
            }

            public static void XblMultiplayerGetActivitiesWithPropertiesForUsersAsync(
                XblContextHandle xboxLiveContext,
                string scid,
                UInt64[] xuids,
                XblMultiplayerGetActivitiesResult completionRoutine
                )
            {
                if (xboxLiveContext == null)
                {
                    completionRoutine(HR.E_INVALIDARG, new XblMultiplayerActivityDetails[0]);
                    return;
                }

                int hr;

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    hr = XblInterop.XblMultiplayerGetActivitiesWithPropertiesForUsersResultSize(
                        block,
                        out SizeT resultSizeInBytes
                        );

                    if (HR.FAILED(hr))
                    {
                        completionRoutine(hr, new XblMultiplayerActivityDetails[0]);
                        return;
                    }

                    using (DisposableBuffer buffer = new DisposableBuffer(resultSizeInBytes.ToInt32()))
                    {
                        hr = XblInterop.XblMultiplayerGetActivitiesWithPropertiesForUsersResult(
                            block,
                            resultSizeInBytes,
                            buffer.IntPtr,
                            out IntPtr ptrToResults,
                            out SizeT resultCount,
                            out SizeT bufferUsed
                        );

                        if (HR.FAILED(hr))
                        {
                            completionRoutine(hr, null);
                            return;
                        }

                        completionRoutine(hr, Converters.PtrToClassArray<XblMultiplayerActivityDetails, Interop.XblMultiplayerActivityDetails>(ptrToResults, resultCount, r => new XblMultiplayerActivityDetails(r)));
                    }
                });

                hr = XblInterop.XblMultiplayerGetActivitiesWithPropertiesForUsersAsync(
                    xboxLiveContext.InteropHandle,
                    Converters.StringToNullTerminatedUTF8ByteArray(scid),
                    xuids,
                    new SizeT(xuids?.Length ?? 0),
                    asyncBlock
                    );

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, new XblMultiplayerActivityDetails[0]);
                }
            }

            public static Int32 XblMultiplayerSetSubscriptionsEnabled(
                XblContextHandle xblContext,
                bool subscriptionsEnabled
                )
            {
                return XblInterop.XblMultiplayerSetSubscriptionsEnabled(xblContext.InteropHandle, subscriptionsEnabled);
            }

            public static bool XblMultiplayerSubscriptionsEnabled(
                XblContextHandle xblHandle
                )
            {
                return XblInterop.XblMultiplayerSubscriptionsEnabled(xblHandle.InteropHandle);
            }

            public static Int32 XblMultiplayerSessionSetCustomPropertyJson(
                XblMultiplayerSessionHandle handle,
                string name,
                string valueJson
                )
            {
                return XblInterop.XblMultiplayerSessionSetCustomPropertyJson(handle.InteropHandle, Converters.StringToNullTerminatedUTF8ByteArray(name), Converters.StringToNullTerminatedUTF8ByteArray(valueJson));
            }

            public static Int32 XblMultiplayerSessionDeleteCustomPropertyJson(
                XblMultiplayerSessionHandle handle,
                string name
                )
            {
                return XblInterop.XblMultiplayerSessionDeleteCustomPropertyJson(handle.InteropHandle, Converters.StringToNullTerminatedUTF8ByteArray(name));
            }

            public static XblMultiplayerSessionChangeTypes XblMultiplayerSessionCompare(
                XblMultiplayerSessionHandle currentSessionHandle,
                XblMultiplayerSessionHandle oldSessionHandle
                )
            {
                if (currentSessionHandle == null || oldSessionHandle == null)
                    return XblMultiplayerSessionChangeTypes.Everything;

                return XblInterop.XblMultiplayerSessionCompare(currentSessionHandle.InteropHandle, oldSessionHandle.InteropHandle);
            }
        }
    }
}
