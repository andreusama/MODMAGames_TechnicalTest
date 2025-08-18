using System;
using System.Runtime.InteropServices;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public delegate void XblUserStatisticsGetSingleUserStatisticCompleted(Int32 hresult, XblUserStatisticsResult result);
    public delegate void XblUserStatisticsGetSingleUserStatisticsCompleted(Int32 hresult, XblUserStatisticsResult result);
    public delegate void XblUserStatisticsGetMultipleUserStatisticsCompleted(Int32 hresult, XblUserStatisticsResult[] results);
    public delegate void XblUserStatisticsGetMultipleUserStatisticsForMultipleServiceConfigurationsCompleted(Int32 hresult, XblUserStatisticsResult[] results);

    public partial class SDK
    {
        public partial class XBL
        {
            public delegate void XblUserStatisticsStatisticChangedHandler(XblStatisticChangeEventArgs args);

            public static void XblUserStatisticsGetSingleUserStatisticAsync(
                XblContextHandle xblContextHandle,
                UInt64 xboxUserId,
                string serviceConfigurationId,
                string statisticName,
                XblUserStatisticsGetSingleUserStatisticCompleted completionRoutine)
            {
                if (xblContextHandle == null)
                {
                    completionRoutine(HR.E_INVALIDARG, default(XblUserStatisticsResult));
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    SizeT resultSizeInBytes;
                    Int32 hr = XblInterop.XblUserStatisticsGetSingleUserStatisticResultSize(block, out resultSizeInBytes);
                    if (HR.FAILED(hr))
                    {
                        completionRoutine(hr, default(XblUserStatisticsResult));
                        return;
                    }

                    using (DisposableBuffer buffer = new DisposableBuffer(resultSizeInBytes.ToInt32()))
                    {
                        hr = XblInterop.XblUserStatisticsGetSingleUserStatisticResult(
                            block,
                            resultSizeInBytes,
                            buffer.IntPtr,
                            out IntPtr ptrToBuffer,
                            out SizeT bufferUsed);

                        if (HR.FAILED(hr))
                        {
                            completionRoutine(hr, default(XblUserStatisticsResult));
                            return;
                        }

                        XblUserStatisticsResult result = Converters.PtrToClass<XblUserStatisticsResult, Interop.XblUserStatisticsResult>(ptrToBuffer, r => new XblUserStatisticsResult(r));
                        completionRoutine(hr, result);
                    }
                });

                Int32 hresult = XblInterop.XblUserStatisticsGetSingleUserStatisticAsync(
                    xblContextHandle.InteropHandle,
                    xboxUserId,
                    Converters.StringToNullTerminatedUTF8ByteArray(serviceConfigurationId),
                    Converters.StringToNullTerminatedUTF8ByteArray(statisticName),
                    asyncBlock);

                if (HR.FAILED(hresult))
                {
                    completionRoutine(hresult, default(XblUserStatisticsResult));
                }
            }

            public static void XblUserStatisticsGetSingleUserStatisticsAsync(
                XblContextHandle xblContextHandle,
                UInt64 xboxUserId,
                string serviceConfigurationId,
                string[] statisticNames,
                XblUserStatisticsGetSingleUserStatisticsCompleted completionRoutine)
            {
                if (xblContextHandle == null)
                {
                    completionRoutine(HR.E_INVALIDARG, default(XblUserStatisticsResult));
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    SizeT resultSizeInBytes;
                    Int32 hr = XblInterop.XblUserStatisticsGetSingleUserStatisticsResultSize(block, out resultSizeInBytes);
                    if (HR.FAILED(hr))
                    {
                        completionRoutine(hr, default(XblUserStatisticsResult));
                        return;
                    }

                    using (DisposableBuffer buffer = new DisposableBuffer(resultSizeInBytes.ToInt32()))
                    {
                        hr = XblInterop.XblUserStatisticsGetSingleUserStatisticsResult(
                            block,
                            resultSizeInBytes,
                            buffer.IntPtr,
                            out IntPtr ptrToBuffer,
                            out SizeT bufferUsed);

                        if (HR.FAILED(hr))
                        {
                            completionRoutine(hr, default(XblUserStatisticsResult));
                            return;
                        }

                        XblUserStatisticsResult result = Converters.PtrToClass<XblUserStatisticsResult, Interop.XblUserStatisticsResult>(ptrToBuffer, r => new XblUserStatisticsResult(r));
                        completionRoutine(hr, result);
                    }
                });

                using (DisposableBuffer statisticNamesBuffer = Converters.StringArrayToUTF8StringArray(statisticNames))
                {
                    Int32 hresult = XblInterop.XblUserStatisticsGetSingleUserStatisticsAsync(
                        xblContextHandle.InteropHandle,
                        xboxUserId,
                        Converters.StringToNullTerminatedUTF8ByteArray(serviceConfigurationId),
                        statisticNamesBuffer.IntPtr,
                        new SizeT(statisticNames.Length),
                        asyncBlock);

                    if (HR.FAILED(hresult))
                    {
                        completionRoutine(hresult, default(XblUserStatisticsResult));
                    }
                }
            }

            public static void XblUserStatisticsGetMultipleUserStatisticsAsync(
                XblContextHandle xblContextHandle,
                UInt64[] xboxUserIds,
                string serviceConfigurationId,
                string[] statisticNames,
                XblUserStatisticsGetMultipleUserStatisticsCompleted completionRoutine)
            {
                if (xblContextHandle == null)
                {
                    completionRoutine(HR.E_INVALIDARG, default(XblUserStatisticsResult[]));
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    SizeT resultSizeInBytes;
                    Int32 hr = XblInterop.XblUserStatisticsGetMultipleUserStatisticsResultSize(block, out resultSizeInBytes);
                    if (HR.FAILED(hr))
                    {
                        completionRoutine(hr, default(XblUserStatisticsResult[]));
                        return;
                    }

                    using (DisposableBuffer buffer = new DisposableBuffer(resultSizeInBytes.ToInt32()))
                    {
                        hr = XblInterop.XblUserStatisticsGetMultipleUserStatisticsResult(
                            block,
                            resultSizeInBytes,
                            buffer.IntPtr,
                            out IntPtr ptrToBuffer,
                            out SizeT resultsCount,
                            out SizeT bufferUsed);

                        if (HR.FAILED(hr))
                        {
                            completionRoutine(hr, default(XblUserStatisticsResult[]));
                            return;
                        }

                        XblUserStatisticsResult[] result = Converters.PtrToClassArray(ptrToBuffer, resultsCount, (Interop.XblUserStatisticsResult r) => new XblUserStatisticsResult(r));
                        completionRoutine(hr, result);
                    }
                });

                using (DisposableBuffer statisticNamesBuffer = Converters.StringArrayToUTF8StringArray(statisticNames))
                {
                    Int32 hresult = XblInterop.XblUserStatisticsGetMultipleUserStatisticsAsync(
                        xblContextHandle.InteropHandle,
                        xboxUserIds,
                        new SizeT(xboxUserIds.Length),
                        Converters.StringToNullTerminatedUTF8ByteArray(serviceConfigurationId),
                        statisticNamesBuffer.IntPtr,
                        new SizeT(statisticNames.Length),
                        asyncBlock);

                    if (HR.FAILED(hresult))
                    {
                        completionRoutine(hresult, default(XblUserStatisticsResult[]));
                    }
                }
            }

            public static void XblUserStatisticsGetMultipleUserStatisticsForMultipleServiceConfigurationsAsync(
                XblContextHandle xblContextHandle,
                UInt64[] xboxUserIds,
                XblRequestedStatistics[] requestedServiceConfigurationStatisticsCollection,
                XblUserStatisticsGetMultipleUserStatisticsForMultipleServiceConfigurationsCompleted completionRoutine)
            {
                if (xblContextHandle == null)
                {
                    completionRoutine(HR.E_INVALIDARG, default(XblUserStatisticsResult[]));
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    SizeT resultSizeInBytes;
                    Int32 hr = XblInterop.XblUserStatisticsGetMultipleUserStatisticsForMultipleServiceConfigurationsResultSize(block, out resultSizeInBytes);
                    if (HR.FAILED(hr))
                    {
                        completionRoutine(hr, default(XblUserStatisticsResult[]));
                        return;
                    }

                    using (DisposableBuffer buffer = new DisposableBuffer(resultSizeInBytes.ToInt32()))
                    {
                        hr = XblInterop.XblUserStatisticsGetMultipleUserStatisticsForMultipleServiceConfigurationsResult(
                            block,
                            resultSizeInBytes,
                            buffer.IntPtr,
                            out IntPtr results,
                            out SizeT resultsCount,
                            out SizeT bufferUsed);

                        if (HR.FAILED(hr))
                        {
                            completionRoutine(hr, default(XblUserStatisticsResult[]));
                            return;
                        }

                        XblUserStatisticsResult[] result = Converters.PtrToClassArray(results, resultsCount, (Interop.XblUserStatisticsResult r) => new XblUserStatisticsResult(r));
                        completionRoutine(hr, result);
                    }
                });

                using (DisposableCollection disposableCollection = new DisposableCollection())
                {
                    IntPtr interopRequestedStatistics = Converters.ClassArrayToPtr(
                        requestedServiceConfigurationStatisticsCollection,
                        (request, disposables) => new Interop.XblRequestedStatistics(request, disposables),
                        disposableCollection,
                        out SizeT requestedStatisticsCount);

                    Int32 hresult = XblInterop.XblUserStatisticsGetMultipleUserStatisticsForMultipleServiceConfigurationsAsync(
                        xblContextHandle.InteropHandle,
                        xboxUserIds,
                        Convert.ToUInt32(xboxUserIds.Length),
                        interopRequestedStatistics,
                        requestedStatisticsCount.ToUInt32(),
                        asyncBlock);

                    if (HR.FAILED(hresult))
                    {
                        completionRoutine(hresult, default(XblUserStatisticsResult[]));
                    }
                }
            }

            public static Int32 XblUserStatisticsTrackStatistics(
                XblContextHandle xblContextHandle,
                UInt64[] xboxUserIds,
                string serviceConfigurationId,
                string[] statisticNames
                )
            {
                using (DisposableBuffer statisticNamesBuffer = Converters.StringArrayToUTF8StringArray(statisticNames))
                    return XblInterop.XblUserStatisticsTrackStatistics(
                        xblContextHandle.InteropHandle,
                        xboxUserIds,
                        new SizeT(xboxUserIds.Length),
                        Converters.StringToNullTerminatedUTF8ByteArray(serviceConfigurationId),
                        statisticNamesBuffer.IntPtr,
                        new SizeT(statisticNames.Length)
                        );
            }

            public static Int32 XblUserStatisticsStopTrackingStatistics(
                XblContextHandle xblContextHandle,
                UInt64[] xboxUserIds,
                string serviceConfigurationId,
                string[] statisticNames
                )
            {
                using (DisposableBuffer statisticNamesBuffer = Converters.StringArrayToUTF8StringArray(statisticNames))
                    return XblInterop.XblUserStatisticsStopTrackingStatistics(
                        xblContextHandle.InteropHandle,
                        xboxUserIds,
                        new SizeT(xboxUserIds.Length),
                        Converters.StringToNullTerminatedUTF8ByteArray(serviceConfigurationId),
                        statisticNamesBuffer.IntPtr,
                        new SizeT(statisticNames.Length)
                        );
            }
        }
    }
}
