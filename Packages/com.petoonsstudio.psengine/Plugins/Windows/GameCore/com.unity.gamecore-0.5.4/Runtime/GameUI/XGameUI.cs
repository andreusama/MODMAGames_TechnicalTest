using System;
using System.Runtime.InteropServices;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public delegate void XGameUiShowAchievementsCompleted(Int32 hresult);
    public delegate void XGameUiShowMessageDialogCompleted(Int32 hresult, XGameUiMessageDialogButton resultButton);
    public delegate void XGameUiShowErrorDialogCompleted(Int32 hresult);
    public delegate void XGameUiShowSendGameInviteCompleted(Int32 hresult);
    public delegate void XGameUiShowPlayerProfileCardCompleted(Int32 hresult);
    public delegate void XGameUiShowPlayerPickerCompleted(Int32 hresult, UInt64[] resultPlayers);
    public delegate void XGameUiShowTextEntryCompleted(Int32 hresult, string resultText);
    public delegate void XGameUiShowWebAuthenticationAsyncCompleted(int hresult, XGameUiWebAuthenticationResultData result);

    partial class SDK
    {
        public static void XGameUiShowAchievementsAsync(XUserHandle requestingUser, UInt32 titleId, XGameUiShowAchievementsCompleted completionRoutine)
        {
            if (requestingUser == null)
            {
                completionRoutine(HR.E_INVALIDARG);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                Int32 result = XGRInterop.XGameUiShowAchievementsResult(block);
                completionRoutine(result);
            });

            int hr = XGRInterop.XGameUiShowAchievementsAsync(asyncBlock, requestingUser.InteropHandle, titleId);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr);
            }
        }

        public static void XGameUiShowMessageDialogAsync(
            string titleText,
            string contentText,
            string firstButtonText,
            string secondButtonText,
            string thirdButtonText,
            XGameUiMessageDialogButton defaultButton,
            XGameUiMessageDialogButton cancelButton,
            XGameUiShowMessageDialogCompleted completionRoutine)
        {
            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                Int32 result = XGRInterop.XGameUiShowMessageDialogResult(block, out XGameUiMessageDialogButton resultButton);
                completionRoutine(result, resultButton);
            });

            int hr = XGRInterop.XGameUiShowMessageDialogAsync(
                asyncBlock,
                Converters.StringToNullTerminatedUTF8ByteArray(titleText),
                Converters.StringToNullTerminatedUTF8ByteArray(contentText),
                Converters.StringToNullTerminatedUTF8ByteArray(firstButtonText),
                Converters.StringToNullTerminatedUTF8ByteArray(secondButtonText),
                Converters.StringToNullTerminatedUTF8ByteArray(thirdButtonText),
                defaultButton,
                cancelButton);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, default(XGameUiMessageDialogButton));
            }
        }

        public static void XGameUiShowErrorDialogAsync(Int32 errorCode, string context, XGameUiShowErrorDialogCompleted completionRoutine)
        {
            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                Int32 result = XGRInterop.XGameUiShowErrorDialogResult(block);
                completionRoutine(result);
            });

            int hr = XGRInterop.XGameUiShowErrorDialogAsync(asyncBlock, errorCode, Converters.StringToNullTerminatedUTF8ByteArray(context));

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr);
            }
        }

        public static void XGameUiShowSendGameInviteAsync(
            XUserHandle requestingUser,
            string sessionConfigurationId,
            string sessionTemplateName,
            string sessionId,
            string invitationText,
            string customActivationContext,
            XGameUiShowSendGameInviteCompleted completionRoutine)
        {
            if (requestingUser == null)
            {
                completionRoutine(HR.E_INVALIDARG);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                Int32 result = XGRInterop.XGameUiShowSendGameInviteResult(block);
                completionRoutine(result);
            });

            int hr = XGRInterop.XGameUiShowSendGameInviteAsync(
                asyncBlock,
                requestingUser.InteropHandle,
                Converters.StringToNullTerminatedUTF8ByteArray(sessionConfigurationId),
                Converters.StringToNullTerminatedUTF8ByteArray(sessionTemplateName),
                Converters.StringToNullTerminatedUTF8ByteArray(sessionId),
                Converters.StringToNullTerminatedUTF8ByteArray(invitationText),
                Converters.StringToNullTerminatedUTF8ByteArray(customActivationContext));

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr);
            }
        }

        public static void XGameUiShowPlayerProfileCardAsync(XUserHandle requestingUser, UInt64 targetPlayer, XGameUiShowPlayerProfileCardCompleted completionRoutine)
        {
            if (requestingUser == null)
            {
                completionRoutine(HR.E_INVALIDARG);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                Int32 result = XGRInterop.XGameUiShowPlayerProfileCardResult(block);
                completionRoutine(result);
            });

            int hr = XGRInterop.XGameUiShowPlayerProfileCardAsync(asyncBlock, requestingUser.InteropHandle, targetPlayer);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr);
            }
        }

        public static void XGameUiShowPlayerPickerAsync(
            XUserHandle requestingUser,
            string promptText,
            UInt64[] selectFromPlayers,
            UInt64[] preSelectedPlayers,
            UInt32 minSelectionCount,
            UInt32 maxSelectionCount,
            XGameUiShowPlayerPickerCompleted completionRoutine)
        {
            if (requestingUser == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                // extract result
                UInt64[] resultPlayers = null;
                UInt32 count;
                Int32 hresult = XGRInterop.XGameUiShowPlayerPickerResultCount(block, out count);
                if (HR.SUCCEEDED(hresult))
                {
                    if (count > 0)
                    {
                        resultPlayers = new UInt64[count];
                        UInt32 resultPlayersUsed = 0;
                        hresult = XGRInterop.XGameUiShowPlayerPickerResult(block, count, resultPlayers, out resultPlayersUsed);

                        if (HR.FAILED(hresult))
                        {
                            completionRoutine(hresult, null);
                        }
                    }
                }
                completionRoutine(hresult, resultPlayers);
            });

            Int32 hr = XGRInterop.XGameUiShowPlayerPickerAsync(
                asyncBlock,
                requestingUser.InteropHandle,
                Converters.StringToNullTerminatedUTF8ByteArray(promptText),
                Convert.ToUInt32(selectFromPlayers?.Length ?? 0),
                selectFromPlayers,
                Convert.ToUInt32(preSelectedPlayers?.Length ?? 0),
                preSelectedPlayers,
                minSelectionCount,
                maxSelectionCount);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }

        public static Int32 XGameUiSetNotificationPositionHint(XGameUiNotificationPositionHint position)
        {
            return XGRInterop.XGameUiSetNotificationPositionHint(position);
        }

        public static void XGameUiShowTextEntryAsync(
            string titleText,
            string descriptionText,
            string defaultText,
            XGameUiTextEntryInputScope inputScope,
            UInt32 maxTextLength,
            XGameUiShowTextEntryCompleted completionRoutine)
        {
            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                // extract result
                string resultText = null;
                UInt32 size;
                Int32 hresult = XGRInterop.XGameUiShowTextEntryResultSize(block, out size);
                if (HR.SUCCEEDED(hresult))
                {
                    if (size > 0)
                    {
                        Byte[] textData = new Byte[size];
                        UInt32 resultTextBufferUsed = 0;
                        hresult = XGRInterop.XGameUiShowTextEntryResult(block, size, textData, out resultTextBufferUsed);

                        if (HR.SUCCEEDED(hresult))
                        {
                            resultText = Converters.ByteArrayToString(textData);
                        }
                    }
                }
                completionRoutine(hresult, resultText);
            });

            Int32 hr = XGRInterop.XGameUiShowTextEntryAsync(
                asyncBlock,
                Converters.StringToNullTerminatedUTF8ByteArray(titleText),
                Converters.StringToNullTerminatedUTF8ByteArray(descriptionText),
                Converters.StringToNullTerminatedUTF8ByteArray(defaultText),
                inputScope,
                maxTextLength);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }

        public static void XGameUIShowWebAuthenticationAsync(
              XUserHandle requestingUser,
              string requestUri,
              string completionUri,
              XGameUiShowWebAuthenticationAsyncCompleted completionRoutine)
        {
            if (requestingUser == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
            }
            else
            {
                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (block =>
                {
                    SizeT bufferSize;
                    int hr1 = XGRInterop.XGameUiShowWebAuthenticationResultSize(block, out bufferSize);

                    if (HR.FAILED(hr1))
                        completionRoutine(hr1, null);

                    using (DisposableBuffer disposableBuffer = new DisposableBuffer(bufferSize.ToInt32()))
                    {
                        IntPtr ptrToBuffer;
                        SizeT bufferUsed;
                        int hr2 = XGRInterop.XGameUiShowWebAuthenticationResult(block, bufferSize, disposableBuffer.IntPtr, out ptrToBuffer, out bufferUsed);

                        if (HR.FAILED(hr2))
                        {
                            completionRoutine(hr2, null);
                        }
                        else
                        {
                            XGameUiWebAuthenticationResultData result = Converters.PtrToClass<XGameUiWebAuthenticationResultData, Interop.XGameUiWebAuthenticationResultData>(ptrToBuffer, (r => new XGameUiWebAuthenticationResultData(r)));
                            completionRoutine(hr2, result);
                        }
                    }
                }));
                int hr = XGRInterop.XGameUiShowWebAuthenticationAsync(asyncBlock, requestingUser.InteropHandle, Converters.StringToNullTerminatedUTF8ByteArray(requestUri), Converters.StringToNullTerminatedUTF8ByteArray(completionUri));

                if (!HR.FAILED(hr))
                    return;

                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }
    }
}
