using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef bool CALLBACK XSpeechSynthesizerInstalledVoicesCallback(
    //_In_ const XSpeechSynthesizerVoiceInformation* information,
    //_In_ void* context
    //);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate NativeBool XSpeechSynthesizerInstalledVoicesCallback(ref XSpeechSynthesizerVoiceInformation information, IntPtr context);

    partial class XGRInterop
    {
        //STDAPI XSpeechSynthesizerEnumerateInstalledVoices(
        //    _In_opt_ void* context,
        //    _In_ XSpeechSynthesizerInstalledVoicesCallback* callback 
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XSpeechSynthesizerEnumerateInstalledVoices(
            IntPtr context,
            XSpeechSynthesizerInstalledVoicesCallback callback);

        //STDAPI XSpeechSynthesizerCreate(
        //    _Out_ XSpeechSynthesizerHandle* speechSynthesizer
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XSpeechSynthesizerCreate(
            out XSpeechSynthesizerHandle speechSynthesizer);

        //STDAPI XSpeechSynthesizerCloseHandle(
        //    _In_opt_ XSpeechSynthesizerHandle speechSynthesizer
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XSpeechSynthesizerCloseHandle(
            XSpeechSynthesizerHandle speechSynthesizer);

        //STDAPI XSpeechSynthesizerSetDefaultVoice(
        //    _In_ XSpeechSynthesizerHandle speechSynthesizer
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XSpeechSynthesizerSetDefaultVoice(
            XSpeechSynthesizerHandle speechSynthesizer);

        //STDAPI XSpeechSynthesizerSetCustomVoice(
        //    _In_ XSpeechSynthesizerHandle speechSynthesizer,
        //    _In_z_ const char* voiceId
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XSpeechSynthesizerSetCustomVoice(
            XSpeechSynthesizerHandle speechSynthesizer,
            Byte[] voiceId);

        //STDAPI XSpeechSynthesizerCreateStreamFromText(
        //    _In_ XSpeechSynthesizerHandle speechSynthesizer,
        //    _In_z_ const char* text,
        //    _Out_ XSpeechSynthesizerStreamHandle* speechSynthesisStream
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XSpeechSynthesizerCreateStreamFromText(
            XSpeechSynthesizerHandle speechSynthesizer,
            Byte[] text,
            out XSpeechSynthesizerStreamHandle speechSynthesisStream);

        //STDAPI XSpeechSynthesizerCreateStreamFromSsml(
        //    _In_ XSpeechSynthesizerHandle speechSynthesizer,
        //    _In_z_ const char* ssml,
        //    _Out_ XSpeechSynthesizerStreamHandle* speechSynthesisStream
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XSpeechSynthesizerCreateStreamFromSsml(
            XSpeechSynthesizerHandle speechSynthesizer,
            Byte[] ssml,
            out XSpeechSynthesizerStreamHandle speechSynthesisStream);

        //STDAPI XSpeechSynthesizerCloseStreamHandle(
        //    _In_ XSpeechSynthesizerStreamHandle speechSynthesisStream
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XSpeechSynthesizerCloseStreamHandle(
              XSpeechSynthesizerStreamHandle speechSynthesisStream);

        //STDAPI XSpeechSynthesizerGetStreamDataSize(
        //    _In_ XSpeechSynthesizerStreamHandle speechSynthesisStream,
        //    _Out_ size_t* bufferSize
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XSpeechSynthesizerGetStreamDataSize(
            XSpeechSynthesizerStreamHandle speechSynthesisStream,
            out SizeT bufferSize);

        //STDAPI XSpeechSynthesizerGetStreamData(
        //    _In_ XSpeechSynthesizerStreamHandle speechSynthesisStream,
        //    _In_ size_t bufferSize,
        //    _Out_writes_to_(bufferSize, * bufferUsed) void* buffer,
        //    _Out_opt_ size_t* bufferUsed
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XSpeechSynthesizerGetStreamData(
            XSpeechSynthesizerStreamHandle speechSynthesisStream,
            SizeT bufferSize,
            [Out] Byte[] buffer,
            out SizeT bufferUsed);
    }
}
