using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public delegate void XSpeechSynthesizerInstalledVoicesCallback(XSpeechSynthesizerVoiceInformation information);

    public partial class SDK
    {
        #region Callbacks
        [MonoPInvokeCallback]
        private static NativeBool SpeechSynthesizerInstalledVoicesCallback(ref Interop.XSpeechSynthesizerVoiceInformation information, IntPtr context)
        {
            GCHandle cbHandle = GCHandle.FromIntPtr(context);

            var collection = cbHandle.Target as List<XSpeechSynthesizerVoiceInformation>;
            collection.Add(new XSpeechSynthesizerVoiceInformation(information));

            return new NativeBool(true);
        }
        #endregion

        public static Int32 XSpeechSynthesizerEnumerateInstalledVoices(
            out XSpeechSynthesizerVoiceInformation[] voiceInformation)
        {
            // init to null
            voiceInformation = null;

            List<XSpeechSynthesizerVoiceInformation> localInfos = new List<XSpeechSynthesizerVoiceInformation>();
            GCHandle infoHandle = GCHandle.Alloc(localInfos);

            Int32 hr = XGRInterop.XSpeechSynthesizerEnumerateInstalledVoices(GCHandle.ToIntPtr(infoHandle), SpeechSynthesizerInstalledVoicesCallback);
            voiceInformation = localInfos.ToArray();
            infoHandle.Free();
            return hr;
        }

        public static Int32 XSpeechSynthesizerCreate(
            out XSpeechSynthesizerHandle speechSynthesizer)
        {
            Int32 hr = XGRInterop.XSpeechSynthesizerCreate(out Interop.XSpeechSynthesizerHandle speechSynthesizerHandle);
            speechSynthesizer = new XSpeechSynthesizerHandle(speechSynthesizerHandle);
            return hr;
        }

        public static Int32 XSpeechSynthesizerCloseHandle(
            XSpeechSynthesizerHandle speechSynthesizer)
        {
            if (speechSynthesizer == null)
            {
                return HR.E_INVALIDARG;
            }

            return XGRInterop.XSpeechSynthesizerCloseHandle(speechSynthesizer.InteropHandle);
        }

        public static Int32 XSpeechSynthesizerSetDefaultVoice(
            XSpeechSynthesizerHandle speechSynthesizer)
        {
            if (speechSynthesizer == null)
            {
                return HR.E_INVALIDARG;
            }

            return XGRInterop.XSpeechSynthesizerSetDefaultVoice(speechSynthesizer.InteropHandle);
        }

        public static Int32 XSpeechSynthesizerSetCustomVoice(
            XSpeechSynthesizerHandle speechSynthesizer,
            XSpeechSynthesizerVoiceInformation voiceId)
        {
            if (speechSynthesizer == null)
            {
                return HR.E_INVALIDARG;
            }

            return XGRInterop.XSpeechSynthesizerSetCustomVoice(speechSynthesizer.InteropHandle, Converters.StringToNullTerminatedUTF8ByteArray(voiceId.VoiceId));
        }

        public static Int32 XSpeechSynthesizerCreateStreamFromText(
            XSpeechSynthesizerHandle speechSynthesizer,
            string text,
            out XSpeechSynthesizerStreamHandle speechSynthesisStream)
        {
            // init to null;
            speechSynthesisStream = null;

            if (speechSynthesizer == null || String.IsNullOrEmpty(text))
            {
                return HR.E_INVALIDARG;
            }

            Byte[] bytes = Converters.StringToNullTerminatedUTF8ByteArray(text);

            Interop.XSpeechSynthesizerStreamHandle interopStreamHandle = new Interop.XSpeechSynthesizerStreamHandle();
            Int32 hr = XGRInterop.XSpeechSynthesizerCreateStreamFromText(speechSynthesizer.InteropHandle, bytes, out interopStreamHandle);
            return XSpeechSynthesizerStreamHandle.WrapInteropHandleAndReturnHResult(hr, interopStreamHandle, out speechSynthesisStream);
        }

        public static Int32 XSpeechSynthesizerCreateStreamFromSsml(
            XSpeechSynthesizerHandle speechSynthesizer,
            string ssml,
            out XSpeechSynthesizerStreamHandle speechSynthesisStream)
        {
            // init to null;
            speechSynthesisStream = null;

            if (speechSynthesizer == null || String.IsNullOrEmpty(ssml))
            {
                return HR.E_INVALIDARG;
            }

            Byte[] bytes = Converters.StringToNullTerminatedUTF8ByteArray(ssml);

            Interop.XSpeechSynthesizerStreamHandle interopStreamHandle = new Interop.XSpeechSynthesizerStreamHandle();
            Int32 hr = XGRInterop.XSpeechSynthesizerCreateStreamFromSsml(speechSynthesizer.InteropHandle, bytes, out interopStreamHandle);
            return XSpeechSynthesizerStreamHandle.WrapInteropHandleAndReturnHResult(hr, interopStreamHandle, out speechSynthesisStream);
        }

        public static Int32 XSpeechSynthesizerCloseStreamHandle(
            XSpeechSynthesizerStreamHandle speechSynthesisStream)
        {
            if (speechSynthesisStream == null)
            {
                return HR.E_INVALIDARG;
            }

            return XGRInterop.XSpeechSynthesizerCloseStreamHandle(speechSynthesisStream.InteropHandle);
        }

        public static Int32 XSpeechSynthesizerGetStreamData(
            XSpeechSynthesizerStreamHandle speechSynthesisStream,
            out Byte[] buffer)
        {
            buffer = default(Byte[]);
            if (speechSynthesisStream == null)
            {
                return HR.E_INVALIDARG;
            }

            int hr = XGRInterop.XSpeechSynthesizerGetStreamDataSize(speechSynthesisStream.InteropHandle, out SizeT bufferSize);
            if (HR.SUCCEEDED(hr))
            {
                buffer = new byte[bufferSize.ToInt32()];
                return XGRInterop.XSpeechSynthesizerGetStreamData(speechSynthesisStream.InteropHandle, bufferSize, buffer, out SizeT bufferUsed);
            }
            else
            {
                return hr;
            }
        }
    }
}

