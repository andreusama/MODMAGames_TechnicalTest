using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef void
    //(CALLBACK* HCWebSocketMessageFunction) (
    //     _In_ HCWebsocketHandle websocket,
    //     _In_z_ const char* incomingBodyString,
    //     _In_ void* functionContext
    //    );
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void HCWebSocketMessageFunction(HCWebsocketHandle websocket, IntPtr incomingBodyString, IntPtr functionContext);

    //typedef void
    //(CALLBACK* HCWebSocketBinaryMessageFunction) (
    //     _In_ HCWebsocketHandle websocket,
    //     _In_reads_bytes_(payloadSize) const uint8_t* payloadBytes,
    //     _In_ uint32_t payloadSize,
    //     _In_ void* functionContext
    //    );
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void HCWebSocketBinaryMessageFunction(HCWebsocketHandle websocket, IntPtr payloadBytes, UInt32 payloadSize, IntPtr functionContext);

    //typedef void
    //(CALLBACK* HCWebSocketCloseEventFunction) (
    //     _In_ HCWebsocketHandle websocket,
    //     _In_ HCWebSocketCloseStatus closeStatus,
    //     _In_ void* functionContext
    //    );
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void HCWebSocketCloseEventFunction(HCWebsocketHandle websocket, HCWebSocketCloseStatus closeStatus, IntPtr functionContext);

    //typedef void
    //(STDAPIVCALLTYPE* HCWebSocketRoutedHandler) (
    //     _In_ HCWebsocketHandle websocket,
    //     _In_ bool receiving,
    //     _In_opt_z_ const char* message,
    //     _In_opt_ const uint8_t* payloadBytes,
    //     _In_ size_t payloadSize,
    //     _In_opt_ void* context
    //    );
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void HCWebSocketRoutedHandler(HCWebsocketHandle websocket, NativeBool receiving, [Optional] IntPtr message, [Optional] IntPtr payloadBytes, SizeT payloadSize, IntPtr conext);

    // Interop definitions for functions defined in httpClient.h
    partial class XGRInterop
    {
        // STDAPI HCInitialize(_In_opt_ HCInitArgs* args) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 HCInitialize(IntPtr args);

        // STDAPI HCCleanupAsync(XAsyncBlock* async) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 HCCleanupAsync(XAsyncBlockPtr asyncBlock);

        //STDAPI HCWebSocketCreate(
        //    _Out_ HCWebsocketHandle* websocket,
        //    _In_opt_ HCWebSocketMessageFunction messageFunc,
        //    _In_opt_ HCWebSocketBinaryMessageFunction binaryMessageFunc,
        //    _In_opt_ HCWebSocketCloseEventFunction closeFunc,
        //    _In_opt_ void* functionContext
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 HCWebSocketCreate(out HCWebsocketHandle websocket, [Optional] HCWebSocketMessageFunction messageFunc, [Optional] HCWebSocketBinaryMessageFunction binaryMessageFunc, [Optional] HCWebSocketCloseEventFunction closeFunc, IntPtr functionContext);

        //STDAPI HCWebSocketSetProxyUri(
        //    _In_ HCWebsocketHandle websocket,
        //    _In_z_ const char* proxyUri
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 HCWebSocketSetProxyUri(HCWebsocketHandle websocket, Byte[] proxyUri);

        //STDAPI HCWebSocketSetHeader(
        //    _In_ HCWebsocketHandle websocket,
        //    _In_z_ const char* headerName,
        //    _In_z_ const char* headerValue
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 HCWebSocketSetHeader(HCWebsocketHandle websocket, Byte[] headerName, Byte[] headerValue);

        //STDAPI HCWebSocketGetEventFunctions(
        //    _In_ HCWebsocketHandle websocket,
        //    _Out_opt_ HCWebSocketMessageFunction* messageFunc,
        //    _Out_opt_ HCWebSocketBinaryMessageFunction* binaryMessageFunc,
        //    _Out_opt_ HCWebSocketCloseEventFunction* closeFunc,
        //    _Out_ void** functionContext
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 HCWebSocketGetEventFunctions(HCWebsocketHandle websocket, [Optional] out HCWebSocketMessageFunction messageFunc, [Optional] out HCWebSocketBinaryMessageFunction binaryMessageFunc, [Optional] out HCWebSocketCloseEventFunction closeFunc, out IntPtr functionContext);

        //STDAPI HCWebSocketConnectAsync(
        //    _In_z_ const char* uri,
        //    _In_z_ const char* subProtocol,
        //    _In_ HCWebsocketHandle websocket,
        //    _Inout_ XAsyncBlock* asyncBlock
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 HCWebSocketConnectAsync(Byte[] uri, Byte[] subProtocol, HCWebsocketHandle websocket, XAsyncBlockPtr asyncBlock);

        //STDAPI HCGetWebSocketConnectResult(
        //    _Inout_ XAsyncBlock* asyncBlock,
        //    _In_ WebSocketCompletionResult* result
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 HCGetWebSocketConnectResult(XAsyncBlockPtr asyncBlock, [In] ref WebSocketCompletionResult result);

        //STDAPI HCWebSocketSendMessageAsync(
        //    _In_ HCWebsocketHandle websocket,
        //    _In_z_ const char* message,
        //    _Inout_ XAsyncBlock* asyncBlock
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 HCWebSocketSendMessageAsync(HCWebsocketHandle websocket, Byte[] message, XAsyncBlockPtr asyncBlock);

        //STDAPI HCWebSocketSendBinaryMessageAsync(
        //    _In_ HCWebsocketHandle websocket,
        //    _In_reads_bytes_(payloadSize) const uint8_t* payloadBytes,
        //    _In_ uint32_t payloadSize,
        //    _Inout_ XAsyncBlock* asyncBlock
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 HCWebSocketSendBinaryMessageAsync(HCWebsocketHandle websocket, Byte[] data, UInt32 payloadSize, XAsyncBlockPtr asyncBlock);

        //STDAPI HCGetWebSocketSendMessageResult(
        //    _Inout_ XAsyncBlock* asyncBlock,
        //    _In_ WebSocketCompletionResult* result
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 HCGetWebSocketSendMessageResult(XAsyncBlockPtr asyncBlock, [In] ref WebSocketCompletionResult result);

        //STDAPI HCWebSocketDisconnect(
        //    _In_ HCWebsocketHandle websocket
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 HCWebSocketDisconnect(HCWebsocketHandle websocket);

        //STDAPI_(HCWebsocketHandle) HCWebSocketDuplicateHandle(
        //    _In_ HCWebsocketHandle websocket
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern HCWebsocketHandle HCWebSocketDuplicateHandle(HCWebsocketHandle websocket);

        //STDAPI HCWebSocketCloseHandle(
        //    _In_ HCWebsocketHandle websocket
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 HCWebSocketCloseHandle(HCWebsocketHandle websocket);

        //STDAPI_(int32_t) HCAddWebSocketRoutedHandler(
        //    _In_ HCWebSocketRoutedHandler handler,
        //    _In_opt_ void* context
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 HCAddWebSocketRoutedHandler(HCWebSocketRoutedHandler handler, IntPtr conext);

        //STDAPI_(void) HCRemoveWebSocketRoutedHandler(
        //    _In_ int32_t handlerId
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern void HCRemoveWebSocketRoutedHandler(Int32 handlerId);
    }
}
